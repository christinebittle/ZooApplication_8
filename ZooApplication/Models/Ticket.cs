using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZooApplication.Models
{
    public class Ticket
    {
        [Key]
        public int TicketID { get; set; }
        public string TicketType { get; set; }
        //current price of this ticket
        public decimal TicketPrice { get; set; }
        
    }

    public class TicketDto
    {
        public int TicketID { get; set; }
        public string TicketType { get; set; }
        //current price of this ticket
        public decimal TicketPrice { get; set; }
    }
}