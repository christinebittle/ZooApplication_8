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
            bool isAdmin = User.IsInRole("Admin");

            //Admins see all, guests only see their own
            List<Booking> Bookings;
            Debug.WriteLine("id is "+User.Identity.GetUserId());
            if (isAdmin) Bookings = db.Bookings.ToList();
            else
            {
                string UserId = User.Identity.GetUserId();
                Bookings = db.Bookings.Where(b => b.UserID == UserId).ToList();
            }

            List<BookingDto> BookingDtos = new List<BookingDto>();

            Bookings.ForEach(b => BookingDtos.Add(new BookingDto()
            {
                BookingID = b.BookingID,
                BookingName = b.BookingName,
                BookingPhone = b.BookingPhone,
                BookingDate = b.BookingDate,
                UserId=b.UserID
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
                BookingDate = Booking.BookingDate,
                UserId=Booking.UserID
                
            };
            if (Booking == null)
            {
                return NotFound();
            }

            //do not process if the (user is not an admin) and (the booking does not belong to the user)
            bool isAdmin = User.IsInRole("Admin");
            //Forbidden() isn't a natively implemented status like BadRequest()
            if (!isAdmin && (Booking.UserID != User.Identity.GetUserId())) return StatusCode(HttpStatusCode.Forbidden);


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
                Debug.WriteLine("bad model state");
                return BadRequest(ModelState);
            }

            if (id != Booking.BookingID)
            {
                Debug.WriteLine("id mismatch");
                return BadRequest();
            }

            //do not process if the (user is not an admin) and (the booking does not belong to the user)
            bool isAdmin = User.IsInRole("Admin");
            //Forbidden() isn't a natively implemented status like BadRequest()
            if (!isAdmin && (Booking.UserID != User.Identity.GetUserId()))
            {
                Debug.WriteLine("not allowed. booking user"+ Booking.UserID+" user "+User.Identity.GetUserId());
                return StatusCode(HttpStatusCode.Forbidden);
            }

            db.Entry(Booking).State = EntityState.Modified;
            //do not modify the attached user id on update
            db.Entry(Booking).Property(b => b.UserID).IsModified = false;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                {
                    Debug.WriteLine("not found");
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

            //add one of each ticket at 0 qty
            List<Ticket> Tickets = db.Tickets.ToList();
            Tickets.ForEach(t =>
                db.BookingxTickets.Add(
                    new BookingxTicket
                    {
                        TicketID = t.TicketID,
                        BookingID = Booking.BookingID,
                        TicketQty = 0,
                        TicketPrice = t.TicketPrice
                    }
                )
            );
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

            //do not process if the (user is not an admin) and (the booking does not belong to the user)
            bool isAdmin = User.IsInRole("Admin");
            //Forbidden() isn't a natively implemented status like BadRequest()
            if (!isAdmin && (Booking.UserID != User.Identity.GetUserId())) return StatusCode(HttpStatusCode.Forbidden);


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
