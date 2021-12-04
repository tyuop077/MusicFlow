using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
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
        Regex usernameRegex = new Regex("^(?=.{3,20}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$", RegexOptions.Compiled);
        public bool isEmailValid(string email)
        {
            try
            {
                new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromForm]string email, [FromForm]string username, [FromForm]string password, [FromForm]string password2)
        {
            if (!isEmailValid(email))
            {
                ViewData["tooltip"] = "Enter a valid email";
                return Page();
            }

            if (username is null || !usernameRegex.IsMatch(username))
            {
                ViewData["tooltip"] = "Invalid username, hover over username field for more info";
                return Page();
            }

            if (password is null || password.Length < 8)
            {
                ViewData["tooltip"] = "Password shouldn't be less, than 8 characters";
                return Page();
            }

            if (password != password2)
            {
                ViewData["tooltip"] = "Passwords aren't same, please try again";
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

            return Redirect("/");
        }
    }
}
