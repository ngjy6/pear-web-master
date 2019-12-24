using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NTU_FYP_REBUILD_17.Models;
namespace NTU_FYP_REBUILD_17.Dtos
{
    public class ActivityPreferenceDto
	{
		[Key]
		public int patientID { get; set; }
		//public Patient Patient { get; set; }
		[Required]
		public int centreActivityID { get; set; }
		public CentreActivity CentreActivity { get; set; }
		public int isLike { get; set; }
		public int isDislike { get; set; }
		public int isNeutral { get; set; }
		public int isApproved { get; set; }
		public int doctorRecommendation { get; set; }
		public int isDeleted { get; set; }
		public string doctorRemarks { get; set; }

	}
}