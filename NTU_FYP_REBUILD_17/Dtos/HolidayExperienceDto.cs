using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.Dtos
{
    public class HolidayExperienceDto
    {
		[Key]
		public int holidayExpID { get; set; }
		public string holidayExp { get; set; }
		[StringLength(150)]
		public string country { get; set; }
		public SocialHistory SocialHistory { get; set; }
		public int socialHistoryID { get; set; }
		public int isApproved { get; set; }
		public int isDeleted { get; set; }
	}
}