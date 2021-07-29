using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZooApplication.Models.ViewModels
{
    public class SpeciesList
    {
        public bool IsAdmin { get; set; }
        public IEnumerable<SpeciesDto> Species { get; set; }
    }
}