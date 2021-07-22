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
    public class TicketController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static TicketController()
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

        // GET: Ticket/List
        public ActionResult List()
        {
            //objective: communicate with our Ticket data api to retrieve a list of Tickets
            //curl https://localhost:44324/api/Ticketdata/listTickets


            string url = "TicketData/ListTickets";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<TicketDto> Tickets = response.Content.ReadAsAsync<IEnumerable<TicketDto>>().Result;
            //Debug.WriteLine("Number of Tickets received : ");
            //Debug.WriteLine(Tickets.Count());


            return View(Tickets);
        }

        // GET: Ticket/Details/5
        public ActionResult Details(int id)
        {
            DetailsTicket ViewModel = new DetailsTicket();

            //objective: communicate with our Ticket data api to retrieve one Ticket
            //curl https://localhost:44324/api/Ticketdata/findTicket/{id}

            string url = "TicketData/FindTicket/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            TicketDto SelectedTicket = response.Content.ReadAsAsync<TicketDto>().Result;
            ViewModel.SelectedTicket = SelectedTicket;

            //Get Bookings for this ticket
            url = "BookingData/ListBookingsForTicket/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<BookingDto> Bookings = response.Content.ReadAsAsync<IEnumerable<BookingDto>>().Result;

            ViewModel.Bookings = Bookings;


            return View(ViewModel);
        }

        public ActionResult Error()
        {

            return View();
        }

        // GET: Ticket/New
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST: Ticket/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Ticket Ticket)
        {
            //validate request
            GetApplicationCookie();

            Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(Ticket.TicketName);
            //objective: add a new Ticket into our system using the API
            //curl -H "Content-Type:application/json" -d @Ticket.json https://localhost:44324/api/Ticketdata/addTicket 
            string url = "TicketData/AddTicket";


            string jsonpayload = jss.Serialize(Ticket);
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

        // GET: Ticket/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            string url = "TicketData/FindTicket/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            TicketDto SelectedTicket = response.Content.ReadAsAsync<TicketDto>().Result;
            return View(SelectedTicket);
        }

        // POST: Ticket/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Ticket Ticket)
        {
            //validate request
            GetApplicationCookie();

            string url = "TicketData/UpdateTicket/" + id;
            string jsonpayload = jss.Serialize(Ticket);
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

        // GET: Ticket/Delete/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "TicketData/FindTicket/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            TicketDto selectedTicket = response.Content.ReadAsAsync<TicketDto>().Result;
            return View(selectedTicket);
        }

        // POST: Ticket/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            //validate request
            GetApplicationCookie();

            string url = "TicketData/DeleteTicket/" + id;
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