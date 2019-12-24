using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class Prescription
    {
        [Key]
        public int prescriptionID { get; set; }
        public virtual PatientAllocation PatientAllocation { get; set; }
        [ForeignKey("PatientAllocation")]
        public int patientAllocationID { get; set; }
        public virtual List_Prescription List_Prescription { get; set; }
        [ForeignKey("List_Prescription")]
        public int drugNameID { get; set; }
        [StringLength(256)]
        public string dosage { get; set; }
        public int frequencyPerDay { get; set; }
        [StringLength(256)]
        public string instruction { get; set; }
        public DateTime startDate { get; set; }
        public DateTime? endDate { get; set; }
        public TimeSpan? timeStart { get; set; }
        public int beforeMeal { get; set; }
        public int afterMeal { get; set; }
        public string notes { get; set; }
        public int isChronic { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}