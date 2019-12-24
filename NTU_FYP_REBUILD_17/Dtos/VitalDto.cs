using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.Dtos
{
    public class VitalDto
    {
		[Required]
		public int vitalID { get; set; }

		public Patient Patient { get; set; }
		public int patientID { get; set; }
		public int afterMeal { get; set; }
		public int isDeleted { get; set; }
		public int isApproved { get; set; }

		public double temperature { get; set; }
		public double height { get; set; }
		public double weight { get; set; }

		[StringLength(10)]
		public string bloodPressure { get; set; }
		public string notes { get; set; }

		public DateTime createDateTime { get; set; }
	}
}