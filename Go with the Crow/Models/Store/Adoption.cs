using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Go_with_the_Crow.Models.Store
{
    public class Adoption
    {
        [Key] public int ID { get; set; }
        public string AdopterID { get; set; }
        public int BirdID { get; set; }
        public DateTime AdoptDate { get; set; }
        
    }
}