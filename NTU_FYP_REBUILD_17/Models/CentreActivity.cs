using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class CentreActivity
    {
        [Key]
        public int centreActivityID { get; set; }
        [StringLength(256)]
        public string activityTitle { get; set; }
        [StringLength(8)]
        public string shortTitle { get; set; }
        [StringLength(256)]
        public string activityDesc { get; set; }
        public int isCompulsory { get; set; }
        public int isFixed { get; set; }
        public int isGroup { get; set; }
        public int minDuration { get; set; }
        public int maxDuration { get; set; }
        public int minPeopleReq { get; set; }
        public DateTime activityStartDate { get; set; }
        public DateTime? activityEndDate { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}