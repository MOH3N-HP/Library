using LIbrary.Models;
using Microsoft.AspNetCore.Mvc;
using LIbrary.Services;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace LIbrary.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string Name, string UserName, string Password)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var result = await AuthenticationServices.Register(UserName, Name, Password);
            if (!result)
            {
                ModelState.AddModelError("Id", "User Already Exists.");
                return View();
            }
            return RedirectToAction("SignIn");
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(string UserName, string Password)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await AuthenticationServices.Authenticate(UserName, Password);
            if (user == null)
            {
                ModelState.AddModelError("Id", "Either UserName or Password isn't corroct.");
                return View();
            }

            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.UserName),
                new ("UserId", user.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("MyCookieAuth", principal);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Index", "Home");
        }
    }
}
