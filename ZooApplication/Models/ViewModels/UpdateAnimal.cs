using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZooApplication.Models.ViewModels
{
    public class UpdateAnimal
    {
        //This viewmodel is a class which stores information that we need to present to /Animal/Update/{}

        //the existing animal information

        public AnimalDto SelectedAnimal { get; set; }

        // all species to choose from when updating this animal

        public IEnumerable<SpeciesDto> SpeciesOptions { get; set; }
    }
}