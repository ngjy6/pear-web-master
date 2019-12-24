using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class MedicationLog
    {
        [Key]
        public int medicationLogID { get; set; }
        public virtual PatientAllocation PatientAllocation { get; set; }
        [ForeignKey("PatientAllocation")]
        public int patientAllocationID { get; set; }
        public virtual Prescription Prescription { get; set; }
        [ForeignKey("Prescription")]
        public int prescriptionID { get; set; }
        public DateTime dateForMedication { get; set; }
        public TimeSpan timeForMedication { get; set; }
        public virtual User User { get; set; }
        [ForeignKey("User")]
        public int? userID { get; set; }
        public DateTime? dateTaken { get; set; }
        public TimeSpan? timeTaken { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}