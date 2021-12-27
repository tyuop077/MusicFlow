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
        public async Task<RedirectResult> OnPostSendMessage(int id, [FromForm] string content, [FromForm] int rid)
        {
            string oid = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            int result = await db.CreateThreadMessage(id, oid, content, rid);
            return Redirect($"/forum/{id}#m{result}");
        }
        public async Task<EmptyResult> OnPostDeleteMessage(int id, [FromForm] int mid)
        {
            string oid = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            DBReturnStatus result = await db.DeleteForumMessage(mid, oid);
            HttpContext.Response.StatusCode = result is DBReturnStatus.SUCCESS ? 200 : 404;
            return new EmptyResult();
        }
        public async Task<EmptyResult> OnPostEditMessage(int id, [FromForm] int mid, [FromForm] string content)
        {
            string oid = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            DBReturnStatus result = await db.EditForumMessage(mid, oid, content);
            HttpContext.Response.StatusCode = result is DBReturnStatus.SUCCESS ? 200 : 404;
            return new EmptyResult();
        }
    }
}
