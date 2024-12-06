using Blog.Core.Entities;
using Blog.Core.Interfaces;
using Blog.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(AccountModels.AccountLoginModels model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if user exists in the database
            var user = await _unitOfWork.Users
                .FindAsync(u => u.Userlogin == model.UserName && u.Password == model.Password);
            var userEntity = user.FirstOrDefault();

            if (userEntity != null)
            {
                // Create claims for the authenticated user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userEntity.Userlogin),
                    new Claim(ClaimTypes.Email, userEntity.Email ?? "")
                };

                // Set up the user identity and principal
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign in the user with the specified scheme
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redirect to the home page upon successful login
                return RedirectToAction("Index", "Home");
            }

            // If login fails, show an error message
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        // GET: Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
