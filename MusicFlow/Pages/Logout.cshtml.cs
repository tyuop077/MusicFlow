using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MusicFlow.Pages
{
    public class LogoutModel : PageModel
    {
        public void OnGet()
        {
            Response.Cookies.Append("token", "", new CookieOptions
            {
                MaxAge = TimeSpan.FromDays(0)
            });
            //return RedirectToPage("Index"); //MVC
            Response.Redirect("/");
        }
    }
}
