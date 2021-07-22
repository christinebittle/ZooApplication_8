using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZooApplication.Models.ViewModels
{
    public class UpdateTrivia
    {
        public TriviaDto SelectedTrivia { get; set; }
        //to populate a dropdownlist of species this trivia can be attached to
        public IEnumerable<SpeciesDto> PotentialSpecies {get;set;}
    }
}