using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZooApplication.Models.ViewModels
{
    public class AnimalList
    {
        public bool IsAdmin { get; set; }
        public IEnumerable<AnimalDto> Animals { get; set; }
    }
}