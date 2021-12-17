using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicFlow.Entities;
using MusicFlow.Services;

namespace MusicFlow.Pages.Forum
{
    public class IndexModel : PageModel
    {
        private Database db;
        public IndexModel(Database database)
        {
            this.db = database;
        }
        public List<ForumThread> Threads { get; set; }
        public async Task OnGetAsync()
        {
            Threads = await db.FetchForumThreads(1);
        }
        public async Task<RedirectResult> OnPostAsync([FromForm] string topic)
        {
            int tid = await db.CreateForumThread(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), topic ?? "Untitled thread");
            return Redirect($"/forum/{tid}");
        }
    }
}
