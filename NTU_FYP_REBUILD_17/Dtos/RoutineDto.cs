using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class RoutineDto
    {
		[Required]
		public int routineID { get; set; }

		public int patientID { get; set; }
		public int includeInSchedule { get; set; }
		public int everyNum { get; set; }
		public int isApproved { get; set; }
		public int? centreActivityID { get; set; }
		public int isDeleted { get; set; }

		public DateTime startDate { get; set; }
		public DateTime endDate { get; set; }
		public TimeSpan? startTime { get; set; }
		public TimeSpan? endTime { get; set; }

		[StringLength(255)]
		public string eventName { get; set; }
		public string notes { get; set; }
		public string everyLabel { get; set; }

		public DateTime createDateTime { get; set; }
	}
}