using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StaffPortal.Data;
using StaffPortal.Models;

namespace StaffPortal.Pages.StaffRequests
{
    public class EditModel : PageModel
    {
        private readonly StaffPortal.Data.ApplicationDbContext _context;

        public EditModel(StaffPortal.Data.ApplicationDbContext context)
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(StaffRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffRequestExists(StaffRequest.StaffRequestID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool StaffRequestExists(int id)
        {
            return _context.StaffRequest_1.Any(e => e.StaffRequestID == id);
        }
    }
}
