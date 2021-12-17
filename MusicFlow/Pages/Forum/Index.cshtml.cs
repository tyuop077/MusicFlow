using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MusicFlow.Pages.Forum
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync([FromForm] string topic)
        {
            if (topic is null || topic == "")
            {
                ViewData["error"] = "Topic can't be empty";
                return Page();
            }
            return Page();
        }
    }
}
