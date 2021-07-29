using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZooApplication.Models.ViewModels
{
    public class DetailsAnimal
    {
        public bool IsAdmin { get; set; }
        public AnimalDto SelectedAnimal { get; set; }
        public IEnumerable<KeeperDto> ResponsibleKeepers { get; set; }

        public IEnumerable<KeeperDto> AvailableKeepers { get; set; }
    }
}