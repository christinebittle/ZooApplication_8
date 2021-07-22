using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZooApplication.Models.ViewModels
{
    public class DetailsSpecies
    {
        public SpeciesDto SelectedSpecies { get; set; }
        //animals that are part of this species
        public IEnumerable<AnimalDto> RelatedAnimals { get; set; }
        //trivia for this species
        public IEnumerable<TriviaDto> RelatedTrivias { get; set; }
    }
}