using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using MusicFlow.Entities;
using MusicFlow.Services;

namespace MusicFlow.Pages.Forum
{
    public class TopicModel : PageModel
    {
        private Database db;
        public ForumThread thread { get; set; }
        public List<ForumContent> contents { get; set; }
        public TopicModel(Database database) => this.db = database;
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var threadRes = await db.FetchForumThread(id);
            if (threadRes.Status is DBReturnStatus.SUCCESS)
            {
                var contentsRes = await db.FetchThreadContents(id, 1);
                if (contentsRes.Status is DBReturnStatus.SUCCESS)
                {
                    thread = threadRes.Data;
                    contents = contentsRes.Data;
                    return Page();
                }
            }
            return NotFound();
        }
    }
}
