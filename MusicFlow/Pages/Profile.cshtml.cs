using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicFlow.Entities;
using MusicFlow.Services;

namespace MusicFlow.Pages
{
    public class ProfileModel : PageModel
    {
        private AuthManager authManager;
        private Database db;
        public ProfileModel(AuthManager authManager, Database database)
        {
            this.authManager = authManager;
            this.db = database;
        }

        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var claim = HttpContext.User;
            if (claim.FindFirstValue(ClaimTypes.NameIdentifier) is null)
            {
                return RedirectToPage("Index");
            }
            User user = await db.FetchUser(claim.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewData["email"] = user.Email;
            if (user.Avatar is null)
            {
                string EmailMD5 = CreateMD5(user.Email).ToLower();
                ViewData["avatar"] = $"https://www.gravatar.com/avatar/{EmailMD5}?d=https%3A%2F%2Fui-avatars.com%2Fapi%2F/{claim.FindFirstValue(ClaimTypes.Name)}/128/20B2AA";
            }
            else
            {
                ViewData["avatar"] = $"https://i.imgur.com/{user.Avatar}";
            }
            return Page();
        }
    }
}
