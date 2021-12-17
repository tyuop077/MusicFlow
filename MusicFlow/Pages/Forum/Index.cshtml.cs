using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicFlow.Entities;

namespace MusicFlow.Pages.Forum
{
    public class IndexModel : PageModel
    {
        public ForumThread[] Threads { get; set; }
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
