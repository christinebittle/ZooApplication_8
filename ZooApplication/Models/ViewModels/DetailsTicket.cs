using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZooApplication.Models.ViewModels
{
    public class DetailsTicket
    {
        public bool IsAdmin { get;set; }
        public TicketDto SelectedTicket { get; set; }
        public IEnumerable<BookingDto> Bookings { get; set; }
    }
}