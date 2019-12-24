using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.Dtos
{
    public class PatientAllocationDto
    {
        public int patientAllocationID { get; set; }
        public virtual Patient Patient { get; set; }
        public int patientID { get; set; }
        public virtual User Doctor { get; set; }
        public int doctorID { get; set; }
        public virtual User tempDoctor { get; set; }
        public int? tempDoctorID { get; set; }
        public virtual User Gametherapist { get; set; }
        public int gametherapistID { get; set; }
        public virtual User Caregiver { get; set; }
        public int caregiverID { get; set; }
        public virtual User tempCaregiver { get; set; }
        public int? tempCaregiverID { get; set; }
        public virtual User Guardian { get; set; }
        public int? guardianID { get; set; }
        public virtual User Guardian2 { get; set; }
        public int? guardian2ID { get; set; }
        public virtual User Supervisor { get; set; }
        public int supervisorID { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}