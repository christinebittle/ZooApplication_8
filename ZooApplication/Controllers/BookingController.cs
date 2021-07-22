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

using Microsoft.AspNet.Identity;

namespace ZooApplication.Controllers
{
    public class BookingController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static BookingController()
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

        // GET: Booking/List
        [Authorize(Roles = "Admin,Guest")]
        public ActionResult List()
        {
            GetApplicationCookie();
            //objective: communicate with our Booking data api to retrieve a list of Bookings
            //curl https://localhost:44324/api/Bookingdata/listBookings


            string url = "Bookingdata/listBookings";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<BookingDto> Bookings = response.Content.ReadAsAsync<IEnumerable<BookingDto>>().Result;
            //Debug.WriteLine("Number of Bookings received : ");
            //Debug.WriteLine(Bookings.Count());


            return View(Bookings);
        }

        // GET: Booking/Details/5
        [Authorize(Roles = "Admin,Guest")]
        public ActionResult Details(int id)
        {
            GetApplicationCookie();
            DetailsBooking ViewModel = new DetailsBooking();

            //objective: communicate with our Booking data api to retrieve one Booking
            //curl https://localhost:44324/api/Bookingdata/findBooking/{id}

            string url = "BookingData/FindBooking/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            
            BookingDto SelectedBooking = response.Content.ReadAsAsync<BookingDto>().Result;
            
            ViewModel.SelectedBooking = SelectedBooking;

            //Show tickets associated with this booking
            url = "TicketData/ListTicketsForBooking/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<BookingXTicketDto> Tickets = response.Content.ReadAsAsync<IEnumerable<BookingXTicketDto>>().Result;

            ViewModel.BookedTickets = Tickets;

            

            return View(ViewModel);
        }


        //POST: Booking/Associate/{BookingID}/{TicketID}/{Qty}
        [Route("Booking/Associate/{BookingID}/{TicketID}/{Qty}")]
        [HttpPost]
        [Authorize(Roles = "Admin,Guest")]
        public ActionResult Associate( int TicketID, int BookingID, int Qty)
        {
            GetApplicationCookie();//get token credentials
            

            //call our api to associate Booking with keeper
            string url = "TicketData/AssociateTicketWithBooking/"+TicketID+"/"+BookingID+"/"+Qty;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + BookingID);
        }


        //Get: Booking/UnAssociate/{BxTID}?BookingID={BookingID}
        [HttpGet]
        [Authorize(Roles = "Admin,Guest")]
        public ActionResult UnAssociate(int id, int BookingID)
        {
            GetApplicationCookie();//get token credentials
            

            //call our api to remove a booking x ticket
            string url = "TicketData/AssociateTicketWithBooking/"+id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + BookingID);
        }


        public ActionResult Error()
        {

            return View();
        }

        // GET: Booking/New
        [Authorize(Roles = "Admin,Guest")]
        public ActionResult New()
        {
            
            return View();
        }

        // POST: Booking/Create
        [HttpPost]
        [Authorize(Roles = "Admin,Guest")]
        public ActionResult Create(Booking Booking)
        {
            GetApplicationCookie();//get token credentials
           
            //Debug.WriteLine(Booking.BookingName);
            //objective: add a new Booking into our system using the API
            //curl -H "Content-Type:application/json" -d @Booking.json https://localhost:44324/api/Bookingdata/addBooking 
            string url = "Bookingdata/addBooking";


            string jsonpayload = jss.Serialize(Booking);
            Debug.WriteLine("the json payload is :");
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

        // GET: Booking/Edit/5
        [Authorize(Roles = "Admin,Guest")]
        public ActionResult Edit(int id)
        {
            
            //the existing Booking information
            string url = "Bookingdata/findBooking/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BookingDto SelectedBooking = response.Content.ReadAsAsync<BookingDto>().Result;

            
            return View(SelectedBooking);
        }

        // POST: Booking/Update/5
        [HttpPost]
        [Authorize(Roles = "Admin,Guest")]
        public ActionResult Update(int id, Booking Booking)
        {
            GetApplicationCookie();//get token credentials   
            string url = "Bookingdata/updateBooking/" + id;
            string jsonpayload = jss.Serialize(Booking);
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

        // GET: Booking/Delete/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "Bookingdata/findBooking/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BookingDto selectedBooking = response.Content.ReadAsAsync<BookingDto>().Result;
            return View(selectedBooking);
        }

        // POST: Booking/Delete/5
        [HttpPost]
        [Authorize(Roles="Admin,Guest")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
            string url = "Bookingdata/deleteBooking/" + id;
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