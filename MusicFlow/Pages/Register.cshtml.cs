using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MusicFlow.Pages
{
    public class RegisterModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAsync([FromForm]string email, [FromForm]string password, [FromBody]string password2)
        {
            if (!ModelState.IsValid)
            {
                ViewData["tooltip"] = "Couldn't get username and password data";
                return Page();
            }

            // TODO implement DB user check

            ViewData["tooltip"] = "Test error response";

            return Page();
        }
    }
}
