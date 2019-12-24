using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class Log
    {
        [Key]
        public int logID { get; set; }
        public string oldLogData { get; set; }
        public string logData { get; set; }
        public string logDesc { get; set; }
        public virtual LogCategory LogCategory { get; set; }
        [ForeignKey("LogCategory")]
        public int logCategoryID { get; set; }
        public virtual PatientAllocation PatientAllocation { get; set; }
        [ForeignKey("PatientAllocation")]
        public int? patientAllocationID { get; set; }
        public virtual User UserInit { get; set; }
        [ForeignKey("UserInit")]
        public int? userIDInit { get; set; }
        public virtual User UserApproved { get; set; }
        [ForeignKey("UserApproved")]
        public int? userIDApproved { get; set; }
        public virtual UserType UserType { get; set; }
        [ForeignKey("UserType")]
        public int? intendedUserTypeID { get; set; }
        public string additionalInfo { get; set; }
        public string remarks { get; set; }
        [StringLength(256)]
        public string tableAffected { get; set; }
        public string columnAffected { get; set; }
        public string logOldValue { get; set; }
        public string logNewValue { get; set; }
        public int? rowAffected { get; set; }
        public int supNotified { get; set; }
        public int approved { get; set; }
        public int reject { get; set; }
        public int userNotified { get; set; }
        public string rejectReason { get; set; }
        public string deleteReason { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}