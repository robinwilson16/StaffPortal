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

namespace StaffPortal.Pages.StaffDetails
{
    public class EditModel : PageModel
    {
        private readonly StaffPortal.Data.ApplicationDbContext _context;

        public EditModel(StaffPortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(StaffDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffDetailExists(StaffDetail.StaffDetailID))
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

        private bool StaffDetailExists(int id)
        {
            return _context.StaffDetail.Any(e => e.StaffDetailID == id);
        }
    }
}
