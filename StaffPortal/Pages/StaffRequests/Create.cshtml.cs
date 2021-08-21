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
using StaffPortal.Shared;

namespace StaffPortal.Pages.StaffRequests
{
    public class CreateModel : PageModel
    {
        private readonly StaffPortal.Data.ApplicationDbContext _context;

        public CreateModel(StaffPortal.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(string academicYear)
        {
            AcademicYearID = academicYear;
            DefaultAcademicYearID = await AcademicYearFunctions.GetDefaultAcademicYear(_context);

            string selectListDomain = null;

            selectListDomain = "ROOM";
            RoomData = await _context.SelectListData
                .FromSqlInterpolated($"EXEC SPR_SPO_SelectListData @AcademicYear={AcademicYearID}, @Domain={selectListDomain}")
                .ToListAsync();
            ViewData["RoomID"] = new SelectList(RoomData, "Code", "Description");

            selectListDomain = "CAMPUS";
            CampusData = await _context.SelectListData
                .FromSqlInterpolated($"EXEC SPR_SPO_SelectListData @AcademicYear={AcademicYearID}, @Domain={selectListDomain}")
                .ToListAsync();
            ViewData["CampusID"] = new SelectList(CampusData, "Code", "Description");

            selectListDomain = "TEAM";
            TeamData = await _context.SelectListData
                .FromSqlInterpolated($"EXEC SPR_SPO_SelectListData @AcademicYear={AcademicYearID}, @Domain={selectListDomain}")
                .ToListAsync();
            ViewData["TeamID"] = new SelectList(TeamData, "Code", "Description");

            selectListDomain = "EMPLOYEE_TYPE";
            EmployeeData = await _context.SelectListData
                .FromSqlInterpolated($"EXEC SPR_SPO_SelectListData @AcademicYear={AcademicYearID}, @Domain={selectListDomain}")
                .ToListAsync();
            ViewData["EmployeeTypeID"] = new SelectList(EmployeeData, "Code", "Description");

            return Page();
        }

        [BindProperty]
        public StaffRequest StaffRequest { get; set; }

        public string AcademicYearID { get; set; }
        public string DefaultAcademicYearID { get; set; }

        public IList<SelectListData> RoomData { get; set; }
        public IList<SelectListData> CampusData { get; set; }
        public IList<SelectListData> TeamData { get; set; }
        public IList<SelectListData> EmployeeData { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.StaffRequest_1.Add(StaffRequest);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
