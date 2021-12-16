using System.Web;
using Microsoft.AspNetCore.Diagnostics;
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
                    Response.Redirect($"/login?r={HttpUtility.UrlEncode(HttpContext.Features.Get<IStatusCodeReExecuteFeature>().OriginalPath)}");
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
