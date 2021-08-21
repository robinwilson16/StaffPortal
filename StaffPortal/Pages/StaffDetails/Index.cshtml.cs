using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StaffPortal.Data;
using StaffPortal.Models;

namespace StaffPortal.Pages.StaffDetails
{
    public class IndexModel : PageModel
    {
        private readonly StaffPortal.Data.ApplicationDbContext _context;

        public IndexModel(StaffPortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<StaffDetail> StaffDetail { get;set; }

        public async Task OnGetAsync(string search)
        {
            //StaffDetail = await _context.StaffDetail.ToListAsync();
            StaffDetail = await _context.StaffDetail
                .FromSqlInterpolated($"EXEC SPR_SPO_StaffDetails @Search={search}")
                .ToListAsync();
        }

        public async Task<JsonResult> OnGetJsonAsync(string search)
        {
            StaffDetail = await _context.StaffDetail
                .FromSqlInterpolated($"EXEC SPR_SPO_StaffDetails @Search={search}")
                .ToListAsync();

            return new JsonResult(StaffDetail);
        }
    }
}
