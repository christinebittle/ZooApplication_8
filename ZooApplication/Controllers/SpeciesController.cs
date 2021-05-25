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
    public class SpeciesController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static SpeciesController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44324/api/Speciesdata/");
        }

        // GET: Species/List
        public ActionResult List()
        {
            //objective: communicate with our Species data api to retrieve a list of Speciess
            //curl https://localhost:44324/api/Speciesdata/listSpeciess


            string url = "listspecies";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<SpeciesDto> Speciess = response.Content.ReadAsAsync<IEnumerable<SpeciesDto>>().Result;
            //Debug.WriteLine("Number of Speciess received : ");
            //Debug.WriteLine(Speciess.Count());


            return View(Speciess);
        }

        // GET: Species/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with our Species data api to retrieve one Species
            //curl https://localhost:44324/api/Speciesdata/findspecies/{id}

            string url = "findspecies/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            SpeciesDto selectedSpecies = response.Content.ReadAsAsync<SpeciesDto>().Result;
            Debug.WriteLine("Species received : ");
            Debug.WriteLine(selectedSpecies.SpeciesName);


            return View(selectedSpecies);
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
            string url = "addspecies";


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
            string url = "findspecies/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            SpeciesDto selectedSpecies = response.Content.ReadAsAsync<SpeciesDto>().Result;
            return View(selectedSpecies);
        }

        // POST: Species/Update/5
        [HttpPost]
        public ActionResult Update(int id, Species Species)
        {

            string url = "updatespecies/" + id;
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
            string url = "findspecies/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            SpeciesDto selectedSpecies = response.Content.ReadAsAsync<SpeciesDto>().Result;
            return View(selectedSpecies);
        }

        // POST: Species/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "deletespecies/" + id;
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
