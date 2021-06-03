using System;
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
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44324/api/");
        }

        // GET: Animal/List
        public ActionResult List()
        {
            //objective: communicate with our animal data api to retrieve a list of animals
            //curl https://localhost:44324/api/animaldata/listanimals

           
            string url = "animaldata/listanimals";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<AnimalDto> animals = response.Content.ReadAsAsync<IEnumerable<AnimalDto>>().Result;
            //Debug.WriteLine("Number of animals received : ");
            //Debug.WriteLine(animals.Count());


            return View(animals);
        }

        // GET: Animal/Details/5
        public ActionResult Details(int id)
        {
            DetailsAnimal ViewModel = new DetailsAnimal();

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
        public ActionResult Associate(int id, int KeeperID)
        {
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
        public ActionResult UnAssociate(int id, int KeeperID)
        {
            Debug.WriteLine("Attempting to unassociate animal :" + id + " with keeper: " + KeeperID);

            //call our api to associate animal with keeper
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
        public ActionResult Create(Animal animal)
        {
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
        public ActionResult Update(int id, Animal animal)
        {
            
            string url = "animaldata/updateanimal/"+id;
            string jsonpayload = jss.Serialize(animal);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Animal/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "animaldata/findanimal/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AnimalDto selectedanimal = response.Content.ReadAsAsync<AnimalDto>().Result;
            return View(selectedanimal);
        }

        // POST: Animal/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
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
