using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class LogAccount
    {
        [Key]
        public int logAccountID { get; set; }
        public int? userID { get; set; }
        public virtual Log Log { get; set; }
        [ForeignKey("Log")]
        public int? logID { get; set; }
        public string oldLogData { get; set; }
        public string logData { get; set; }
        public string logDesc { get; set; }
        public virtual LogCategory LogCategory { get; set; }
        [ForeignKey("LogCategory")]
        public int logCategoryID { get; set; }
        public string remarks { get; set; }
        [StringLength(256)]
        public string tableAffected { get; set; }
        public int? rowAffected { get; set; }
        public string columnAffected { get; set; }
        public string logOldValue { get; set; }
        public string logNewValue { get; set; }
        public string deleteReason { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}