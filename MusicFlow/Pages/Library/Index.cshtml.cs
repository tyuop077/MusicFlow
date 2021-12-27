using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicFlow.Entities;
using MusicFlow.Services;

namespace MusicFlow.Pages.Library
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private Database db;
        public IndexModel(Database database) => this.db = database;
        public List<MusicLibrary> Library { get; set; }
        public async Task OnGetAsync()
        {
            Library = await db.FetchUserLibrary(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        public async Task<IActionResult> OnGetSong([FromQuery] Guid id)
        {
            DBResult<System.IO.Stream> result = await db.FetchSong(id);
            if (result.Status is DBReturnStatus.SUCCESS)
            {
                return File(result.Data, "application/octet-stream", $"{id}.mp4");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
