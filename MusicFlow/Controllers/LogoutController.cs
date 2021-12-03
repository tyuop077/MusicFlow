using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MusicFlow.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LogoutController : Controller
    {
        public IActionResult Index()
        {
            Response.Cookies.Append("token", "", new CookieOptions
            {
                MaxAge = TimeSpan.FromDays(0)
            });
            return Redirect("/");
        }
    }
}
