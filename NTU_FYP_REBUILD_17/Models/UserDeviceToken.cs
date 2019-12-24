using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class UserDeviceToken
    {
        [Key]
        public int UserDeviceTokenID { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("ApplicationUser")]
        public string uid { get; set; }
        public string deviceToken { get; set; }
        public DateTime createDateTime { get; set; }
        public DateTime lastAccessedDate { get; set; }
    }
}