using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicFlow.Entities;
using MusicFlow.Services;

namespace MusicFlow.Pages.Forum
{
    [Authorize]
    public class ThreadModel : PageModel
    {
        private Database db;
        public ForumThread Thread { get; set; }
        public List<ForumContent> Contents { get; set; }
        public ThreadModel(Database database) => this.db = database;
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var threadRes = await db.FetchForumThread(id);
            if (threadRes.Status is DBReturnStatus.SUCCESS)
            {
                Contents = await db.FetchThreadContents(id, 1);
                Thread = threadRes.Data;
                ViewData["Title"] = $"{Thread.Topic} - Music Flow";
                return Page();
            }
            return NotFound();
        }
        public async Task<RedirectResult> OnPostSendMessage(int id, [FromForm] string content)
        {
            string oid = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            int result = await db.CreateThreadMessage(id, oid, content, null);
            return Redirect($"/forum/{id}#m{result}");
        }
    }
}
