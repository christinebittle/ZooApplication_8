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
    public class KeeperController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static KeeperController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44324/api/");
        }

        // GET: Keeper/List
        public ActionResult List()
        {
            //objective: communicate with our Keeper data api to retrieve a list of Keepers
            //curl https://localhost:44324/api/Keeperdata/listkeepers


            string url = "keeperdata/listkeepers";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<KeeperDto> Keepers = response.Content.ReadAsAsync<IEnumerable<KeeperDto>>().Result;
            //Debug.WriteLine("Number of Keepers received : ");
            //Debug.WriteLine(Keepers.Count());


            return View(Keepers);
        }

        // GET: Keeper/Details/5
        public ActionResult Details(int id)
        {
            DetailsKeeper ViewModel = new DetailsKeeper();

            //objective: communicate with our Keeper data api to retrieve one Keeper
            //curl https://localhost:44324/api/Keeperdata/findkeeper/{id}

            string url = "keeperdata/findKeeper/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            KeeperDto SelectedKeeper = response.Content.ReadAsAsync<KeeperDto>().Result;
            Debug.WriteLine("Keeper received : ");
            Debug.WriteLine(SelectedKeeper.KeeperFirstName);

            ViewModel.SelectedKeeper = SelectedKeeper;

            //show all animals under the care of this keeper
            url = "animaldata/listanimalsforkeeper/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<AnimalDto> KeptAnimals = response.Content.ReadAsAsync<IEnumerable<AnimalDto>>().Result;

            ViewModel.KeptAnimals = KeptAnimals;


            return View(ViewModel);
        }

        public ActionResult Error()
        {

            return View();
        }

        // GET: Keeper/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Keeper/Create
        [HttpPost]
        public ActionResult Create(Keeper Keeper)
        {
            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(Keeper.KeeperName);
            //objective: add a new Keeper into our system using the API
            //curl -H "Content-Type:application/json" -d @Keeper.json https://localhost:44324/api/Keeperdata/addKeeper 
            string url = "keeperdata/addkeeper";


            string jsonpayload = jss.Serialize(Keeper);
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

        // GET: Keeper/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "keeperdata/findkeeper/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            KeeperDto selectedKeeper = response.Content.ReadAsAsync<KeeperDto>().Result;
            return View(selectedKeeper);
        }

        // POST: Keeper/Update/5
        [HttpPost]
        public ActionResult Update(int id, Keeper Keeper)
        {

            string url = "keeperdata/updatekeeper/" + id;
            string jsonpayload = jss.Serialize(Keeper);
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

        // GET: Keeper/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "keeperdata/findkeeper/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            KeeperDto selectedKeeper = response.Content.ReadAsAsync<KeeperDto>().Result;
            return View(selectedKeeper);
        }

        // POST: Keeper/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "keeperdata/deletekeeper/" + id;
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
