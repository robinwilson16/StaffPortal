using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StaffPortal.Data;
using StaffPortal.Models;

namespace StaffPortal.Pages.StaffDetails
{
    public class CreateModel : PageModel
    {
        private readonly StaffPortal.Data.ApplicationDbContext _context;

        public CreateModel(StaffPortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public StaffDetail StaffDetail { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.StaffDetail.Add(StaffDetail);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
