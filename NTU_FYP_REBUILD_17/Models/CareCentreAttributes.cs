using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class CareCentreAttributes
    {
        [Key]
        public int centreID { get; set; }
        public string centreName { get; set; }
        public int centreCountryID { get; set; }
        public string centreAddress { get; set; }
        [StringLength(16)]
        public string centrePostalCode { get; set; }
        [StringLength(16)]
        public string centreContactNo { get; set; }
        public string centreEmail { get; set; }
        public int devicesAvailable { get; set; }
        public int isDeleted { get; set; }
        //[Column(TypeName = "datetime2")]
        public DateTime createDateTime { get; set; }
    }
}