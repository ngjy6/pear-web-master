using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class ActivityAvailability
    {
        //[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Key]
        public int activityAvailabilityID { get; set; }
        public virtual CentreActivity CentreActivity { get; set; }
        [ForeignKey("CentreActivity")]
        public int centreActivityID { get; set; }
        [StringLength(16)]
        public string day { get; set; }
        public TimeSpan timeStart { get; set; }
        public TimeSpan timeEnd { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}
 