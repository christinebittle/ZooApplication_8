using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ZooApplication.Models;
using System.Diagnostics;

using Microsoft.AspNet.Identity;

namespace ZooApplication.Controllers
{
    public class BookingDataController : ApiController
    {

        private ApplicationDbContext db = new ApplicationDbContext();


        /// <summary>
        /// Returns all Bookings in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Bookings in the database, including their associated species.
        /// </returns>
        /// <example>
        /// GET: api/BookingData/ListBookings
        /// </example>
        [HttpGet]
        [ResponseType(typeof(BookingDto))]
        [Authorize(Roles = "Admin,Guest")]
        public IHttpActionResult ListBookings()
        {
            List<Booking> Bookings = db.Bookings.ToList();
            List<BookingDto> BookingDtos = new List<BookingDto>();

            Bookings.ForEach(a => BookingDtos.Add(new BookingDto()
            {
                BookingID = a.BookingID,
                BookingName = a.BookingName,
                BookingPhone = a.BookingPhone,
                BookingDate = a.BookingDate
            }));

            return Ok(BookingDtos);
        }




        /// <summary>
        /// Returns all Bookings in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An Booking in the system matching up to the Booking ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the Booking</param>
        /// <example>
        /// GET: api/BookingData/FindBooking/5
        /// </example>
        [ResponseType(typeof(BookingDto))]
        [HttpGet]
        [Authorize(Roles = "Admin,Guest")]
        public IHttpActionResult FindBooking(int id)
        {
            
            Booking Booking = db.Bookings.Find(id);
            BookingDto BookingDto = new BookingDto()
            {
                BookingID = Booking.BookingID,
                BookingName = Booking.BookingName,
                BookingPhone = Booking.BookingPhone,
                BookingDate = Booking.BookingDate
            };
            if (Booking == null)
            {
                return NotFound();
            }

            return Ok(BookingDto);
        }

        /// <summary>
        /// Updates a particular Booking in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Booking ID primary key</param>
        /// <param name="Booking">JSON FORM DATA of an Booking</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/BookingData/UpdateBooking/5
        /// FORM DATA: Booking JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize(Roles="Admin,Guest")]
        public IHttpActionResult UpdateBooking(int id, Booking Booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Booking.BookingID)
            {

                return BadRequest();
            }

            db.Entry(Booking).State = EntityState.Modified;
          
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        

        /// <summary>
        /// Adds an Booking to the system
        /// </summary>
        /// <param name="Booking">JSON FORM DATA of an Booking</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Booking ID, Booking Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/BookingData/AddBooking
        /// FORM DATA: Booking JSON Object
        /// </example>
        [ResponseType(typeof(Booking))]
        [HttpPost]
        [Authorize(Roles="Admin,Guest")]
        public IHttpActionResult AddBooking(Booking Booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //attach the id
            Booking.UserID = User.Identity.GetUserId();

            db.Bookings.Add(Booking);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Booking.BookingID }, Booking);
        }

        /// <summary>
        /// Deletes an Booking from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the Booking</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/BookingData/DeleteBooking/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Booking))]
        [HttpPost]
        [Authorize(Roles="Admin,Guest")]
        public IHttpActionResult DeleteBooking(int id)
        {
            Booking Booking = db.Bookings.Find(id);
            if (Booking == null)
            {
                return NotFound();
            }

            

            db.Bookings.Remove(Booking);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookingExists(int id)
        {
            return db.Bookings.Count(e => e.BookingID == id) > 0;
        }

        
        /// <summary>
        /// Gathers information about all Bookings related to a particular Ticket ID
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Bookings in the database
        /// </returns>
        /// <param name="id">Ticket ID.</param>
        /// <example>
        /// GET: api/BookingData/ListBookingsForTicket/3
        /// </example>
        [HttpGet]
        [ResponseType(typeof(BookingXTicketDto))]
        public IHttpActionResult ListBookingsForTicket(int id)
        {
            //can't directly access the ticket's bookings
            //access the bookingxticket bridging table,
            //joining the tickets in
            List<BookingxTicket> BxTs = db.BookingxTickets.Where(bxt => bxt.TicketID == id).Include(bxt => bxt.Booking).ToList();

            List<BookingDto> BookingDtos = new List<BookingDto>();

            BxTs.ForEach(bxt => BookingDtos.Add(new BookingDto()
            {
                BookingID = bxt.Booking.BookingID,
                BookingName = bxt.Booking.BookingName,
                BookingPhone = bxt.Booking.BookingPhone,
                BookingDate = bxt.Booking.BookingDate
            }));

            return Ok(BookingDtos);
        }

    }
}
