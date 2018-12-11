using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdentityServerTutorialMVCClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace IdentityServerTutorialMVCClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult ProtectedInfo()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ProductsCallWithHttpContext()
        {
            var accessToken = HttpContext.GetTokenAsync("access_token").Result;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = client.GetStringAsync("https://localhost:5001/products").Result;

            var products = JsonConvert.DeserializeObject<List<Product>>(content);

            var viewModel = new ProductsViewModel()
            {
                Products = products
            };

            return View("Products", viewModel);
        }

        public IActionResult ProductsCallWithTokenClient()
        {
            // discover endpoints from metadata
            var disco = DiscoveryClient.GetAsync("https://localhost:5000").Result;
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                throw new Exception(disco.Error);
            }

            // request token for clientcredentials client
            var tokenClient = new TokenClient(disco.TokenEndpoint, "mvc", "secret");
            var tokenResponse = tokenClient.RequestClientCredentialsAsync("api1").Result;

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = client.GetAsync("https://localhost:5001/products").Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
            else
            {
                var content = response.Content.ReadAsStringAsync().Result;

                var products = JsonConvert.DeserializeObject<List<Product>>(content);

                var viewModel = new ProductsViewModel()
                {
                    Products = products
                };

                return View("Products", viewModel);
            }
        }
    }
}
