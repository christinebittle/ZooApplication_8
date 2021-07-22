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
    public class SpeciesController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static SpeciesController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44324/api/");
        }

        // GET: Species/List
        public ActionResult List()
        {
            //objective: communicate with our Species data api to retrieve a list of Speciess
            //curl https://localhost:44324/api/Speciesdata/listSpeciess


            string url = "speciesdata/listspecies";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<SpeciesDto> Species = response.Content.ReadAsAsync<IEnumerable<SpeciesDto>>().Result;
            //Debug.WriteLine("Number of Speciess received : ");
            //Debug.WriteLine(Speciess.Count());


            return View(Species);
        }

        // GET: Species/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with our Species data api to retrieve one Species
            //curl https://localhost:44324/api/Speciesdata/findspecies/{id}

            DetailsSpecies ViewModel = new DetailsSpecies();

            string url = "speciesdata/findspecies/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            SpeciesDto SelectedSpecies = response.Content.ReadAsAsync<SpeciesDto>().Result;
            Debug.WriteLine("Species received : ");
            Debug.WriteLine(SelectedSpecies.SpeciesName);

            ViewModel.SelectedSpecies = SelectedSpecies;

            //showcase information about animals related to this species
            //send a request to gather information about animals related to a particular species ID
            url = "animaldata/listanimalsforspecies/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<AnimalDto> RelatedAnimals = response.Content.ReadAsAsync<IEnumerable<AnimalDto>>().Result;

            ViewModel.RelatedAnimals = RelatedAnimals;

            //show all trivia about this species
            url = "TriviaData/ListTriviasForSpecies/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<TriviaDto> RelatedTrivias = response.Content.ReadAsAsync<IEnumerable<TriviaDto>>().Result;

            ViewModel.RelatedTrivias = RelatedTrivias;


            return View(ViewModel);
        }

        public ActionResult Error()
        {

            return View();
        }

        // GET: Species/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Species/Create
        [HttpPost]
        public ActionResult Create(Species Species)
        {
            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(Species.SpeciesName);
            //objective: add a new Species into our system using the API
            //curl -H "Content-Type:application/json" -d @Species.json https://localhost:44324/api/Speciesdata/addSpecies 
            string url = "speciesdata/addspecies";


            string jsonpayload = jss.Serialize(Species);
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

        // GET: Species/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "speciesdata/findspecies/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            SpeciesDto selectedSpecies = response.Content.ReadAsAsync<SpeciesDto>().Result;
            return View(selectedSpecies);
        }

        // POST: Species/Update/5
        [HttpPost]
        public ActionResult Update(int id, Species Species)
        {

            string url = "speciesdata/updatespecies/" + id;
            string jsonpayload = jss.Serialize(Species);
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

        // GET: Species/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "speciesdata/findspecies/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            SpeciesDto selectedSpecies = response.Content.ReadAsAsync<SpeciesDto>().Result;
            return View(selectedSpecies);
        }

        // POST: Species/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "speciesdata/deletespecies/" + id;
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
