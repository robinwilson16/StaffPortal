using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StaffPortal.Data;
using StaffPortal.Models;

namespace StaffPortal.Pages.NewsArticles
{
    public class DetailsModel : PageModel
    {
        private readonly StaffPortal.Data.ApplicationDbContext _context;

        public DetailsModel(StaffPortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public News News { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            News = await _context.News.FirstOrDefaultAsync(m => m.NewsID == id);

            if (News == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
