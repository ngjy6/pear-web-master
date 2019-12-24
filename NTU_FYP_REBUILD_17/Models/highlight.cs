using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class Highlight
    {
        [Key]
        public int highlightID { get; set; }
        public virtual PatientAllocation PatientAllocation { get; set; }
        [ForeignKey("PatientAllocation")]
        public int patientAllocationID { get; set; }
        public virtual HighlightType HighlightType { get; set; }
        [ForeignKey("HighlightType")]
        public int highlightTypeID { get; set; }
        [StringLength(256)]
        public string highlightData { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}