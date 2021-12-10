using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicFlow.Entities;
using MusicFlow.Services;

namespace MusicFlow.Pages
{
    public class LoginModel : PageModel
    {
        private AuthManager authManager;
        private Database db;
        public LoginModel(AuthManager authManager, Database database)
        {
            this.authManager = authManager;
            this.db = database;
        }
        public PageResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAsync([FromForm] string email, [FromForm] string password)
        {
            if (email is null || password is null)
            {
                ViewData["tooltip"] = "Wrong email or password";
                return Page();
            }

            byte[] hashedPassword = authManager.HashPassword(password);

            DBResult<User> res = await db.LoginUser(email, hashedPassword);

            if (res.Status == DBReturnStatus.SUCCESS)
            {
                string token = authManager.GenerateToken(res.Data);

                Response.Cookies.Append("token", token, new CookieOptions
                {
                    MaxAge = TimeSpan.FromDays(7)
                });
                return RedirectToPage("Index");
            }
            else
            {
                ViewData["tooltip"] = "Wrong email or password";
                return Page();
            }
        }
    }
}
