using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class AttendanceLog
    {
        [Key]
        public int attendanceLogID { get; set; }
        public virtual PatientAllocation PatientAllocation { get; set; }
        [ForeignKey("PatientAllocation")]
        public int patientAllocationID { get; set; }
        public DateTime attendanceDate { get; set; }
        [StringLength(16)]
        public string dayOfWeek { get; set; }
        public TimeSpan? arrivalTime { get; set; }
        public TimeSpan? departureTime { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}