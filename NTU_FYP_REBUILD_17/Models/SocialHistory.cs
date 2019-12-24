using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
	public class SocialHistory
	{
		[Key]
		public int socialHistoryID { get; set; }
        public virtual PatientAllocation PatientAllocation { get; set; }
        [ForeignKey("PatientAllocation")]
        public int patientAllocationID { get; set; }
        public int sexuallyActive { get; set; }
        public int secondhandSmoker { get; set; }
        public int alcoholUse { get; set; }
        public int caffeineUse { get; set; }
        public int tobaccoUse { get; set; }
        public int drugUse { get; set; }
        public int exercise { get; set; }
        public int retired { get; set; }

        public virtual List_Diet List_Diet { get; set; }
        [ForeignKey("List_Diet")]
        public int dietID { get; set; }
        public virtual List_Religion List_Religion { get; set; }
        [ForeignKey("List_Religion")]
        public int religionID { get; set; }
        public virtual List_Pet List_Pet { get; set; }
        [ForeignKey("List_Pet")]
        public int petID { get; set; }
        public virtual List_Occupation List_Occupation { get; set; }
        [ForeignKey("List_Occupation")]
        public int occupationID { get; set; }
        public virtual List_Education List_Education { get; set; }
        [ForeignKey("List_Education")]
        public int educationID { get; set; }
        public virtual List_LiveWith List_LiveWith { get; set; }
        [ForeignKey("List_LiveWith")]
        public int liveWithID { get; set; }
        public int isApproved { get; set; }
		public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
	}


}