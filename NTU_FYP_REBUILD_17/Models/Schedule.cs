using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class Schedule
    {
        [Key]
        public int scheduleID { get; set; }
        public virtual PatientAllocation PatientAllocation { get; set; }
        [ForeignKey("PatientAllocation")]
        public int patientAllocationID { get; set; }
        public virtual CentreActivity CentreActivity { get; set; }
        [ForeignKey("CentreActivity")]
        public int? centreActivityID { get; set; }
        public virtual Routine Routine { get; set; }
        [ForeignKey("Routine")]
        public int? routineID { get; set; }
        public string scheduleDesc { get; set; }
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }
        public TimeSpan timeStart { get; set; }
        public TimeSpan timeEnd { get; set; }
        public int isClash { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}