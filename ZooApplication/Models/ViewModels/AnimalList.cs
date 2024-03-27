using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZooApplication.Models.ViewModels
{
    public class AnimalList
    {
        //provide the page information in the animal list
        public string PageSummary { get; set; }

        public int PageNum { get; set; }

        public bool IsAdmin { get; set; }
        public IEnumerable<AnimalDto> Animals { get; set; }
    }
}