using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NTU_FYP_REBUILD_17.Models;
namespace NTU_FYP_REBUILD_17.Dtos
{
    public class SocialHistoryDto
	{
		[Required]
		public int socialHistoryID { get; set; }

		public Patient Patient { get; set; }
		public int patientID { get; set; }
		public int sexuallyActive { get; set; }
		public int secondhandSmoker { get; set; }
		public int alcoholUse { get; set; }
		public int caffeineUse { get; set; }
		public int tobaccoUse { get; set; }
		public int drugUse { get; set; }
		public int exercise { get; set; }
		public int isApproved { get; set; }
		public int isDeleted { get; set; }

		[StringLength(50)]
		public string diet { get; set; }
		[StringLength(150)]
		public string religion { get; set; }
		[StringLength(150)]
		public string pet { get; set; }
		[StringLength(255)]
		public string occupation { get; set; }
		[StringLength(255)]
		public string education { get; set; }
		[StringLength(255)]
		public string liveWith { get; set; }

		public DateTime createDateTime { get; set; }


	}
}