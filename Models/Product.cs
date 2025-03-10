using System.ComponentModel.DataAnnotations;

namespace Flügger.Models
{
    public class Product
    {
        public string? ImagePath { get; set; }

        [Display(Name = "Varenummer")]
        public int ItemNumber { get; set; }

        [Display(Name = "Navn")]
        public string? Name { get; set; }

        [Display(Name = "Antal")]
        public int Storage { get; set; }
    }
}
