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
    public class TriviaController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static TriviaController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44324/api/");
        }

        // GET: Trivia/List
        public ActionResult List()
        {
            //objective: communicate with our Trivia data api to retrieve a list of Trivias
            //curl https://localhost:44324/api/Triviadata/listTrivias


            string url = "TriviaData/ListTrivias";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<TriviaDto> Trivias = response.Content.ReadAsAsync<IEnumerable<TriviaDto>>().Result;
            //Debug.WriteLine("Number of Trivias received : ");
            //Debug.WriteLine(Trivias.Count());


            return View(Trivias);
        }

        // GET: Trivia/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with our Trivia data api to retrieve one Trivia
            //curl https://localhost:44324/api/Triviadata/findTrivia/{id}

            string url = "TriviaData/FindTrivia/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            TriviaDto SelectedTrivia = response.Content.ReadAsAsync<TriviaDto>().Result;
            

            return View(SelectedTrivia);
        }

        public ActionResult Error()
        {

            return View();
        }

        // GET: Trivia/New
        public ActionResult New()
        {
            //get a list of species for this trivia to be attached to
            string url = "SpeciesData/ListSpecies";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<SpeciesDto> Species = response.Content.ReadAsAsync<IEnumerable<SpeciesDto>>().Result;
            return View(Species);
        }

        // POST: Trivia/Create
        [HttpPost]
        public ActionResult Create(Trivia Trivia)
        {
            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(Trivia.TriviaName);
            //objective: add a new Trivia into our system using the API
            //curl -H "Content-Type:application/json" -d @Trivia.json https://localhost:44324/api/Triviadata/addTrivia 
            string url = "TriviaData/AddTrivia";


            string jsonpayload = jss.Serialize(Trivia);
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

        // GET: Trivia/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateTrivia ViewModel = new UpdateTrivia();
            
            string url = "Triviadata/FindTrivia/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            TriviaDto SelectedTrivia = response.Content.ReadAsAsync<TriviaDto>().Result;
            ViewModel.SelectedTrivia = SelectedTrivia;

            //get all species data
            url = "SpeciesData/ListSpecies";
            response = client.GetAsync(url).Result;
            IEnumerable<SpeciesDto> Species = response.Content.ReadAsAsync<IEnumerable<SpeciesDto>>().Result;
            ViewModel.PotentialSpecies=Species;

            return View(ViewModel);
        }

        // POST: Trivia/Update/5
        [HttpPost]
        public ActionResult Update(int id, Trivia Trivia)
        {

            string url = "Triviadata/UpdateTrivia/" + id;
            string jsonpayload = jss.Serialize(Trivia);
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

        // GET: Trivia/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "Triviadata/FindTrivia/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            TriviaDto selectedTrivia = response.Content.ReadAsAsync<TriviaDto>().Result;
            return View(selectedTrivia);
        }

        // POST: Trivia/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "TriviaData/DeleteTrivia/" + id;
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