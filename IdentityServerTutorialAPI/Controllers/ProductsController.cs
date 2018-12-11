using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServerTutorialAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerTutorialAPI.Controllers
{
    [Route("products")]
    public class ProductsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            var products = this.GetProductsFromDatabase();
            return new JsonResult(products);
        }

        private List<Product> GetProductsFromDatabase()
        {
            return new List<Product>()
            {
                new Product()
                {
                    Id = 1,
                    Name = "OLED TV",
                    Price = 499
                },
                new Product()
                {
                    Id = 2,
                    Name = "iDroid Phone",
                    Price = 349
                },
                new Product()
                {
                    Id = 3,
                    Name = "5.1 Surround System",
                    Price = 237
                }
            };
        }
    }
}