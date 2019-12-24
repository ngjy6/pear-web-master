using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class DoctorNoteDto
    {
        [Required]
        public int doctorNoteID { get; set; }
        public string note { get; set; }
        public DateTime createDateTime { get; set; }
        public Patient Patient { get; set; }
        public int patientID { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
    }
}