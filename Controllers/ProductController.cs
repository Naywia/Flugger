using Flügger.Models;
using Microsoft.AspNetCore.Mvc;

namespace Flügger.Controllers
{
    public static class ProductController
    {
        private static List<Product> _products = [
            new() { ImagePath = "/images/CTMWALL5.png", ItemNumber = 1, Name = "Væg Maling", Storage = 3 },
            new() { ImagePath = "/images/FACADE.png", ItemNumber = 2, Name = "Facade Universal", Storage = 5 },
            new() { ImagePath = "/images/FL2IN1CL.png", ItemNumber = 3, Name = "2-in-1 WoodTex Classic", Storage = 1 },
            new() { ImagePath = "/images/FLUTEX.png", ItemNumber = 4, Name = "Flutex 7S", Storage = 7 },
            new() { ImagePath = "/images/H2O.png", ItemNumber = 5, Name = "Væg primer H2O", Storage = 8 },
            new() { ImagePath = "/images/sandplast.png", ItemNumber = 6, Name = "Sandplast LG", Storage = 4 }
        ];

        public static List<Product> GetProducts()
        {
            return _products;
        }
    }
}
