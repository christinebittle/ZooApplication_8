using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZooApplication.Models.ViewModels
{
    public class DetailsKeeper
    {

        public KeeperDto SelectedKeeper { get; set; }
        public IEnumerable<AnimalDto> KeptAnimals { get; set; }
    }
}