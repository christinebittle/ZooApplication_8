﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using ZooApplication.Models;
using ZooApplication.Models.ViewModels;
using System.Web.Script.Serialization;



namespace ZooApplication.Controllers
{
    public class AnimalController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static AnimalController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44324/api/");
        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }

        // GET: Animal/List?PageNum={PageNum}
        public ActionResult List(int PageNum=0)
        {
            //objective: communicate with our animal data api to retrieve a list of animals
            //curl https://localhost:44324/api/animaldata/listanimals

            AnimalList ViewModel = new AnimalList();
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin")) ViewModel.IsAdmin = true;
            else ViewModel.IsAdmin = false;
           
            string url = "animaldata/getanimalcount";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            // -- Start of Pagination Algorithm --

            // Find the total number of animals
            int AnimalCount = response.Content.ReadAsAsync<int>().Result;
            // Number of animals to display per page
            int PerPage = 4;
            // Determines the maximum number of pages (rounded up), assuming a page 0 start.
            int MaxPage = (int)Math.Ceiling((decimal)AnimalCount / PerPage) - 1;

            // Lower boundary for Max Page
            if (MaxPage < 0) MaxPage = 0;
            // Lower boundary for Page Number
            if (PageNum < 0) PageNum = 0;
            // Upper Bound for Page Number
            if (PageNum > MaxPage) PageNum = MaxPage;

            // The Record Index of our Page Start
            int StartIndex = PerPage * PageNum;

            //Helps us generate the HTML which shows "Page 1 of ..." on the list view
            ViewModel.PageNum = PageNum;
            ViewModel.PageSummary = " " + (PageNum + 1) + " of " + (MaxPage + 1) + " ";

            // -- End of Pagination Algorithm --

            //Send another request to get the page slice of the full list
            url = "AnimalData/ListAnimalsPage/" + StartIndex + "/" + PerPage;
            response = client.GetAsync(url).Result;

            // Retrieve the response of the HTTP Request
            IEnumerable<AnimalDto> SelectedAnimalsPage = response.Content.ReadAsAsync<IEnumerable<AnimalDto>>().Result;

            ViewModel.Animals = SelectedAnimalsPage;



            return View(ViewModel);
        }

        // GET: Animal/Details/5
        public ActionResult Details(int id)
        {
            DetailsAnimal ViewModel = new DetailsAnimal();

            
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin")) ViewModel.IsAdmin = true;
            else ViewModel.IsAdmin = false;

            //objective: communicate with our animal data api to retrieve one animal
            //curl https://localhost:44324/api/animaldata/findanimal/{id}

            string url = "animaldata/findanimal/"+id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            AnimalDto SelectedAnimal = response.Content.ReadAsAsync<AnimalDto>().Result;
            Debug.WriteLine("animal received : ");
            Debug.WriteLine(SelectedAnimal.AnimalName);

            ViewModel.SelectedAnimal = SelectedAnimal;

            //show associated keepers with this animal
            url = "keeperdata/listkeepersforanimal/"+id;
            response = client.GetAsync(url).Result;
            IEnumerable<KeeperDto> ResponsibleKeepers = response.Content.ReadAsAsync<IEnumerable<KeeperDto>>().Result;

            ViewModel.ResponsibleKeepers = ResponsibleKeepers;

            url = "keeperdata/listkeepersnotcaringforanimal/" + id;
                response = client.GetAsync(url).Result;
            IEnumerable<KeeperDto> AvailableKeepers = response.Content.ReadAsAsync<IEnumerable<KeeperDto>>().Result;

            ViewModel.AvailableKeepers = AvailableKeepers;


            return View(ViewModel);
        }


        //POST: Animal/Associate/{animalid}
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Associate(int id, int KeeperID)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("Attempting to associate animal :"+id+ " with keeper "+KeeperID);

            //call our api to associate animal with keeper
            string url = "animaldata/associateanimalwithkeeper/" + id + "/" + KeeperID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }


        //Get: Animal/UnAssociate/{id}?KeeperID={keeperID}
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult UnAssociate(int id, int KeeperID)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("Attempting to unassociate animal :" + id + " with keeper: " + KeeperID);

            //call our api to unassociate animal with keeper
            string url = "animaldata/unassociateanimalwithkeeper/" + id + "/" + KeeperID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }


        public ActionResult Error()
        {

            return View();
        }

        // GET: Animal/New
        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            //information about all species in the system.
            //GET api/speciesdata/listspecies

            string url = "speciesdata/listspecies";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<SpeciesDto> SpeciesOptions = response.Content.ReadAsAsync<IEnumerable<SpeciesDto>>().Result;

            return View(SpeciesOptions);
        }

        // POST: Animal/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Animal animal)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(animal.AnimalName);
            //objective: add a new animal into our system using the API
            //curl -H "Content-Type:application/json" -d @animal.json https://localhost:44324/api/animaldata/addanimal 
            string url = "animaldata/addanimal";

            
            string jsonpayload = jss.Serialize(animal);
            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }

            
        }

        // GET: Animal/Edit/5
        [Authorize(Roles="Admin")]
        public ActionResult Edit(int id)
        {
            UpdateAnimal ViewModel = new UpdateAnimal();

            //the existing animal information
            string url = "animaldata/findanimal/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AnimalDto SelectedAnimal = response.Content.ReadAsAsync<AnimalDto>().Result;
            ViewModel.SelectedAnimal = SelectedAnimal;

            // all species to choose from when updating this animal
            //the existing animal information
            url = "speciesdata/listspecies/";
            response = client.GetAsync(url).Result;
            IEnumerable<SpeciesDto> SpeciesOptions = response.Content.ReadAsAsync<IEnumerable<SpeciesDto>>().Result;

            ViewModel.SpeciesOptions = SpeciesOptions;

            return View(ViewModel);
        }

        // POST: Animal/Update/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Animal animal, HttpPostedFileBase AnimalPic)
        {
            GetApplicationCookie();//get token credentials   
            string url = "animaldata/updateanimal/" + id;
            string jsonpayload = jss.Serialize(animal);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);

            //update request is successful, and we have image data
            if (response.IsSuccessStatusCode && AnimalPic != null)
            {
                //Updating the animal picture as a separate request
                Debug.WriteLine("Calling Update Image method.");
                //Send over image data for player
                url = "AnimalData/UploadAnimalPic/" + id;
                //Debug.WriteLine("Received Animal Picture "+AnimalPic.FileName);

                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(AnimalPic.InputStream);
                requestcontent.Add(imagecontent, "AnimalPic", AnimalPic.FileName);
                response = client.PostAsync(url, requestcontent).Result;

                return RedirectToAction("List");
            }
            else if (response.IsSuccessStatusCode)
            {
                //No image upload, but update still successful
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Animal/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "animaldata/findanimal/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AnimalDto selectedanimal = response.Content.ReadAsAsync<AnimalDto>().Result;
            return View(selectedanimal);
        }

        // POST: Animal/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
            string url = "animaldata/deleteanimal/"+id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
