using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace NTU_FYP_REBUILD_17.Models
{
    public class ActivityPreference
    {
        [Key]
        public int activityPreferencesID { get; set; }
        public virtual PatientAllocation PatientAllocation { get; set; }
        [ForeignKey("PatientAllocation")]
        public int patientAllocationID { get; set; }
        public virtual CentreActivity CentreActivity { get; set; }
        [ForeignKey("CentreActivity")]
        public int centreActivityID { get; set; }
        public int isLike { get; set; }
        public int isDislike { get; set; }
        public int isNeutral { get; set; }
        public int doctorRecommendation { get; set; }
        public virtual User Doctor { get; set; }
        [ForeignKey("Doctor")]
        public int? doctorID { get; set; }
        public string doctorRemarks { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}