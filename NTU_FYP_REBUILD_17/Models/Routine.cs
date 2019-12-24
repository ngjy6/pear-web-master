using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class Routine
    {
        [Key]
        public int routineID { get; set; }
        public virtual PatientAllocation PatientAllocation { get; set; }
        [ForeignKey("PatientAllocation")]
        public int patientAllocationID { get; set; }
        public virtual CentreActivity CentreActivity { get; set; }
        [ForeignKey("CentreActivity")]
        public int? centreActivityID { get; set; }
        [StringLength(256)]
        public string eventName { get; set; }
        public string notes { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        [StringLength(16)]
        public string day { get; set; }
        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }
        public int includeInSchedule { get; set; }
        public string reasonForExclude { get; set; }
        public string concerningIssues { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}