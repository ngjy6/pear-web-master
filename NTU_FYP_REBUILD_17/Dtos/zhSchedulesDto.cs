using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.Dtos
{
    public class zhSchedulesDto
	{
		[Required]
		public int scheduleID { get; set; }

		public Routine Routine { get; set; }
		public int? routineID { get; set; }
		public PatientAllocation PatientAllocation { get; set; }
		public int? patientAllocationID { get; set; }
		public int isDeleted { get; set; }
		public int isApproved { get; set; }
		public CentreActivity CentreActivity { get; set; }
		public int? centreActivityID { get; set; }
		public int isClash { get; set; }

		public string scheduleDesc { get; set; }

		public TimeSpan? timeStart { get; set; }
		public TimeSpan? timeEnd { get; set; }
		public DateTime? dateStart { get; set; }
		public DateTime? dateEnd { get; set; }
		public DateTime createDateTime { get; set; }


	}
}