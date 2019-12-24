using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.ViewModels
{
    public class PatientListPageAPIViewModel
	{
		
	}

	public class PatientAlbumDoctorNoteViewModel
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
		public string patient_gender { get; set; }
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

	public class activityPref_centreActivityViewModel
	{
		[Key, Column(Order = 0)]
		public int activityPref_patientID { get; set; }
		[Key, Column(Order = 1)]
		public int activityPref_centreActivityID { get; set; }
		public int activityPref_isLike { get; set; }
		public int activityPref_isDislike { get; set; }
		public int activityPref_isNeutral { get; set; }
		public int activityPref_isApproved { get; set; }
		public int activityPref_doctorRecommendation { get; set; }
		public int activityPref_isDeleted { get; set; }
		public string activityPref_doctorRemarks { get; set; }

		[Required]
		public int centreActivity_centreActivityID { get; set; }
		[StringLength(255)]
		public string centreActivity_activityTitle { get; set; }
		[StringLength(255)]
		public string centreActivity_activityDesc { get; set; }
		public int centreActivity_isCompulsory { get; set; }
		public int centreActivity_isFixed { get; set; }
		public int centreActivity_isGroup { get; set; }
		public int centreActivity_interval { get; set; }
		public int centreActivity_minDuration { get; set; }
		public int centreActivity_maxDuration { get; set; }
		public int centreActivity_isDeleted { get; set; }
		public int centreActivity_isApproved { get; set; }
		public int centreActivity_minPeopleReq { get; set; }
		public DateTime centreActivity_createDateTime { get; set; }
	}


}