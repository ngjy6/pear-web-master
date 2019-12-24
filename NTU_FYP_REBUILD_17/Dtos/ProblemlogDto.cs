using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NTU_FYP_REBUILD_17.Dtos
{
    public class problemlogDto 
    {
		[Required]
		public int problemLogID { get; set; }
		public int userID { get; set; }
		public int patientID { get; set; }
		public int isApproved { get; set; }
		public int isDeleted { get; set; }
		public DateTime createdDateTime { get; set; }
		[StringLength(150)]
		public string category { get; set; }
		public string notes { get; set; }

	}
}