using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class PrescriptionDto
	{
        [Required]
        public int prescriptionID { get; set; }
        public int patientID { get; set; }
        public int frequencyPerDay { get; set; }
        public int beforeMeal { get; set; }
        public int afterMeal { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }

        public string drugName { get; set; }
        public string dosage { get; set; }
        public string instruction { get; set; }
        public string notes { get; set; }

        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }
}