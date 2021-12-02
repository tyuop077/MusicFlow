using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MusicFlow.Pages
{
    public class LoginModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAsync([FromForm] string email, [FromForm] string password)
        {
            if (!ModelState.IsValid) // TODO implement DB user check
            {
                ViewData["tooltip"] = "Couldn't get username and password data";
                return Page();
            }

            // TODO implement DB user check

            ViewData["tooltip"] = $"Test error response, data: ({email}; {password})";

            return Page();
            //return RedirectToPage("./Index");
        }
    }
}
