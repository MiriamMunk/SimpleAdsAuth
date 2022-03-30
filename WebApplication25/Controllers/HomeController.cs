using ClassLibrary1;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication25.Models;

namespace WebApplication25.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimpleAd;Integrated Security=true;";

        public IActionResult Index()
        {
            AdRepository ar = new(_connectionString);
            return View(new ViewModel { Ads = ar.GetAds() });
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(string name, string email, string password)
        {
            AdRepository ar = new(_connectionString);
            ar.AddPerson(name, email, password);
            return Redirect("/home/logIn");
        }

        public IActionResult LogIn()
        {
            return View(new ViewModel { Message = (string)TempData["message"] });
        }

        [HttpPost]
        public IActionResult LogIn(string email, string password)
        {
            AdRepository ar = new(_connectionString);
            User user = ar.LogIn(email, password);
            if (user == null)
            {
                TempData["message"] = "invalid login";
                return Redirect("/home/login");
            }

            var claims = new List<Claim>
            {
                 new Claim("user", email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            return Redirect("/home/NewAd");
        }

        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }
        [HttpPost]
        public IActionResult NewAd(string phone, string description)
        {
            AdRepository ar = new(_connectionString);
            ar.AddAd(phone, description, User.Identity.Name);
            return Redirect("/home/index");
        }
        public IActionResult delete(int id)
        {
            AdRepository ar = new(_connectionString);
            ar.DeleteAd(id);
            return Redirect("/home/index");
        }
        public IActionResult logOut()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/home/index");
        }
        public IActionResult MyAccount()
        {
            AdRepository ar = new(_connectionString);
            return View(new ViewModel { Ads = ar.GetLogedInUsersAds(ar.GetByEmail(User.Identity.Name).Id) });
        }
    }
}
