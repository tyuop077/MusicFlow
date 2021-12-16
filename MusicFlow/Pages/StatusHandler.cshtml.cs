using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MusicFlow.Pages
{
    public class StatusesHandlerModel : PageModel
    {
        public void OnGet(int code)
        {
            switch(code)
            {
                case 401:
                    Response.Redirect("/login");
                    return;
                case 404:
                    ViewData["Header"] = "Page not found... :(";
                    ViewData["Title"] = "404 Not Found - Music Flow";
                    ViewData["Message"] = "This page no longer exists or forwards to invalid path, consider using search page, it may help";
                    return;
                default:
                    ViewData["Header"] = $"Error {code}";
                    ViewData["Title"] = $"Error {code} - Music Flow";
                    ViewData["Message"] = "Unknown error happened, please report this to us on about page";
                    return;
            }
        }
    }
}
