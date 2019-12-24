using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class PatientAllocation
    {
        [Key]
        public int patientAllocationID { get; set; }
        public virtual Patient Patient { get; set; }
        [ForeignKey("Patient")]
        public int patientID { get; set; }
        public virtual User Doctor { get; set; }
        [ForeignKey("Doctor")]
        public int doctorID { get; set; }
        public virtual User tempDoctor { get; set; }
        [ForeignKey("tempDoctor")]
        public int? tempDoctorID { get; set; }
        public virtual User Gametherapist { get; set; }
        [ForeignKey("Gametherapist")]
        public int gametherapistID { get; set; }
        public virtual User Caregiver { get; set; }
        [ForeignKey("Caregiver")]
        public int caregiverID { get; set; }
        public virtual User tempCaregiver { get; set; }
        [ForeignKey("tempCaregiver")]
        public int? tempCaregiverID { get; set; }
        public virtual User Guardian { get; set; }
        [ForeignKey("Guardian")]
        public int? guardianID { get; set; }
        public virtual User Guardian2 { get; set; }
        [ForeignKey("Guardian2")]
        public int? guardian2ID { get; set; }
        public virtual User Supervisor { get; set; }
        [ForeignKey("Supervisor")]
        public int supervisorID { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}