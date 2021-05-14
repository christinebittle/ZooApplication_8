using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ZooApplication.Models
{
    public class Keeper
    {

        [Key]
        public int KeeperID { get; set; }
        public string KeeperFirstName { get; set; }
        public string KeeperLastName { get; set; }


        //A keeper can take care of many animals
        public ICollection<Animal> Animals { get; set; }

    }
}