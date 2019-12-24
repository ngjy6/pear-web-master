using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class PersonInCharge
    {
        [Key]
        public int personInChargeID { get; set; }
        public virtual User primaryUser { get; set; }
        [ForeignKey("primaryUser")]
        public int primaryUserID { get; set; }
        public virtual UserType primaryUserType { get; set; }
        [ForeignKey("primaryUserType")]
        public int primaryUserTypeID { get; set; }
        public virtual User tempUser { get; set; }
        [ForeignKey("tempUser")]
        public int tempUserID { get; set; }
        public virtual UserType tempUserType { get; set; }
        [ForeignKey("tempUserType")]
        public int tempUserTypeID { get; set; }
        public string reason { get; set; }
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }
        public TimeSpan timeStart { get; set; }
        public TimeSpan timeEnd { get; set; }
    }
}