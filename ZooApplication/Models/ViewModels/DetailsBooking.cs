using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZooApplication.Models.ViewModels
{
    public class DetailsBooking
    {
        public BookingDto SelectedBooking { get; set; }
        public IEnumerable<BookingXTicketDto> BookedTickets { get; set; }

        
    }
}