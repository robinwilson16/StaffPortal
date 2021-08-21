using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffPortal.Models
{
    public class News
    {
        public int NewsID { get; set; }

		public int CategoryID { get; set; }
		public int TypeID { get; set; }
		public int TypeSort { get; set; }
		public string Title { get; set; }
		public DateTime? DisplayFromDate { get; set; }
		public DateTime? DisplayToDate { get; set; }
		public string IntroText { get; set; }
		public string MainText { get; set; }
		public string NewsType { get; set; }
		public string NewsClass { get; set; }
		public int IconID { get; set; }
		public string IconPath { get; set; }
		public int NumComments { get; set; }
		public int NumViews { get; set; }
		public string ArticleLink { get; set; }
		public DateTime? CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public string CreatedBySurname { get; set; }
		public string CreatedByForename { get; set; }
		public string CreatedByName { get; set; }
		public string CreatedByEmail { get; set; }
	}
}
