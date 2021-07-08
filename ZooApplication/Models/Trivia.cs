using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZooApplication.Models
{
    public class Trivia
    {
        [Key]
        public string TriviaTitle { get; set; }
        public string TriviaDesc { get; set; }

        //each piece of trivia points to a species
        [ForeignKey("Species")]
        public int SpeciesID { get; set; }
        public virtual Species Species { get; set; }
    }

    public class TriviaDto
    {
        public string TriviaTitle { get; set; }
        public string TriviaDesc { get; set; }
        public int SpeciesID { get; set; }
        public string SpeciesName { get; set; }
    }
}