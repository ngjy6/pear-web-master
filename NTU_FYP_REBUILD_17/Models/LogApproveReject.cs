using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class LogApproveReject
    {
        [Key]
        public int approveRejectID { get; set; }
        public virtual User UserInit { get; set; }
        [ForeignKey("UserInit")]
        public int userIDInit { get; set; }
        public virtual User UserReceived { get; set; }
        [ForeignKey("UserReceived")]
        public int userIDReceived { get; set; }
        public virtual Log Log { get; set; }
        [ForeignKey("Log")]
        public int? logID { get; set; }
        public int approve { get; set; }
        public int reject { get; set; }
        public DateTime createDateTime { get; set; }
    }
}