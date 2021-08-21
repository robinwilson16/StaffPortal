using Microsoft.EntityFrameworkCore;
using StaffPortal.Data;
using StaffPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StaffPortal.Shared
{
    public class Identity
    {
        public static string GetUserName(ClaimsPrincipal user, ApplicationDbContext _context)
        {
            string userName = null;
            try
            {
                userName = user.Identity.Name.Split('\\').Last();
            }
            catch
            {
                userName = "UNKNOWN";
            }

            return userName;
        }
        public static StaffMember StaffMember { get; set; }

        public static async Task<string> GetFullName(string academicYear, string username, ApplicationDbContext _context)
        {
            string CurrentAcademicYear = await AcademicYearFunctions.GetAcademicYear(academicYear, _context);

            StaffMember = (await _context.StaffMember
                .FromSqlInterpolated($"EXEC SPR_ETG_GetStaffMember @AcademicYear={CurrentAcademicYear}, @UserName={username}")
                .ToListAsync())
                .FirstOrDefault();

            if (StaffMember != null)
            {
                return StaffMember.StaffDetails;
            }
            else
            {
                return username;
            }


        }

        public static string GetGreeting()
        {
            string greeting = "";
            int currentHour = DateTime.Now.Hour;

            if (currentHour < 12)
            {
                greeting = "Good Morning";
            }
            else if (currentHour < 17)
            {
                greeting = "Good Afternoon";
            }
            else
            {
                greeting = "Good Evening";
            }

            return greeting;
        }
    }
}
