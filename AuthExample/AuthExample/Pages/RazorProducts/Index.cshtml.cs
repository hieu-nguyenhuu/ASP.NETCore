using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AuthExample.Data;
using Microsoft.AspNetCore.Authorization;

namespace AuthExample.Pages.RazorProducts
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly AuthExample.Data.ApplicationDbContext _context;

        public IndexModel(AuthExample.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Products != null)
            {
                Product = await _context.Products.ToListAsync();
            }
        }
    }
}
