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
    public class TicketDataController : ApiController
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all Tickets in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Tickets in the database
        /// </returns>
        /// <example>
        /// GET: api/TicketData/ListTickets
        /// </example>
        [HttpGet]
        [ResponseType(typeof(TicketDto))]
        public IHttpActionResult ListTickets()
        {
            List<Ticket> Tickets = db.Tickets.ToList();
            List<TicketDto> TicketDtos = new List<TicketDto>();

            Tickets.ForEach(a => TicketDtos.Add(new TicketDto()
            {
                TicketID = a.TicketID,
                TicketType = a.TicketType,
                TicketPrice = a.TicketPrice
            }));

            return Ok(TicketDtos);
        }

        /// <summary>
        /// Gathers information about all Tickets related to a particular Booking ID
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Tickets in the database
        /// </returns>
        /// <param name="id">Booking ID.</param>
        /// <example>
        /// GET: api/TicketData/ListTicketsForBooking/3
        /// </example>
        [HttpGet]
        [ResponseType(typeof(BookingXTicketDto))]
        public IHttpActionResult ListTicketsForBooking(int id)
        {
            //can't directly access the ticket's bookings
            //access the bookingxticket bridging table,
            //joining the tickets in
            List<BookingxTicket> BxTs = db.BookingxTickets.Where(bxt=>bxt.BookingID==id).Include(bxt=>bxt.Ticket).ToList();

            List<BookingXTicketDto> BxTDtos = new List<BookingXTicketDto>();

            BxTs.ForEach(bxt => BxTDtos.Add(new BookingXTicketDto()
            {
                BookingXTicketID = bxt.BookingXTicketID,
                TicketID=bxt.Ticket.TicketID,
                BookingID=bxt.Booking.BookingID,
                TicketType = bxt.Ticket.TicketType,
                TicketQty = bxt.TicketQty,
                TicketPrice = bxt.TicketPrice //what was paid, not current cost of ticket (bxt.Ticket.TicketPrice)
            }));

            return Ok(BxTDtos);
        }




        /// <summary>
        /// Associates a particular Booking with a particular Ticket
        /// </summary>
        /// <param name="Ticketid">The Ticket ID primary key</param>
        /// <param name="Bookingid">The Booking ID primary key</param>
        /// <param name="Qty">The number of Tickets</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// or
        /// HEADER: 400 (BAD REQUEST)
        /// </returns>
        /// <example>
        /// POST api/TicketData/AssociateTicketWithBooking/4/3/2
        /// </example>
        [HttpPost]
        [Route("api/TicketData/AssociateTicketWithBooking/{Ticketid}/{Bookingid}/{Qty}")]
        [Authorize(Roles="Admin,Guest")]
        public IHttpActionResult AssociateTicketWithBooking(int Ticketid, int Bookingid, int Qty)
        {
            //no negative quantity
            if (Qty < 0) return BadRequest();

            

            //Try to Find the ticket
            Ticket SelectedTicket = db.Tickets.Find(Ticketid);

            //Try to Find the booking
            Booking SelectedBooking = db.Bookings.Find(Bookingid);

            //if ticket or booking doesn't exist return 404
            if (SelectedTicket == null || SelectedBooking == null)
            {
                return NotFound();
            }

            //do not process if the (user is not an admin) and (the booking does not belong to the user)
            bool isAdmin = User.IsInRole("Admin");
            //Forbidden() isn't a natively implemented status like BadRequest()
            if (!isAdmin && (SelectedBooking.UserID != User.Identity.GetUserId())) return StatusCode(HttpStatusCode.Forbidden);

            //try to update an already existing association between the ticket and booking
            BookingxTicket BookingxTicket = db.BookingxTickets.Where(bxt => (bxt.TicketID == Ticketid && bxt.BookingID == Bookingid)).FirstOrDefault();
            if (BookingxTicket != null)
            {
                BookingxTicket.TicketQty = Qty;
                //assume previous price
            }
            //otherwise add a new association between the ticket and the booking
            else { 
                //Get the current price of the ticket
                decimal TicketPrice = SelectedTicket.TicketPrice;

                //Create a new instance of ticket x booking
                BookingxTicket NewBxT = new BookingxTicket()
                {
                    Ticket = SelectedTicket,
                    Booking = SelectedBooking,
                    TicketQty = Qty,
                    TicketPrice = TicketPrice
                };
                db.BookingxTickets.Add(NewBxT);
            }
            db.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Removes an association between a particular Booking and a particular Ticket
        /// function is deprecated (not in use). Just use a different qty with 'AssociateTicketWithBooking'
        /// </summary>
        /// <param name="BxTID">Booking X Ticket Primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/TicketData/AssociateTicketWithBooking/200
        /// </example>
        [HttpPost]
        [Route("api/TicketData/UnAssociateTicketWithBooking/{BxTID}")]
        [Authorize]
        public IHttpActionResult UnAssociateTicketWithBooking(int BxTID)
        {
            
            //Note: this could also be done with the two FK ticket ID and booking ID
            //find the booking x ticket
            BookingxTicket SelectedBxT = db.BookingxTickets.Find(BxTID);
            if (SelectedBxT == null) return NotFound();

            db.BookingxTickets.Remove(SelectedBxT);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Returns all Tickets in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An Ticket in the system matching up to the Ticket ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the Ticket</param>
        /// <example>
        /// GET: api/TicketData/FindTicket/5
        /// </example>
        [ResponseType(typeof(TicketDto))]
        [HttpGet]
        public IHttpActionResult FindTicket(int id)
        {
            Ticket Ticket = db.Tickets.Find(id);
            TicketDto TicketDto = new TicketDto()
            {
                TicketID = Ticket.TicketID,
                TicketType = Ticket.TicketType,
                TicketPrice = Ticket.TicketPrice
            };
            if (Ticket == null)
            {
                return NotFound();
            }

            return Ok(TicketDto);
        }

        /// <summary>
        /// Updates a particular Ticket in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Ticket ID primary key</param>
        /// <param name="Ticket">JSON FORM DATA of an Ticket</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/TicketData/UpdateTicket/5
        /// FORM DATA: Ticket JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateTicket(int id, Ticket Ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Ticket.TicketID)
            {

                return BadRequest();
            }

            db.Entry(Ticket).State = EntityState.Modified;
        
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(id))
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
        /// Adds an Ticket to the system
        /// </summary>
        /// <param name="Ticket">JSON FORM DATA of an Ticket</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Ticket ID, Ticket Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/TicketData/AddTicket
        /// FORM DATA: Ticket JSON Object
        /// </example>
        [ResponseType(typeof(Ticket))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddTicket(Ticket Ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Debug.WriteLine("Ticket type is :"+Ticket.TicketType);

            db.Tickets.Add(Ticket);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Ticket.TicketID }, Ticket);
        }

        /// <summary>
        /// Deletes an Ticket from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the Ticket</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/TicketData/DeleteTicket/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Ticket))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteTicket(int id)
        {
            Ticket Ticket = db.Tickets.Find(id);
            if (Ticket == null)
            {
                return NotFound();
            }

            db.Tickets.Remove(Ticket);
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

        private bool TicketExists(int id)
        {
            return db.Tickets.Count(e => e.TicketID == id) > 0;
        }

        
    }
}
