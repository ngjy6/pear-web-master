using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class AlbumPatient
    {
        [Key]
        public int albumID { get; set; }
        public string albumPath { get; set; }
        public virtual AlbumCategory AlbumCategory { get; set; }
        [ForeignKey("AlbumCategory")]
        public int albumCatID { get; set; }
        public virtual PatientAllocation PatientAllocation { get; set; }
        [ForeignKey("PatientAllocation")]
        public int patientAllocationID { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}