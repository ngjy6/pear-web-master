using NTU_FYP_REBUILD_17.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.ViewModels
{
    public class CenterActivityViewModel
    {
        public List<Patient> Patients { get; set; }

        public List<CentreActivity> CentreActivities { get; set; }

        public List<ActivityPreferenceViewModel> ActivityPreferences { get; set; }
    }

    public class AutomatedScheduleTestingViewModel
	{
		public string DropDownListDate { get; set; }
	}

	public class ActivityPreferenceViewModel
    {
        public int PatientID { get; set; }
        public int CentreActivityId { get; set; }
        public int IsLike { get; set; }
        public int IsDislike { get; set; }
        public int? CentreactivityExclusionID { get; set; }
        public bool IsActivity { get; set; }
        public int DoctorRecomendation { get; set; }
    }
}