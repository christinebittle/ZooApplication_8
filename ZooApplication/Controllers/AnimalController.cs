using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using ZooApplication.Models;
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
            client.BaseAddress = new Uri("https://localhost:44324/api/animaldata/");
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

        // GET: Animal/List
        public ActionResult List()
        {
            //objective: communicate with our animal data api to retrieve a list of animals
            //curl https://localhost:44324/api/animaldata/listanimals

           
            string url = "listanimals";
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
            //objective: communicate with our animal data api to retrieve one animal
            //curl https://localhost:44324/api/animaldata/findanimal/{id}

            string url = "findanimal/"+id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            AnimalDto selectedanimal = response.Content.ReadAsAsync<AnimalDto>().Result;
            Debug.WriteLine("animal received : ");
            Debug.WriteLine(selectedanimal.AnimalName);
            

            return View(selectedanimal);
        }

        public ActionResult Error()
        {

            return View();
        }

        // GET: Animal/New
        
        public ActionResult New()
        {
            return View();
        }

        // POST: Animal/Create
        [HttpPost]
       
        public ActionResult Create(Animal animal)
        {
            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(animal.AnimalName);
            //objective: add a new animal into our system using the API
            //curl -H "Content-Type:application/json" -d @animal.json https://localhost:44324/api/animaldata/addanimal 
            string url = "addanimal";

            
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
        [Authorize]
        public ActionResult Edit(int id)
        {
            string url = "findanimal/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AnimalDto selectedanimal = response.Content.ReadAsAsync<AnimalDto>().Result;
            return View(selectedanimal);
        }

        // POST: Animal/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Animal animal)
        {
            
            string url = "updateanimal/"+id;
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
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "findanimal/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            AnimalDto selectedanimal = response.Content.ReadAsAsync<AnimalDto>().Result;
            return View(selectedanimal);
        }

        // POST: Animal/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            string url = "deleteanimal/"+id;
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
