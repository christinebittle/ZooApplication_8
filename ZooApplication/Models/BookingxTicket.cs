using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZooApplication.Models
{
    // Explicit bridging table.
    // We need to store extra information like ticket price and paid qty
    public class BookingxTicket
    {
        public int BookingXTicketID { get; set; }

        [ForeignKey("Ticket")]
        public int TicketID { get; set; }
        public virtual Ticket Ticket { get; set; }

        [ForeignKey("Booking")]
        public int BookingID { get; set; }
        public virtual Booking Booking { get; set; }
        public int TicketQty { get; set; }

        //logs price of ticket when it was paid
        public decimal TicketPrice { get; set; }
    }

    public class BookingXTicketDto
    {
        public int BookingXTicketID { get; set; }
        public string TicketType { get; set; }

        public int TicketQty { get; set; }

        //logs price of ticket when it was paid
        public decimal TicketPrice { get; set; }

    }
}