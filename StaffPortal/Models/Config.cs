using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StaffPortal.Models
{
    public class Config
    {
        [StringLength(5)]
        public string AcademicYear { get; set; }
    }
}
