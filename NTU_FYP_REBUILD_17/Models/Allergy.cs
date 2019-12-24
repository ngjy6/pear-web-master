using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class Allergy
    {
        [Key]
        public int allergyID { get; set; }
        public virtual PatientAllocation PatientAllocation { get; set; }
        [ForeignKey("PatientAllocation")]
        public int patientAllocationID { get; set; }
        public virtual List_Allergy List_Allergy { get; set; }
        [ForeignKey("List_Allergy")]
        public int allergyListID { get; set; }
        [StringLength(256)]
        public string reaction { get; set; }
        public string notes { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}