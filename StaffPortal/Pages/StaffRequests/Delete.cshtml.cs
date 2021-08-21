using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StaffPortal.Data;
using StaffPortal.Models;

namespace StaffPortal.Pages.StaffRequests
{
    public class DeleteModel : PageModel
    {
        private readonly StaffPortal.Data.ApplicationDbContext _context;

        public DeleteModel(StaffPortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public StaffRequest StaffRequest { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StaffRequest = await _context.StaffRequest_1.FirstOrDefaultAsync(m => m.StaffRequestID == id);

            if (StaffRequest == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StaffRequest = await _context.StaffRequest_1.FindAsync(id);

            if (StaffRequest != null)
            {
                _context.StaffRequest_1.Remove(StaffRequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
