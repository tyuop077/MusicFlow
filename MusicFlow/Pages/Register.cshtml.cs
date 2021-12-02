using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicFlow.Entities;
using MusicFlow.Services;

namespace MusicFlow.Pages
{
    [ValidateAntiForgeryToken]
    public class RegisterModel : PageModel
    {
        private AuthManager authManager;
        private Database db;
        public RegisterModel(AuthManager authManager, Database database)
        {
            this.authManager = authManager;
            this.db = database;
        }
        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromForm]string email, [FromForm]string username, [FromForm]string password, [FromForm]string password2)
        {
            /*if (!ModelState.IsValid)
            {
                string error = String.Join(" | ", ModelState.Values
                    .SelectMany(entry => entry.Errors)
                    .Select(e => e.ErrorMessage));
                ViewData["tooltip"] = $"Couldn't get username and password data: {error}";
                return Page();
            }*/

            if (password != password2)
            {
                ViewData["tooltip"] = $"Passwords aren't same {password}; {password2}";
                return Page();
            }

            if (password.Length < 8)
            {
                ViewData["tooltip"] = "Password shouldn't be less, than 8 characters";
                return Page();
            }

            User user = new User
            {
                Email = email,
                Username = username,
                Password = authManager.HashPassword(password)
            };

            DBReturnStatus dbRes = await db.RegisterUser(user);

            if (dbRes == DBReturnStatus.ALREADY_EXISTS)
            {
                ViewData["tooltip"] = "That user already exists!";
                return Page();
            }

            string token = authManager.GenerateToken(user);

            Response.Cookies.Append("token", token, new CookieOptions
            {
                MaxAge = TimeSpan.FromDays(7)
            });

            // TODO implement DB user check

            ViewData["tooltip"] = $"Test error response, hashed password: {user.Password}";

            return Page();
        }
    }
}
