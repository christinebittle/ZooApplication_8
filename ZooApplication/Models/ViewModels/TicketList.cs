using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZooApplication.Models.ViewModels
{
    public class TicketList
    {
        public bool IsAdmin { get; set; }
        public IEnumerable<TicketDto> Tickets { get; set; }
    }
}