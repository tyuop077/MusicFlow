using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
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
            if (!claim.Identity.IsAuthenticated)
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
        public Task<IActionResult> OnPostAsync([FromForm]string action, [FromForm]IFormFile image, [FromForm]string email, [FromForm]string currentpwd, [FromForm]string newpwd, [FromForm]string newpwd2)
        {
            switch (action)
            {
                case "email":
                    return ChangeEmail(email);
                case "password":
                    return ChangePassword(currentpwd, newpwd, newpwd2);
                case "set_avatar":
                case "reset_avatar":
                    ViewData["error"] = "Not implemented";
                    return OnGetAsync();
                default:
                    ViewData["error"] = "Unknown action";
                    return OnGetAsync();
            }
        }
        public string GetUserId()
        {
            return HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        public async Task<IActionResult> ChangeEmail(string email)
        {
            string id = GetUserId();
            if (id is null)
            {
                ViewData["error"] = "Access denied";
                return await OnGetAsync();
            }
            bool success = await db.ChangeEmail(id, email);
            if (success is true)
            {
                ViewData["success"] = "Successfuly changed email";
            }
            else
            {
                ViewData["error"] = "Couldn't change email, something went wrong";
            }
            return await OnGetAsync();
        }
        public async Task<IActionResult> ChangePassword(string current, string password, string password2)
        {
            string id = GetUserId();
            if (id is null)
            {
                ViewData["error"] = "Access denied";
                return await OnGetAsync();
            }
            if (!await db.VerifyPasword(id, authManager.HashPassword(current)))
            {
                ViewData["error"] = "Wrong current password";
                return await OnGetAsync();
            }
            if (password is null || password.Length < 8)
            {
                ViewData["error"] = "Password shouldn't be less, than 8 characters";
                return await OnGetAsync();
            }
            if (password != password2)
            {
                ViewData["error"] = "Passwords aren't same, please try again";
                return await OnGetAsync();
            }
            bool success = await db.ChangePassword(id, authManager.HashPassword(password));
            if (success is true)
            {
                ViewData["success"] = "Successfuly changed password";
            }
            else
            {
                ViewData["error"] = "Couldn't change email, something went wrong";
            }
            return await OnGetAsync();
        }
    }
}
