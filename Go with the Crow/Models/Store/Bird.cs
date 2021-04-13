using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Go_with_the_Crow.Models.Store
{
    public class Bird
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 2, ErrorMessage = "{0} must be between {2} and {1} letters!")]
        [Display(Name = "Name")]
        public string BirdName { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "{0} must be between {1} and {2}!")]
        public int Age { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "{0} must be at least {1}!")]
        [Display(Name = "Adoption Fee")]
        public double AdoptionFee { get; set; }

        [Required]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "{0} must be between {2} and {1} letters!")]
        public string Species { get; set; }

        [Display(Name = "List Date")]
        public DateTime ListDate { get; set; }

        public string AdoptedBy { get; set; } //FK TO USER ID 

        public string Description { get; set; }

        [Display(Name ="Available for Adoption")]
        public bool IsAvailable { get; set; }

        public string ImagePath { get; set; }

        public string PostedBy { get; set; }
    }
}