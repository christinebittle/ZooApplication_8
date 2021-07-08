using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ZooApplication.Models
{
    public class Species
    {

        [Key]
        public int SpeciesID { get; set; }

        public string SpeciesName { get; set; }

        public bool SpeciesEndangered { get; set; }

        public IEnumerable<Trivia> Trivias { get; set; }

        //whether the species is currently available at the zoo
        public bool SpeciesAvailable { get; set; }

        //todo: show featured species on homepage
        public bool SpeciesFeatured { get; set; }
    }

    public class SpeciesDto
    {
        public int SpeciesID { get; set; }
        public string SpeciesName { get; set; }
        public bool SpeciesEndangered { get; set; }

    }
}