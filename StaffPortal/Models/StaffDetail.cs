using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffPortal.Models
{
    public class StaffDetail
    {
		public int StaffDetailID { get; set; }
		public string StaffRef { get; set; }
		public string Surname { get; set; }
		public string Forename { get; set; }
		public string Email { get; set; }
		public string Email2 { get; set; }
		public string Tel { get; set; }
		public string Ext { get; set; }
		public string Mobile { get; set; }
		public byte[] Photo { get; set; }
		public string SiteCode { get; set; }
		public string SiteName { get; set; }
		public string RoomCode { get; set; }
		public string RoomName { get; set; }
		public int? StaffTypeCode { get; set; }
		public string StaffTypeName { get; set; }
		public string CollegeLevelCode { get; set; }
		public string CollegeLevelName { get; set; }
		public string CollegeLevels { get; set; }
		public int? ContractNo { get; set; }
		public string ContractRef { get; set; }
		public string MainJobPostTitle { get; set; }
		public decimal? MainJobFTE { get; set; }
		public DateTime? MainJobStartDate { get; set; }
		public DateTime? MainJobLeaveDate { get; set; }
		public bool? MainJobPostActive { get; set; }
		public decimal? MainJobContractHours { get; set; }
		public decimal? MainJobContactHours { get; set; }
		public string PostTitles { get; set; }
		public decimal? TotalFTE { get; set; }
		public DateTime? EarliestStartDate { get; set; }
		public DateTime? LatestLeaveDate { get; set; }
		public decimal? TotalContractHours { get; set; }
		public decimal? TotalContactHours { get; set; }
		public string EmployeeStatus { get; set; }
		public int? EmailForwarding { get; set; }
    }
}
