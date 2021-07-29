using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZooApplication.Models.ViewModels
{
    public class KeeperList
    {
        public bool IsAdmin { get; set; }
        public IEnumerable<KeeperDto> Keepers { get; set; }
    }
}