using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class AlbumDto
    {
        [Required]
        public int albumID { get; set; }
        public string albumPath { get; set; }
        public AlbumCategory AlbumCategory { get; set; }
        [ForeignKey("AlbumCategory")]
        public int albumCatID { get; set; }
        public Patient Patient { get; set; }
        public int patientID { get; set; }
        public int isDeleted { get; set; }
        public int isApproved { get; set; }
        public DateTime createDateTime { get; set; }
    }
}