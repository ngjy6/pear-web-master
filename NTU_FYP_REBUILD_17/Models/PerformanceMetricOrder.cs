using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class PerformanceMetricOrder
    {
        public virtual PerformanceMetricName PerformanceMetricName { get; set; }
        [Key]
        [Column(Order = 0)]
        [ForeignKey("PerformanceMetricName")]
        public int pmnID { get; set; }

        public virtual Game Game { get; set; }
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Game")]
        public int gameID { get; set; }

        public int metricOrder { get; set; }
        public DateTime createDateTime { get; set; }
    }
}