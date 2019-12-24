using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class PerformanceMetricName
    {
        [Key]
        public int pmnID { get; set; }
        public string performanceMetricName { get; set; }
        public string performanceMetricDetail { get; set; }
        public virtual Game Game { get; set; }
        [ForeignKey("Game")]
        public int gameID { get; set; }
        public DateTime createDateTime { get; set; }
    }
}