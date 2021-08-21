using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StaffPortal.Models
{
    public class StaffRequest
    {
        public int StaffRequestID { get; set; }

        [Display(Name = "New Staff Surname*")]
        [StringLength(50)]
        [Required]
        public string Surname { get; set; }

        [Display(Name = "New Staff Forename*")]
        [StringLength(50)]
        [Required]
        public string Forename { get; set; }

        [Display(Name = "Room No")]
        [StringLength(20)]
        public string RoomNumber { get; set; }

        [Display(Name = "Campus*")]
        [StringLength(50)]
        [Required]
        public string Campus { get; set; }

        [Display(Name = "Department*")]
        [StringLength(50)]
        [Required]
        public string Department { get; set; }

        [Display(Name = "Employee Type*")]
        [StringLength(50)]
        [Required]
        public string EmployeeType { get; set; }

        [Display(Name = "Agency Name (if applicable)")]
        [StringLength(200)]
        public string AgencyName { get; set; }

        [Display(Name = "Phone Extension")]
        public int? PhoneExtension { get; set; }

        [Display(Name = "Expected Job Title*")]
        [StringLength(200)]
        [Required]
        public string JobTitle { get; set; }
    }
}
