using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.ViewModels
{
    public class PatientAlbumDoctorNoteDTO2ViewModel
	{
		public Patient Patient { get; set; }
		public AlbumPatient Album { get; set; }
		public DoctorNote DoctorNote { get; set; }
	}
}