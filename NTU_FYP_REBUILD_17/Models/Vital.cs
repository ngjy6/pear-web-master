using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class Vital
    {
        [Key]
        public int vitalID { get; set; }
        public virtual PatientAllocation PatientAllocation { get; set; }
        [ForeignKey("PatientAllocation")]
        public int patientAllocationID { get; set; }
        public int afterMeal { get; set; }
        public double temperature { get; set; }
        [StringLength(16)]
        public string bloodPressure { get; set; }
        public int systolicBP { get; set; }
        public int diastolicBP { get; set; }
        public int heartRate { get; set; }
        public int spO2 { get; set; }
        public int bloodSugarlevel { get; set; }
        public double height { get; set; }
        public double weight { get; set; }
        public string notes { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
	}
}