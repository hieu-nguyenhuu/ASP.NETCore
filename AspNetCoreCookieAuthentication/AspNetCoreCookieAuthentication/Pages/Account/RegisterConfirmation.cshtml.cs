using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AspNetCoreCookieAuthentication.Data;
using System.Linq;

namespace AspNetCoreCookieAuthentication.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {

        private readonly ApplicationDbContext Db;

        public RegisterConfirmationModel(ApplicationDbContext Db)
        {
            this.Db = Db;
        }

        public string Email { get; set; }


        public async Task<IActionResult> OnGetAsync(string email)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            var user = Db.Users.Where(f => f.Email == email).FirstOrDefault();
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;

            return Page();
        }
    }
}