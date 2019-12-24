using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class CareCentreHours
    {
        [Key]
        public int centreHoursID { get; set; }
        public virtual CareCentreAttributes CareCentreAttributes { get; set; }
        [ForeignKey("CareCentreAttributes")]
        public int centreID { get; set; }
        [StringLength(16)]
        public string centreWorkingDay { get; set; }
        public TimeSpan centreOpeningHours { get; set; }
        public TimeSpan centreClosingHours { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}