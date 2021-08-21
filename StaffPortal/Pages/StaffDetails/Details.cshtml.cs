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
    public class DetailsModel : PageModel
    {
        private readonly StaffPortal.Data.ApplicationDbContext _context;

        public DetailsModel(StaffPortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public StaffDetail StaffDetail { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StaffDetail = await _context.StaffDetail.FirstOrDefaultAsync(m => m.StaffDetailID == id);

            if (StaffDetail == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
