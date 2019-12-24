using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.Dtos
{
    public class PatientListPageAPIViewModel
	{
		
	}

	public class PatientAlbumDoctorNoteDTOViewModel
	{
		[Required]
		public int patient_patientID { get; set; }
		public int patient_isDeleted { get; set; }
		public int patient_isApproved { get; set; }
		public int patient_updateBit { get; set; }
		public int patient_autoGame { get; set; }
		[StringLength(255)]
		public string patient_firstName { get; set; }
		[StringLength(255)]
		public string patient_lastName { get; set; }
		[StringLength(255)]
		public string patient_nric { get; set; }
		[StringLength(255)]
		public string patient_address { get; set; }
		[StringLength(20)]
		public string patient_officeNo { get; set; }
		[StringLength(20)]
		public string patient_handphoneNo { get; set; }
		[StringLength(255)]
		public string patient_guardianName { get; set; }
		[StringLength(20)]
		public string patient_guardianContactNo { get; set; }
		[StringLength(255)]
		public string patient_guardianEmail { get; set; }
		[StringLength(255)]
		public string patient_preferredName { get; set; }
		[StringLength(255)]
		public string patient_preferredLanguage { get; set; }
		[StringLength(10)]
		public char patient_gender { get; set; }
		public DateTime? patient_DOB { get; set; }
		public DateTime patient_createDateTime { get; set; }

		[Required]
		public int album_albumID { get; set; }
		public string album_albumPath { get; set; }
		public int album_albumCatID { get; set; }
		public int album_patientID { get; set; }
		public int album_isDeleted { get; set; }
		public int album_isApproved { get; set; }
		public DateTime album_createDateTime { get; set; }

		[Required]
		public int doctorNote_doctorNoteID { get; set; }
		public string doctorNote_note { get; set; }
		public DateTime doctorNote_createDateTime { get; set; }
		public int doctorNote_patientID { get; set; }
		public int doctorNote_isApproved { get; set; }
		public int doctorNote_isDeleted { get; set; }

	}


}