using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NTU_FYP_REBUILD_17.Dtos
{
    public class AllergyDto 
    {
		[Required]
		public int allergyID { get; set; }
		[StringLength(250)]
		public string allergy { get; set; }
		[StringLength(250)]
		public string reaction { get; set; }
		public string notes { get; set; }
		public int patientID { get; set; }
		public int isApproved { get; set; }
		public int isDeleted { get; set; }
		public DateTime createDateTime { get; set; }

	}
}