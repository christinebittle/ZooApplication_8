using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZooApplication.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }
        public string BookingName { get; set; }
        public string BookingPhone { get; set; }
        public DateTime BookingDate { get; set; }

        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

    }

    public class BookingDto
    {
        public int BookingID { get; set; }
        public string BookingName { get; set; }
        public string BookingPhone { get; set; }
        public DateTime BookingDate { get; set; }
    }
}