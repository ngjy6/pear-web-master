using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class HighlightThreshold
    {
        [Key]
        public int thresholdID { get; set; }
        public virtual HighlightType HighlightType { get; set; }
        [ForeignKey("HighlightType")]
        public int highlightTypeID { get; set; }
        [StringLength(256)]
        public string category { get; set; }
        public int minValue { get; set; }
        public int maxValue { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}