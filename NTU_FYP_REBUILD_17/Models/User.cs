using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class User
    {
        [Key]
        public int userID { get; set; }
        public virtual ApplicationUser AspNetUsers { get; set; }
        [ForeignKey("AspNetUsers")]
        [StringLength(128)]
        public string aspNetID { get; set; }
        public string token { get; set; }
        public DateTime lastPasswordChanged { get; set; }
        public DateTime? loginTimeStamp { get; set; }
    }
}