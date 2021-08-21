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
    public class IndexModel : PageModel
    {
        private readonly StaffPortal.Data.ApplicationDbContext _context;

        public IndexModel(StaffPortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<StaffRequest> StaffRequest { get;set; }

        public async Task OnGetAsync()
        {
            StaffRequest = await _context.StaffRequest_1.ToListAsync();
        }
    }
}
