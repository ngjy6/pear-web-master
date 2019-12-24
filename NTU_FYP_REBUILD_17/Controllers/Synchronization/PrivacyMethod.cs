using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace NTU_FYP_REBUILD_17.Controllers.Synchronization
{
    public class PrivacyMethod
    {
        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();

        public PrivacyMethod()
        {
            _context = new ApplicationDbContext();
        }

        public int? getPatientAlcoholPrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? alcoholUseBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        alcoholUseBit = Convert.ToInt32(privacySettings.alcoholUseBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        alcoholUseBit = Convert.ToInt32(privacyUserRole.alcoholUseBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        alcoholUseBit = Convert.ToInt32(privacySettings.alcoholUseBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        alcoholUseBit = Convert.ToInt32(privacyUserRole.alcoholUseBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        alcoholUseBit = Convert.ToInt32(privacySettings.alcoholUseBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                alcoholUseBit = Convert.ToInt32(privacyUserRole.alcoholUseBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        alcoholUseBit = Convert.ToInt32(privacySettings.alcoholUseBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                alcoholUseBit = Convert.ToInt32(privacyUserRole.alcoholUseBit);
                            }
                        }
                    }
                    break;
            }
            return alcoholUseBit;
        }

        public int? getPatientCaffeinePrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? caffeineUseBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        caffeineUseBit = Convert.ToInt32(privacySettings.caffeineUseBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        caffeineUseBit = Convert.ToInt32(privacyUserRole.caffeineUseBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        caffeineUseBit = Convert.ToInt32(privacySettings.caffeineUseBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        caffeineUseBit = Convert.ToInt32(privacyUserRole.caffeineUseBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        caffeineUseBit = Convert.ToInt32(privacySettings.caffeineUseBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                caffeineUseBit = Convert.ToInt32(privacyUserRole.caffeineUseBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        caffeineUseBit = Convert.ToInt32(privacySettings.caffeineUseBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                caffeineUseBit = Convert.ToInt32(privacyUserRole.caffeineUseBit);
                            }
                        }
                    }
                    break;
            }
            return caffeineUseBit;
        }

        public int? getPatientDietPrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? dietBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        dietBit = Convert.ToInt32(privacySettings.dietBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        dietBit = Convert.ToInt32(privacyUserRole.dietBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        dietBit = Convert.ToInt32(privacySettings.dietBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        dietBit = Convert.ToInt32(privacyUserRole.dietBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        dietBit = Convert.ToInt32(privacySettings.dietBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                dietBit = Convert.ToInt32(privacyUserRole.dietBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        dietBit = Convert.ToInt32(privacySettings.dietBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                dietBit = Convert.ToInt32(privacyUserRole.dietBit);
                            }
                        }
                    }
                    break;
            }
            return dietBit;
        }

        public int? getPatientExercisePrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? exerciseBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        exerciseBit = Convert.ToInt32(privacySettings.exerciseBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        exerciseBit = Convert.ToInt32(privacyUserRole.exerciseBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        exerciseBit = Convert.ToInt32(privacySettings.exerciseBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        exerciseBit = Convert.ToInt32(privacyUserRole.exerciseBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        exerciseBit = Convert.ToInt32(privacySettings.exerciseBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                exerciseBit = Convert.ToInt32(privacyUserRole.exerciseBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        exerciseBit = Convert.ToInt32(privacySettings.exerciseBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                exerciseBit = Convert.ToInt32(privacyUserRole.exerciseBit);
                            }
                        }
                    }
                    break;
            }
            return exerciseBit;
        }

        public int? getPatientRetiredPrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? retiredBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        retiredBit = Convert.ToInt32(privacySettings.retiredBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        retiredBit = Convert.ToInt32(privacyUserRole.retiredBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        retiredBit = Convert.ToInt32(privacySettings.retiredBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        retiredBit = Convert.ToInt32(privacyUserRole.retiredBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        retiredBit = Convert.ToInt32(privacySettings.retiredBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                retiredBit = Convert.ToInt32(privacyUserRole.retiredBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        retiredBit = Convert.ToInt32(privacySettings.retiredBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                retiredBit = Convert.ToInt32(privacyUserRole.retiredBit);
                            }
                        }
                    }
                    break;
            }
            return retiredBit;
        }

        public int? getPatientLiveWithPrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? liveWithBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        liveWithBit = Convert.ToInt32(privacySettings.liveWithBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        liveWithBit = Convert.ToInt32(privacyUserRole.liveWithBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        liveWithBit = Convert.ToInt32(privacySettings.liveWithBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        liveWithBit = Convert.ToInt32(privacyUserRole.liveWithBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        liveWithBit = Convert.ToInt32(privacySettings.liveWithBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                liveWithBit = Convert.ToInt32(privacyUserRole.liveWithBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        liveWithBit = Convert.ToInt32(privacySettings.liveWithBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                liveWithBit = Convert.ToInt32(privacyUserRole.liveWithBit);
                            }
                        }
                    }
                    break;
            }
            return liveWithBit;
        }

        public int? getPatientTobaccoUsePrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? tobaccoUseBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        tobaccoUseBit = Convert.ToInt32(privacySettings.tobaccoUseBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        tobaccoUseBit = Convert.ToInt32(privacyUserRole.tobaccoUseBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        tobaccoUseBit = Convert.ToInt32(privacySettings.tobaccoUseBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        tobaccoUseBit = Convert.ToInt32(privacyUserRole.tobaccoUseBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        tobaccoUseBit = Convert.ToInt32(privacySettings.tobaccoUseBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                tobaccoUseBit = Convert.ToInt32(privacyUserRole.tobaccoUseBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        tobaccoUseBit = Convert.ToInt32(privacySettings.tobaccoUseBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                tobaccoUseBit = Convert.ToInt32(privacyUserRole.tobaccoUseBit);
                            }
                        }
                    }
                    break;
            }
            return tobaccoUseBit;
        }

        public int? getPatientPetPrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? petBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        petBit = Convert.ToInt32(privacySettings.petBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        petBit = Convert.ToInt32(privacyUserRole.petBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        petBit = Convert.ToInt32(privacySettings.petBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        petBit = Convert.ToInt32(privacyUserRole.petBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        petBit = Convert.ToInt32(privacySettings.petBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                petBit = Convert.ToInt32(privacyUserRole.petBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        petBit = Convert.ToInt32(privacySettings.petBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                petBit = Convert.ToInt32(privacyUserRole.petBit);
                            }
                        }
                    }
                    break;
            }
            return petBit;
        }

        public int? getPatientSecondhandSmokerPrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? secondhandSmokerBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        secondhandSmokerBit = Convert.ToInt32(privacySettings.secondhandSmokerBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        secondhandSmokerBit = Convert.ToInt32(privacyUserRole.secondhandSmokerBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        secondhandSmokerBit = Convert.ToInt32(privacySettings.secondhandSmokerBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        secondhandSmokerBit = Convert.ToInt32(privacyUserRole.secondhandSmokerBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        secondhandSmokerBit = Convert.ToInt32(privacySettings.secondhandSmokerBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                secondhandSmokerBit = Convert.ToInt32(privacyUserRole.secondhandSmokerBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        secondhandSmokerBit = Convert.ToInt32(privacySettings.secondhandSmokerBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                secondhandSmokerBit = Convert.ToInt32(privacyUserRole.secondhandSmokerBit);
                            }
                        }
                    }
                    break;
            }
            return secondhandSmokerBit;
        }

        public int? getPatientDislikePrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? dislikeBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        dislikeBit = Convert.ToInt32(privacySettings.dislikeBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        dislikeBit = Convert.ToInt32(privacyUserRole.dislikeBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        dislikeBit = Convert.ToInt32(privacySettings.dislikeBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        dislikeBit = Convert.ToInt32(privacyUserRole.dislikeBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        dislikeBit = Convert.ToInt32(privacySettings.dislikeBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                dislikeBit = Convert.ToInt32(privacyUserRole.dislikeBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        dislikeBit = Convert.ToInt32(privacySettings.dislikeBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                dislikeBit = Convert.ToInt32(privacyUserRole.dislikeBit);
                            }
                        }
                    }
                    break;
            }
            return dislikeBit;
        }

        public int? getPatientHabitPrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? habitBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        habitBit = Convert.ToInt32(privacySettings.habitBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        habitBit = Convert.ToInt32(privacyUserRole.habitBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        habitBit = Convert.ToInt32(privacySettings.habitBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        habitBit = Convert.ToInt32(privacyUserRole.habitBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        habitBit = Convert.ToInt32(privacySettings.habitBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                habitBit = Convert.ToInt32(privacyUserRole.habitBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        habitBit = Convert.ToInt32(privacySettings.habitBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                habitBit = Convert.ToInt32(privacyUserRole.habitBit);
                            }
                        }
                    }
                    break;
            }
            return habitBit;
        }

        public int? getPatientHobbyPrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? hobbyBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        hobbyBit = Convert.ToInt32(privacySettings.hobbyBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        hobbyBit = Convert.ToInt32(privacyUserRole.hobbyBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        hobbyBit = Convert.ToInt32(privacySettings.hobbyBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        hobbyBit = Convert.ToInt32(privacyUserRole.hobbyBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        hobbyBit = Convert.ToInt32(privacySettings.hobbyBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                hobbyBit = Convert.ToInt32(privacyUserRole.hobbyBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        hobbyBit = Convert.ToInt32(privacySettings.hobbyBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                hobbyBit = Convert.ToInt32(privacyUserRole.hobbyBit);
                            }
                        }
                    }
                    break;
            }
            return hobbyBit;
        }

        public int? getPatientHolidaExperiencePrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? holidayExperienceBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        holidayExperienceBit = Convert.ToInt32(privacySettings.holidayExperienceBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        holidayExperienceBit = Convert.ToInt32(privacyUserRole.holidayExperienceBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        holidayExperienceBit = Convert.ToInt32(privacySettings.holidayExperienceBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        holidayExperienceBit = Convert.ToInt32(privacyUserRole.holidayExperienceBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        holidayExperienceBit = Convert.ToInt32(privacySettings.holidayExperienceBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                holidayExperienceBit = Convert.ToInt32(privacyUserRole.holidayExperienceBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        holidayExperienceBit = Convert.ToInt32(privacySettings.holidayExperienceBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                holidayExperienceBit = Convert.ToInt32(privacyUserRole.holidayExperienceBit);
                            }
                        }
                    }
                    break;
            }
            return holidayExperienceBit;
        }

        public int? getPatientLikePrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? likeBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        likeBit = Convert.ToInt32(privacySettings.likeBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        likeBit = Convert.ToInt32(privacyUserRole.likeBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        likeBit = Convert.ToInt32(privacySettings.likeBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        likeBit = Convert.ToInt32(privacyUserRole.likeBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        likeBit = Convert.ToInt32(privacySettings.likeBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                likeBit = Convert.ToInt32(privacyUserRole.likeBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        likeBit = Convert.ToInt32(privacySettings.likeBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                likeBit = Convert.ToInt32(privacyUserRole.likeBit);
                            }
                        }
                    }
                    break;
            }
            return likeBit;
        }

        public int? getPatientLanguagePrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? languageBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        languageBit = Convert.ToInt32(privacySettings.languageBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        languageBit = Convert.ToInt32(privacyUserRole.languageBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        languageBit = Convert.ToInt32(privacySettings.languageBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        languageBit = Convert.ToInt32(privacyUserRole.languageBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        languageBit = Convert.ToInt32(privacySettings.languageBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                languageBit = Convert.ToInt32(privacyUserRole.languageBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        languageBit = Convert.ToInt32(privacySettings.languageBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                languageBit = Convert.ToInt32(privacyUserRole.languageBit);
                            }
                        }
                    }
                    break;
            }
            return languageBit;
        }

        public int? getPatientDrugUsePrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? drugUseBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        drugUseBit = Convert.ToInt32(privacySettings.drugUseBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        drugUseBit = Convert.ToInt32(privacyUserRole.drugUseBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        drugUseBit = Convert.ToInt32(privacySettings.drugUseBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        drugUseBit = Convert.ToInt32(privacyUserRole.drugUseBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        drugUseBit = Convert.ToInt32(privacySettings.drugUseBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                drugUseBit = Convert.ToInt32(privacyUserRole.drugUseBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        drugUseBit = Convert.ToInt32(privacySettings.drugUseBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                drugUseBit = Convert.ToInt32(privacyUserRole.drugUseBit);
                            }
                        }
                    }
                    break;
            }
            return drugUseBit;
        }

        public int? getPatientEducationPrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? educationBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        educationBit = Convert.ToInt32(privacySettings.educationBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        educationBit = Convert.ToInt32(privacyUserRole.educationBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        educationBit = Convert.ToInt32(privacySettings.educationBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        educationBit = Convert.ToInt32(privacyUserRole.educationBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        educationBit = Convert.ToInt32(privacySettings.educationBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                educationBit = Convert.ToInt32(privacyUserRole.educationBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        educationBit = Convert.ToInt32(privacySettings.educationBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                educationBit = Convert.ToInt32(privacyUserRole.educationBit);
                            }
                        }
                    }
                    break;
            }
            return educationBit;
        }

        public int? getPatientOccupationPrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? occupationBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        occupationBit = Convert.ToInt32(privacySettings.occupationBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        occupationBit = Convert.ToInt32(privacyUserRole.occupationBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        occupationBit = Convert.ToInt32(privacySettings.occupationBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        occupationBit = Convert.ToInt32(privacyUserRole.occupationBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        occupationBit = Convert.ToInt32(privacySettings.occupationBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                occupationBit = Convert.ToInt32(privacyUserRole.occupationBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        occupationBit = Convert.ToInt32(privacySettings.occupationBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                occupationBit = Convert.ToInt32(privacyUserRole.occupationBit);
                            }
                        }
                    }
                    break;
            }
            return occupationBit;
        }

        public int? getPatientReligionPrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? religionBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        religionBit = Convert.ToInt32(privacySettings.religionBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        religionBit = Convert.ToInt32(privacyUserRole.religionBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        religionBit = Convert.ToInt32(privacySettings.religionBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        religionBit = Convert.ToInt32(privacyUserRole.religionBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        religionBit = Convert.ToInt32(privacySettings.religionBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                religionBit = Convert.ToInt32(privacyUserRole.religionBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        religionBit = Convert.ToInt32(privacySettings.religionBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                religionBit = Convert.ToInt32(privacyUserRole.religionBit);
                            }
                        }
                    }
                    break;
            }
            return religionBit;
        }

        public int? getPatientSexuallyActivePrivacy(int userID, PatientAllocation patientAllocation)
        {
            int? sexuallyActiveBit = null;
            int patientAllocationID = patientAllocation.patientAllocationID;

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == user.userTypeID));
            string userTypeName = userType.userTypeName;

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isDeleted != 1));

            switch (userTypeName)
            {
                case "Caregiver":
                    // main caregiver
                    if (userID == patientAllocation.caregiverID)
                    {
                        sexuallyActiveBit = Convert.ToInt32(privacySettings.sexuallyActiveBit.Substring(3, 1));
                    }

                    // temp caregiver
                    else if (userID == patientAllocation.tempCaregiverID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempCaregiver" && x.isDeleted != 1));
                        sexuallyActiveBit = Convert.ToInt32(privacyUserRole.sexuallyActiveBit);
                    }
                    break;

                case "Doctor":
                    // main doctor
                    if (userID == patientAllocation.doctorID)
                    {
                        sexuallyActiveBit = Convert.ToInt32(privacySettings.sexuallyActiveBit.Substring(2, 1));
                    }

                    // temp doctor
                    else if (userID == patientAllocation.tempDoctorID)
                    {
                        PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "tempDoctor" && x.isDeleted != 1));
                        sexuallyActiveBit = Convert.ToInt32(privacyUserRole.sexuallyActiveBit);
                    }
                    break;

                case "Game Therapist":
                    // main game therapist
                    if (userID == patientAllocation.gametherapistID)
                    {
                        sexuallyActiveBit = Convert.ToInt32(privacySettings.sexuallyActiveBit.Substring(1, 1));
                    }

                    // game therapist deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "gameDeputy" && x.isDeleted != 1));
                                sexuallyActiveBit = Convert.ToInt32(privacyUserRole.sexuallyActiveBit);
                            }
                        }
                    }
                    break;

                case "Supervisor":
                    // main supervisor
                    if (userID == patientAllocation.supervisorID)
                    {
                        sexuallyActiveBit = Convert.ToInt32(privacySettings.sexuallyActiveBit.Substring(4, 1));
                    }

                    // supervisor deputy
                    else
                    {
                        DateTime now = DateTime.Now;

                        PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userType.userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
                        // if there's deputy
                        if (pic != null)
                        {
                            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
                            if (userID == tempUser.userID)
                            {
                                PrivacyLevel privacyUserRole = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "supervisorDeputy" && x.isDeleted != 1));
                                sexuallyActiveBit = Convert.ToInt32(privacyUserRole.sexuallyActiveBit);
                            }
                        }
                    }
                    break;
            }
            return sexuallyActiveBit;
        }
    }
}