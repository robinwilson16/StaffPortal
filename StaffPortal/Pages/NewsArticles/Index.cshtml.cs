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
    public class IndexModel : PageModel
    {
        private readonly StaffPortal.Data.ApplicationDbContext _context;

        public IndexModel(StaffPortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<News> News { get;set; }

        public async Task OnGetAsync(string newsType)
        {
            //News = await _context.News.ToListAsync();
            News = await _context.News
                .FromSqlInterpolated($"EXEC SPR_SPO_News @NewsType={newsType}")
                .ToListAsync();
        }

        public async Task<JsonResult> OnGetJsonAsync(string newsType)
        {
            //News = await _context.News.ToListAsync();
            News = await _context.News
                .FromSqlInterpolated($"EXEC SPR_SPO_News @NewsType={newsType}")
                .ToListAsync();

            return new JsonResult(News);
        }
    }
}
