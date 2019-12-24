using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;
using System.IO;
using System.Data.Entity;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using ClosedXML.Excel;
using System.Web.Script.Serialization;
using System.Data.Entity.Core.Objects;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace NTU_FYP_REBUILD_17.Controllers
{
    public class SupervisorController : Controller
    {
        private ApplicationDbContext _context;
        private App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        private Synchronization.ListMethod addListMethod = new Synchronization.ListMethod();
        private Controllers.Synchronization.ScheduleMethod scheduler = new Controllers.Synchronization.ScheduleMethod();
        private Controllers.Synchronization.PatientMethod patientMethod = new Controllers.Synchronization.PatientMethod();
        private Controllers.Synchronization.CentreActivityMethod centreActivityMethod = new Controllers.Synchronization.CentreActivityMethod();
        private Controllers.Synchronization.AccountMethod account = new Controllers.Synchronization.AccountMethod();
        private Controllers.Synchronization.AlbumMethod albumMethod = new Synchronization.AlbumMethod();

        public SupervisorController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult Help()
        {
            if (User.IsInRole(RoleName.isSupervisor))
            {
                return View("Help");
            }

            else
            {
                return View("_LoginPage");
            }
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult Index()
        {

            var patientHighlights = (from p in _context.Patients
                                     join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                     //join hl in _context.Highlight on pa.patientAllocationID equals hl.patientAllocationID
                                     where p.isDeleted != 1 && p.isApproved == 1
                                     where pa.isDeleted != 1 && pa.isApproved == 1
                                     where pa.isApproved == 1
                                     where p.endDate > DateTime.Today || p.endDate == null
                                     where p.isActive == 1
                                     //where hl.isApproved == 1
                                     //where hl.isDeleted !=1

                                     select new PatientHighlight
                                     {
                                         patient = p,

                                         prscpList = _context.Highlight.Where(x => x.patientAllocationID == pa.patientAllocationID
                                        && x.isApproved == 1 && x.isDeleted != 1 && x.startDate <= DateTime.Today && x.endDate >= DateTime.Today && x.highlightTypeID == 1).ToList(),

                                         allergyList = _context.Highlight.Where(x => x.patientAllocationID == pa.patientAllocationID
                                         && x.isApproved == 1 && x.isDeleted != 1 && x.startDate <= DateTime.Today && x.endDate >= DateTime.Today && x.highlightTypeID == 2).ToList(),

                                         problemList = _context.Highlight.Where(x => x.patientAllocationID == pa.patientAllocationID
                                         && x.isApproved == 1 && x.isDeleted != 1 && x.startDate <= DateTime.Today && x.endDate >= DateTime.Today && x.highlightTypeID == 4).ToList(),

                                         activityExclusionList = _context.Highlight.Where(x => x.patientAllocationID == pa.patientAllocationID
                                         && x.isApproved == 1 && x.isDeleted != 1 && x.startDate <= DateTime.Today && x.endDate >= DateTime.Today && x.highlightTypeID == 5).ToList(),

                                         vitalList = _context.Highlight.Where(x => x.patientAllocationID == pa.patientAllocationID
                                         && x.isApproved == 1 && x.isDeleted != 1 && x.startDate <= DateTime.Today && x.endDate >= DateTime.Today && x.highlightTypeID == 3).ToList(),

                                     }).ToList();




            foreach (var item in patientHighlights)
            {
                //allergy
                var allergy = new List<JObject>();

                foreach (var x in item.allergyList)
                {

                    if (x.highlightData != null)
                    {
                        JObject jObject = (JObject)JsonConvert.DeserializeObject(x.highlightData);
                        allergy.Add(jObject);
                    }
                }
                item.allergyJObjectList = allergy;


                //problem log
                var problem = new List<JObject>();
                foreach (var x in item.problemList)
                {
                    if (x.highlightData != null)
                    {
                        JObject pJObject = (JObject)JsonConvert.DeserializeObject(x.highlightData);
                        problem.Add(pJObject);
                    }
                }
                item.problemJObject = problem;

                //prescription
                var prescription = new List<JObject>();
                foreach (var x in item.prscpList)
                {
                    if (x.highlightData != null)
                    {
                        JObject psJObject = (JObject)JsonConvert.DeserializeObject(x.highlightData);
                        prescription.Add(psJObject);
                    }
                }
                item.pscpJObjectList = prescription;

                //activityExclusion
                var actEx = new List<JObject>();
                foreach (var x in item.activityExclusionList)
                {
                    if (x.highlightData != null)
                    {
                        JObject aeJObject = (JObject)JsonConvert.DeserializeObject(x.highlightData);
                        actEx.Add(aeJObject);
                    }
                }
                item.actExJObjectList = actEx;

                //vital
                var vital = new List<JObject>();
                foreach (var x in item.vitalList)
                {
                    if (x.highlightData != null)
                    {
                        JObject vJObject = (JObject)JsonConvert.DeserializeObject(x.highlightData);
                        vital.Add(vJObject);
                        //string vitalItems = vJObject["Vital"].ToString();
                        //var vitems = vitalItems.Split(',');
                        //vital.Add(vitems);
                    }
                }
                item.vitalJObjectList = vital;


                //count
                item.problemCount = item.allergyList.Count() + item.problemList.Count() + item.prscpList.Count() + item.activityExclusionList.Count() + item.vitalList.Count();


            }





            var viewModel = new HighlightViewModel()
            {
                patientHighlightsList = patientHighlights,
            };

            return View(viewModel);
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult Dashboard()
        {
            //List<Patient> patientList = _context.Patients.Where(x => x.isApproved == 1 && x.isDeleted != 1).ToList();

            //List<ProblemLog> logList = _context.ProblemLogs.Where(x => x.isApproved == 1 && x.isDeleted != 1).ToList();

            //List<Vital> vitalList = _context.Vitals.Where(x => x.isDeleted != 1).ToList();

            //List<AlbumPatient> albumList = _context.AlbumPatient.Where(x => x.isApproved == 1 && x.isDeleted != 1).ToList();


            //var viewModel = new DashboardViewModel()
            //{
            //    patientList = patientList,
            //    logList = logList,
            //    vitalList = vitalList,
            //    albumList = albumList,

            //};

            //return View(viewModel);

            var patientDetail = (from p in _context.Patients
                                 join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                 join a in _context.AlbumPatient on pa.patientAllocationID equals a.patientAllocationID
                                 join u in _context.Users on pa.caregiverID equals u.userID

                                 where p.isDeleted != 1
                                 where pa.isDeleted != 1
                                 where a.isDeleted != 1
                                 where p.isApproved == 1
                                 where pa.isApproved == 1
                                 where a.isApproved == 1
                                 select new PatientDetail
                                 {
                                     patient = p,
                                     albumPath = a.albumPath,
                                     caregiver = u.firstName + " " + u.lastName,

                                     vitalList = _context.Vitals.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.patientAllocationID == pa.patientAllocationID && DbFunctions.TruncateTime(x.createDateTime) == DbFunctions.TruncateTime(DateTime.Today)).ToList(),

                                     logList = _context.ProblemLogs.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1 && DbFunctions.TruncateTime(x.createdDateTime) == DbFunctions.TruncateTime(DateTime.Today)).ToList(),




                                 }).ToList();

            var viewModel = new ManageSupervisorsViewModel()
            {
                ListOfPatient = patientDetail,
            };

            return View(viewModel);
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManagePatient()
        {


            var patientDetail = (from p in _context.Patients
                                 join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                 join a in _context.AlbumPatient on pa.patientAllocationID equals a.patientAllocationID
                                 join u in _context.Users on pa.caregiverID equals u.userID

                                 where p.isDeleted != 1 && p.isApproved == 1
                                 where pa.isDeleted != 1 && pa.isApproved == 1
                                 where a.isDeleted != 1 && a.isApproved == 1
                                 where a.albumCatID == 1
                                 where p.isActive == 1
                                 where p.endDate > DateTime.Today || p.endDate == null
                                 select new PatientDetail
                                 {
                                     patient = p,
                                     albumPath = a.albumPath,
                                     //albumPath = _context.AlbumPatient.OrderByDescending(x => x.isApproved == 1 && x.isDeleted != 1 && x.patientAllocationID == pa.patientAllocationID).FirstOrDefault().albumPath,
                                     caregiver = u.firstName + " " + u.lastName,

                                     //allergy = _context.Allergies.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).ToList(),

                                     //allergyName = _context.ListAllergy.Where(y => y.list_allergyID ==
                                     //(_context.Allergies.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault().allergyListID)
                                     //&& y.isDeleted != 1).FirstOrDefault().value,

                                     patientAllergies = (from al in _context.Allergies
                                                         where al.patientAllocationID == pa.patientAllocationID
                                                         where al.isDeleted != 1
                                                         where al.isApproved == 1
                                                         select new PatientAllergy
                                                         {
                                                             allergy = al,
                                                             allergyName = _context.ListAllergy.Where(x => x.list_allergyID == al.allergyListID && x.isDeleted != 1).FirstOrDefault().value,
                                                         }).ToList(),

                                     vitalBefore = _context.Vitals.Where(x => x.patientAllocationID == pa.patientAllocationID && x.afterMeal == 0 && x.isApproved == 1 && x.isDeleted != 1).ToList(),

                                     vitalAfter = _context.Vitals.Where(x => x.patientAllocationID == pa.patientAllocationID && x.afterMeal == 1 && x.isApproved == 1 && x.isDeleted != 1).ToList(),


                                     socialHistory = _context.SocialHistories.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault(),

                                     liveWith = _context.ListLiveWiths.Where(x => x.list_liveWithID == (_context.SocialHistories.Where(
                                                y => y.patientAllocationID == pa.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault()).liveWithID).FirstOrDefault().value,

                                     occupation = _context.ListOccupations.Where(x => x.list_occupationID == (_context.SocialHistories.Where(
                                                y => y.patientAllocationID == pa.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault()).occupationID).FirstOrDefault().value,

                                     education = _context.ListEducations.Where(x => x.list_educationID == (_context.SocialHistories.Where(
                                                y => y.patientAllocationID == pa.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault()).educationID).FirstOrDefault().value,

                                     diet = _context.ListDiets.Where(x => x.list_dietID == (_context.SocialHistories.Where(
                                                y => y.patientAllocationID == pa.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault()).dietID).FirstOrDefault().value,

                                     pet = _context.ListPets.Where(x => x.list_petID == (_context.SocialHistories.Where(
                                               y => y.patientAllocationID == pa.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault()).petID).FirstOrDefault().value,

                                     religion = _context.ListReligions.Where(x => x.list_religionID == (_context.SocialHistories.Where(
                                                 y => y.patientAllocationID == pa.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault()).religionID).FirstOrDefault().value,

                                     medHistoryList = _context.MedicalHistory.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).ToList(),

                                     likesList = _context.Likes.Where(y => y.socialHistoryID == _context.SocialHistories.Where(z => z.patientAllocationID == pa.patientAllocationID && z.isApproved == 1 && z.isDeleted != 1).FirstOrDefault().socialHistoryID
                                    && y.isApproved == 1 && y.isDeleted != 1).ToList(),


                                     dislikesList = _context.Dislikes.Where(y => y.socialHistoryID == _context.SocialHistories.Where(z => z.patientAllocationID == pa.patientAllocationID && z.isApproved == 1 && z.isDeleted != 1).FirstOrDefault().socialHistoryID
                                     && y.isApproved == 1 && y.isDeleted != 1).ToList(),


                                     prescriptions = _context.Prescriptions.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).ToList(),


                                     ListOfDementiaCondition = (from dt in _context.DementiaTypes
                                                                join pad in _context.PatientAssignedDementias on dt.dementiaID equals pad.dementiaID
                                                                where pad.patientAllocationID == pa.patientAllocationID
                                                                where pad.isApproved == 1 && pad.isDeleted != 1
                                                                where dt.isApproved == 1 && dt.isDeleted != 1

                                                                select new DementiaList
                                                                {
                                                                    dementiaNames = dt.dementiaType.ToString()
                                                                }).ToList(),

                                     // dementiaCondition = _context.DementiaTypes.Where(x => x.dementiaID ==
                                     //(_context.PatientAssignedDementias.Where(y => y.patientAllocationID == pa.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault()).dementiaID
                                     //&& x.isApproved == 1 & x.isDeleted != 1).FirstOrDefault(),

                                     routines = _context.Routines.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1 && DbFunctions.TruncateTime(x.endDate) >= DbFunctions.TruncateTime(DateTime.Today)).ToList(),

                                     patientGuardian = _context.PatientGuardian.Where(x => x.patientGuardianID == p.patientGuardianID && x.isDeleted != 1).FirstOrDefault(),

                                     preferredLanguage = _context.ListLanguages.Where(x => x.list_languageID == (_context.Languages.Where(y => y.isApproved == 1 && y.isDeleted != 1 && y.languageID == p.preferredLanguageID).FirstOrDefault().languageListID) && x.isDeleted != 1).FirstOrDefault().value,

                                     guardianRelationship = _context.ListRelationships.Where(y => y.list_relationshipID == (_context.PatientGuardian.Where(x => x.patientGuardianID == p.patientGuardianID && x.isDeleted != 1).FirstOrDefault().guardianRelationshipID) && y.isDeleted != 1).FirstOrDefault().value,

                                     guardianRelationship2 = _context.ListRelationships.Where(y => y.list_relationshipID == (_context.PatientGuardian.Where(x => x.patientGuardianID == p.patientGuardianID && x.isDeleted != 1).FirstOrDefault().guardian2RelationshipID) && y.isDeleted != 1).FirstOrDefault().value,

                                     Lmobility = _context.ListMobility.Where(x => x.list_mobilityID == (_context.Mobility.Where(y => y.patientAllocationID == pa.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1).OrderByDescending(z=>z.createdDateTime).FirstOrDefault().mobilityListID)).FirstOrDefault().value,


                                 }).ToList();

            var likesEnum = _context.ListLikes.Where(x => x.isDeleted != 1).ToList();

            var dislikesEnum = _context.ListDislikes.Where(x => x.isDeleted != 1).ToList();

            var drugEnum = _context.ListPrescriptions.Where(x => x.isDeleted != 1).ToList();

            var patientinactiveDetail = (from p in _context.Patients
                                         join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                         join a in _context.AlbumPatient on pa.patientAllocationID equals a.patientAllocationID
                                         join u in _context.Users on pa.caregiverID equals u.userID

                                         where p.isDeleted != 1
                                         where pa.isDeleted != 1
                                         where a.isDeleted != 1
                                         where a.albumCatID == 1
                                         where p.isApproved == 1
                                         where p.isActive == 0
                                         where pa.isApproved == 1
                                         where a.isApproved == 1
                                         where p.endDate > DateTime.Today || p.endDate == null
                                         select new PatientDetail
                                         {
                                             patient = p,
                                             albumPath = a.albumPath,
                                             caregiver = u.firstName + " " + u.lastName,
                                         }).ToList();



            var viewModel = new ManageSupervisorsViewModel()
            {
                ListOfPatient = patientDetail,
                likesEnum = likesEnum,
                dislikesEnum = dislikesEnum,
                ListOfinactivePatient = patientinactiveDetail,
            };

            return View(viewModel);
        }





        // GET: Only Supervisor can access this method
        //[Authorize(Roles = RoleName.isSupervisor)]
        //public ActionResult ManageLog()
        //{


        //    //var patients = _context.Patients.Where( x => x.isApproved == 1 && x.isDeleted != 1 ).ToList();
        //    //var patientAllocations = _context.PatientAllocations.Where(x => x.isApproved == 1 && x.isDeleted != 1).ToList();

        //    var patientDetail = (from p in _context.Patients
        //                         join pa in _context.PatientAllocations on p.patientID equals pa.patientID
        //                         join a in _context.AlbumPatient on pa.patientAllocationID equals a.patientAllocationID
        //                         join u in _context.Users on pa.caregiverID equals u.userID

        //                         where p.isDeleted != 1
        //                         where pa.isDeleted != 1
        //                         where a.isDeleted != 1
        //                         where p.isApproved == 1
        //                         where pa.isApproved == 1
        //                         where a.isApproved == 1
        //                         where p.isActive == 1
        //                         select new PatientDetail
        //                         {
        //                             patient = p,
        //                             albumPath = a.albumPath,
        //                             caregiver = u.firstName + " " + u.lastName,
        //                             allergy = _context.Allergies.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).ToList(),



        //                         }).ToList();


        //    var viewModel = new ManageSupervisorsViewModel()
        //    {
        //        ListOfPatient = patientDetail,

        //    };

        //    return View(viewModel);
        //}

        
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult AddPatient()
        {
            var caregiverList = (from u in _context.Users
                                 join ut in _context.UserTypes on u.userTypeID equals ut.userTypeID
                                 where u.isApproved == 1 && u.isDeleted != 1
                                 where ut.isDeleted != 1
                                 where ut.userTypeName == "Caregiver" || ut.userTypeName == "Supervisor"
                                 select new UserViewModel
                                 {
                                     userID = u.userID,
                                     userFullname = u.lastName + " " + u.firstName,
                                 }).ToList();
            var doctorList = (from u in _context.Users
                              join ut in _context.UserTypes on u.userTypeID equals ut.userTypeID
                              where u.isApproved == 1 && u.isDeleted != 1
                              where ut.isDeleted != 1
                              where ut.userTypeName == "Doctor"
                              select new UserViewModel
                              {
                                  userID = u.userID,
                                  userFullname = u.lastName + " " + u.firstName,
                              }).ToList();

            var gametherapistList = (from u in _context.Users
                                     join ut in _context.UserTypes on u.userTypeID equals ut.userTypeID
                                     where u.isApproved == 1 && u.isDeleted != 1
                                     where ut.isDeleted != 1
                                     where ut.userTypeName == "Game Therapist"
                                     select new UserViewModel
                                     {
                                         userID = u.userID,
                                         userFullname = u.lastName + " " + u.firstName,
                                     }).ToList();

            var listLanguage = _context.ListLanguages.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Language lang = new List_Language();
            lang.value = "Others";
            lang.list_languageID = -1;
            listLanguage.Add(lang);


            var liveWithList = _context.ListLiveWiths.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_LiveWith live = new List_LiveWith();
            live.value = "Others";
            live.list_liveWithID = -1;
            liveWithList.Add(live);


            var religionList = _context.ListReligions.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Religion reg = new List_Religion();
            reg.value = "Others";
            reg.list_religionID = -1;
            religionList.Add(reg);

            var occupationList = _context.ListOccupations.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Occupation occ = new List_Occupation();
            occ.value = "Others";
            occ.list_occupationID = -1;
            occupationList.Add(occ);

            var petList = _context.ListPets.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Pet pet = new List_Pet();
            pet.value = "Others";
            pet.list_petID = -1;
            petList.Add(pet);

            var educationList = _context.ListEducations.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Education ed = new List_Education();
            ed.value = "Others";
            ed.list_educationID = -1;
            educationList.Add(ed);


            var dietList = _context.ListDiets.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Diet diet = new List_Diet();
            diet.value = "Others";
            diet.list_dietID = -1;
            dietList.Add(diet);

            var dementiaList = _context.DementiaTypes.Where(x => x.isApproved == 1 && x.isDeleted != 1).ToList();
            //DementiaType dL = new DementiaType();
            //dL.dementiaType = "Others";
            //dL.dementiaID = -1;
            //dementiaList.Add(dL);

            var relationshipList = _context.ListRelationships.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Relationship rs = new List_Relationship();
            rs.value = "Others";
            rs.list_relationshipID = -1;
            relationshipList.Add(rs);

            var mobilityList = _context.ListMobility.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Mobility ml = new List_Mobility();
            ml.value = "Others";
            ml.list_mobilityID = -1;
            mobilityList.Add(ml);



            var routines = _context.Routines.ToList();

            var listOfAllergies = _context.ListAllergy.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Allergy listAllergy = new List_Allergy();
            listAllergy.list_allergyID = -1;
            listAllergy.value = "Others";
            listOfAllergies.Add(listAllergy);

            var viewModel = new ManageSupervisorsViewModel()
            {
                ListOfLanguages = listLanguage,
                caregiverList = caregiverList,
                doctorList = doctorList,
                gametherapistList = gametherapistList,
                liveWithList = liveWithList,
                religionList = religionList,
                occupationList = occupationList,
                petList = petList,
                educationList = educationList,
                dietList = dietList,
                dementiaList = dementiaList,
                routines = routines,
                relationshipList = relationshipList,
                mobilityList = mobilityList,
                listOfAllergies = listOfAllergies,
            };

            return View(viewModel);
        }




        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult AddPatientMethod(ManageSupervisorsViewModel viewModel)
        {

            //int supervisorID = Int32.Parse(Session["userID"].ToString());
            String errorMsg = "Failed to create a new patient.";
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            //Personal Information (Patient Information)
            string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
            string logDescList = _context.LogCategories.Where(x => x.logCategoryID == 19 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

            

            //Guardian 
            if (viewModel.patient.nric != null && viewModel.patientGuardian.guardianNRIC != null)
            {
               int patientGuardianID = patientMethod.addPatientGuardian(supervisorID, viewModel.patientGuardian.guardianName, viewModel.patientGuardian.guardianNRIC, viewModel.patientGuardian.guardianRelationshipID, viewModel.inputRS,
                       viewModel.patientGuardian.guardianContactNo, viewModel.patientGuardian.guardianEmail, viewModel.patientGuardian.guardianName2, viewModel.patientGuardian.guardianNRIC2, viewModel.patientGuardian.guardian2RelationshipID,
                       viewModel.input2RS, viewModel.patientGuardian.guardianContactNo2, viewModel.patientGuardian.guardianEmail2, 1);

                if (patientGuardianID != -1)
                {
                    int patientID = patientMethod.addPatient(supervisorID, patientGuardianID, viewModel.patient.nric, viewModel.patient.firstName, viewModel.patient.lastName, viewModel.patient.preferredName, viewModel.patient.handphoneNo,  viewModel.patient.homeNo,   viewModel.patient.preferredLanguageID, viewModel.inputLanguage,
                        viewModel.patient.address, viewModel.patient.tempAddress, viewModel.patient.gender, viewModel.patient.DOB, viewModel.patient.startDate, viewModel.patient.endDate, viewModel.patient.isRespiteCare, 1);


                    


                    //Patient Allocation 
                    if (patientID != -1)
                    {
                        int patientAllocationID = patientMethod.addPatientAllocation(supervisorID, patientID, viewModel.assignedDoctor, viewModel.assignedCaregiver, viewModel.assignedGametherapist, supervisorID, 1);

                        

                        //Album Patient
                        if (patientAllocationID != -1)
                        {
                            patientMethod.addDefaultProfileImage(supervisorID, patientAllocationID, 1);


                            ///////////////////////////////////////////////
                            ///Dementia Condition
                            ///

                            var listOfDementiaID = viewModel.listOfDementiaID;

                            for (var i = 0; i < listOfDementiaID.Length; i++)
                            {
                                patientMethod.addPatientAssignedDementia(supervisorID, patientAllocationID, Convert.ToInt32(listOfDementiaID[i]), 1);

                            }


                            //Medical History
                            var medDetails = viewModel.medDetails;
                            var medNotes = viewModel.medNotes;
                            var infoSource = viewModel.medSourceDoc;
                            string[] EstDate = Request.Form.GetValues("medicalEstimatedDate");


                            for (var i = 0; i < medDetails.Length; i++)
                            {

                                if (!medDetails[i].Equals(""))
                                {
                                    DateTime medicalEstimatedDate = DateTime.ParseExact(EstDate[i], "dd/mm/yyyy", null);
                                    patientMethod.addMedicalHistory(supervisorID, patientAllocationID, infoSource[i], medDetails[i], medNotes[i], medicalEstimatedDate, 1);

                                }
                            }

                            //Mobility
                            patientMethod.addMobility(supervisorID, patientAllocationID, viewModel.mobility.mobilityListID, viewModel.inputMobility, 1);


                            
                            //Social History 
                            int alcoholUse = viewModel.socialHistory.alcoholUse;
                            int caffeineUse = viewModel.socialHistory.caffeineUse;
                            int drugUse = viewModel.socialHistory.drugUse;
                            int exercise = viewModel.socialHistory.exercise;
                            int retired = viewModel.socialHistory.retired;
                            int tobaccoUse = viewModel.socialHistory.tobaccoUse;
                            int secondhandSmoker = viewModel.socialHistory.secondhandSmoker;
                            int sexuallyActive = viewModel.socialHistory.sexuallyActive;
                            int occupationID = viewModel.socialHistory.occupationID;
                            string occupationName = viewModel.inputOccupation;

                            int dietListID = viewModel.socialHistory.dietID;
                            string dietName = viewModel.inputDiet;

                            int educationListID = viewModel.socialHistory.educationID;
                            string educationName = viewModel.inputEducation;

                            int liveWithListID = viewModel.socialHistory.liveWithID;
                            string liveWithName = viewModel.inputLiveWith;

                            int petListID = viewModel.socialHistory.petID;
                            string petName = viewModel.inputPet;

                            int religionListID = viewModel.socialHistory.religionID;
                            string religionName = viewModel.inputReligion;


                            int socialHistoryID = patientMethod.addSocialHistory(supervisorID, patientAllocationID, alcoholUse, caffeineUse, drugUse, exercise, retired, tobaccoUse, secondhandSmoker, sexuallyActive, dietListID, dietName, educationListID, educationName, liveWithListID, liveWithName, petListID, petName, religionListID, religionName, occupationID, occupationName, 1);



                            //Privacy Settings
                            patientMethod.addDefaultPrivacySettings(socialHistoryID, patientAllocationID, supervisorID, 1);


                            //Allergy
                            var allergies = viewModel.allergiesInput;
                            string[] others = Request.Form.GetValues("allergyOther");
                            var react = viewModel.allergyReactInput;
                            var notes = viewModel.allergyNotesInput;


                            for (var i = 0; i < allergies.Length; i++)
                            {
                                patientMethod.addAllergy(supervisorID, patientAllocationID, Int32.Parse(allergies[i]), others[i], react[i], notes[i], 1);


                            }


                            //CentreActivity
                            patientMethod.addDefaultActivityPreferences(supervisorID, patientAllocationID, 1);

                            TempData["success"] = "Successfully created a new patient on " + DateTime.Now;

                        }


                    }

                }

                scheduler.generateWeeklySchedule(false, false);

            }
            else
            {
                TempData["error"] = errorMsg;
                return RedirectToAction("AddPatient", "Supervisor");
            }

            return RedirectToAction("ManagePatient", "Supervisor");
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult DeletePatient(string id)
        {
            //int supervisorID = Int32.Parse(Session["userID"].ToString());
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());


            int ID = Int32.Parse(id);
            int patientAllocationID = _context.PatientAllocations.Where(x => x.patientID == ID).SingleOrDefault().patientAllocationID;

            //var activity = _context.CentreActivities.Single(x => x.centreActivityID == ID);
            var patient = _context.Patients.Single(x => x.patientID == ID && x.isApproved == 1 && x.isDeleted != 1);
            var oldLogData = new JavaScriptSerializer().Serialize(patient);

            if (patient != null)
            {
                patient.isDeleted = 1;
                //ViewBag.Error = "Successfully removed a patient on " + DateTime.Now;
                _context.SaveChanges();
                TempData["success"] = "Successfully deleted a patient on " + DateTime.Now;

                // Note: the patientID is equal 0 as it does not affect any patient
                // shortcutMethod.addLogToDB(string? oldLogData, string logData, string logDesc, int logCategoryID, int? patientID, int userIDInit, int? userIDApproved, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                //shortcutMethod.addLogToDB(null, "", "Delete Patient info for Care Center", 4, patientAllocationID, supervisorID, supervisorID, null, null, null, "patient", "ALL", "", "", 0, 0, 1, "");

                var newLogData = new JavaScriptSerializer().Serialize(patient);

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocationID, supervisorID, supervisorID, null, null, null, "patient", "isDeleted", null, null, patient.patientID, 1, 0, null);

            }

            return RedirectToAction("ManagePatient", "Supervisor");
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditPatient(string id)
        {
            int ID = Int32.Parse(id);

            var patient = _context.Patients.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            var pa = _context.PatientAllocations.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();

            //var dementiaCondition = _context.DementiaTypes.Where(x => x.dementiaID ==
            //                     (_context.PatientAssignedDementias.Where(y => y.patientAllocationID == pa.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault()).dementiaID
            //                     && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            //var dementiaList = _context.DementiaTypes.Where(x => x.isApproved == 1 && x.isDeleted != 1).ToList();
            //var listOfDementiaID = _context.PatientAssignedDementias.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).ToList();

            //DementiaType dL = new DementiaType();
            //dL.dementiaType = "Others";
            //dL.dementiaID = -1;
            //dementiaList.Add(dL);

            var listLanguage = _context.ListLanguages.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Language lang = new List_Language();
            lang.value = "Others";
            lang.list_languageID = -1;
            listLanguage.Add(lang);

            var otherLanguage = _context.ListLanguages.Where(x => x.list_languageID == (_context.Languages.Where(y => y.languageID == patient.preferredLanguageID && y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault().languageListID) && x.isDeleted != 1).FirstOrDefault();

            var patientGuardian = _context.PatientGuardian.Where(x => patient.patientGuardianID == x.patientGuardianID && x.isDeleted != 1).SingleOrDefault();

            var preferredLanguage = _context.ListLanguages.Where(x => x.list_languageID == (_context.Languages.Where(y => y.isApproved == 1 && y.isDeleted != 1 && y.languageID == patient.preferredLanguageID).FirstOrDefault().languageListID) && x.isDeleted != 1).FirstOrDefault();

            var relationshipList = _context.ListRelationships.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Relationship rs = new List_Relationship();
            rs.value = "Others";
            rs.list_relationshipID = -1;
            relationshipList.Add(rs);

            var otherRS = _context.ListRelationships.Where(x => x.list_relationshipID == (_context.PatientGuardian.Where(y => y.patientGuardianID == patientGuardian.patientGuardianID && y.isDeleted != 1).FirstOrDefault().guardianRelationshipID) && x.isDeleted != 1).FirstOrDefault();

            var otherRS2 = _context.ListRelationships.Where(x => x.list_relationshipID == (_context.PatientGuardian.Where(y => y.patientGuardianID == patientGuardian.patientGuardianID && y.isDeleted != 1).FirstOrDefault().guardian2RelationshipID) && x.isDeleted != 1).FirstOrDefault();



            var viewModel = new ManageSupervisorsViewModel()
            {
                ListOfLanguages = listLanguage,
                patient = patient,
                //dementiaCondition = dementiaCondition,
                //ListOfAssignedDementia = listOfDementiaID,
                //dementiaList = dementiaList,
                patientGuardian = patientGuardian,
                preferredLanguage = preferredLanguage,
                relationshipList = relationshipList,
                otherLanguage = otherLanguage,
                otherRS = otherRS,
                otherRS2 = otherRS2,

            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditGenInfoMethod(ManageSupervisorsViewModel item)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            Patient patient = _context.Patients.Where(x => x.patientID == item.patient.patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();


            var pa = _context.PatientAllocations.Where(x => x.patientID == item.patient.patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            string logDescList = _context.LogCategories.Where(x => x.logCategoryID == 19 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
            int mobVal = Convert.ToInt32(item.inputMobilityID);


            //Social History 
            var socialHistory = _context.SocialHistories.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();



            var mobility = _context.Mobility.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).OrderByDescending(z=>z.createdDateTime).FirstOrDefault();

            if (mobility != null)
            {
                patientMethod.updateMobility(supervisorID, pa.patientAllocationID, mobVal, item.inputMobility, 1);

                
            }


            var oldLogData = new JavaScriptSerializer().Serialize(socialHistory);


            if (socialHistory != null)
            {


                int liveWithID = Convert.ToInt32(item.inputLiveWithID);
                int dietID = Convert.ToInt32(item.inputDietID);
                int regID = Convert.ToInt32(item.inputReligionID);
                int petID = Convert.ToInt32(item.inputPetID);
                int occID = Convert.ToInt32(item.inputOccupationID);
                int eduID = Convert.ToInt32(item.inputEducationID);

                

                patientMethod.updateSocialHistory(supervisorID, pa.patientAllocationID, item.socialHistory.alcoholUse, item.socialHistory.caffeineUse, item.socialHistory.drugUse, item.socialHistory.exercise,
                item.socialHistory.retired, item.socialHistory.tobaccoUse, item.socialHistory.secondhandSmoker, item.socialHistory.sexuallyActive, dietID, item.inputDiet,
                eduID, item.inputEducation, liveWithID, item.inputLiveWith, petID, item.inputPet, regID, item.inputReligion, occID, item.inputOccupation, 1);



            }
            else
            {
                TempData["error"] = "Failed to save changes.";

            }

            return RedirectToAction("EditGenInfo", "Supervisor", new { id = item.patient.patientID });

        }



        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditPatientMethod(ManageSupervisorsViewModel item)
        {
            Boolean flag;
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            String errorMsg = "Failed to save changes.";
            //int supervisorID = Int32.Parse(Session["userID"].ToString());

            Patient patient = _context.Patients.SingleOrDefault((x => x.patientID == item.patient.patientID && x.isApproved == 1 && x.isDeleted != 1));

            int patientAllocationID = _context.PatientAllocations.Where(y => y.patientID == item.patient.patientID && y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault().patientAllocationID;

            PatientAllocation pa = _context.PatientAllocations.Where(x => x.patientID == item.patient.patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();


            //TO BE DONE:
            //PatientAssignedDementia pad = _context.PatientAssignedDementias.SingleOrDefault(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1);

            PatientGuardian patientGuardian = _context.PatientGuardian.Where(x => x.patientGuardianID == patient.patientGuardianID && x.isDeleted != 1).SingleOrDefault();

            Language language = _context.Languages.Where(x => x.languageID == patient.preferredLanguageID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            string columnAffected = "";
            string logDesc = logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
            string logDescList = _context.LogCategories.Where(x => x.logCategoryID == 19 && x.isDeleted != 1).SingleOrDefault().logCategoryName;


            // Serialize using javascript will have a format something like this { "": "", "": "" } - sample only
            var oldLogData = new JavaScriptSerializer().Serialize(patient);

            if (patient != null)
            {
                flag = true;

                if (item.patient.nric == null || !shortcutMethod.checkNric(item.patient.nric))
                {
                    flag = false;
                    errorMsg += "<br/> Invalid NRIC!";
                }




                ////Guardian Information

                if (item.patientGuardian.guardianNRIC == null || !shortcutMethod.checkNric(item.patientGuardian.guardianNRIC))
                {
                    flag = false;
                    errorMsg += "<br/> Invalid Guardian NRIC!";
                }

                if (item.patientGuardian.guardianNRIC2 != null && !shortcutMethod.checkNric(item.patientGuardian.guardianNRIC2))
                {
                    flag = false;
                    errorMsg += "<br/> Invalid Guardian NRIC!";
                }

                if (flag)
                {
                    if (patient.firstName != item.patient.firstName)
                    {
                        patient.firstName = item.patient.firstName;
                        columnAffected = columnAffected + "firstName,";
                    }

                    if (patient.lastName != item.patient.lastName)
                    {
                        patient.lastName = item.patient.lastName;
                        columnAffected = columnAffected + "lastName,";

                    }

                    if (patient.nric != item.patient.nric)
                    {
                        patient.nric = item.patient.nric;
                        columnAffected = columnAffected + "nric,";

                    }

                    if (patient.address != item.patient.address)
                    {
                        patient.address = item.patient.address;
                        columnAffected = columnAffected + "address,";

                    }

                    if (patient.tempAddress != item.patient.tempAddress)
                    {
                        patient.tempAddress = item.patient.tempAddress;
                        columnAffected = columnAffected + "tempAddress,";

                    }

                    if (patient.handphoneNo != item.patient.handphoneNo)
                    {
                        patient.handphoneNo = item.patient.handphoneNo;
                        columnAffected = columnAffected + "handphoneNo,";
                     }

                    if (patient.homeNo != item.patient.homeNo)
                    {
                        patient.homeNo = item.patient.homeNo;
                        columnAffected = columnAffected + "homeNo,";
                    }


                    if (patient.gender != item.patient.gender)
                    {
                        patient.gender = item.patient.gender;
                        columnAffected = columnAffected + "gender,";
                    }

                    if (patient.DOB != item.patient.DOB)
                    {
                        patient.DOB = item.patient.DOB;
                        columnAffected = columnAffected + "DOB,";
                    }

                    if (patient.startDate != item.patient.startDate)
                    {
                        patient.startDate = item.patient.startDate;
                        columnAffected = columnAffected + "startDate,";
                     }

                    if (patient.endDate != item.patient.endDate)
                    {
                        patient.endDate = item.patient.endDate;
                        columnAffected = columnAffected + "endDate,";

                    }


                    if (patient.preferredName != item.patient.preferredName)
                    {
                        patient.preferredName = item.patient.preferredName;
                        columnAffected = columnAffected + "preferredName,";

                    }


                    int inputLanguageID = Convert.ToInt32(item.inputLanguageID);

                    if (inputLanguageID != -1)
                    {

                        if (language.languageListID != inputLanguageID)
                        {
                            var oldLogDatal = new JavaScriptSerializer().Serialize(language);

                            language.languageListID = inputLanguageID;

                            _context.SaveChanges();

                            var newLogDatal = new JavaScriptSerializer().Serialize(language);

                            string[] logVall = shortcutMethod.GetLogVal(oldLogDatal, newLogDatal);

                            string loldLogVal = logVall[0];
                            string lnewLogVal = logVall[1];

                            shortcutMethod.addLogToDB(oldLogDatal, newLogDatal, logDesc, 17, null, supervisorID, supervisorID, null, null, null, "language", "languageListID", loldLogVal, lnewLogVal, language.languageID, 1, 0, null);

                        }

                    }
                    else
                    {
                        List_Language languageDuplicated = _context.ListLanguages.Where(x => x.value == item.inputLanguage && x.isDeleted != 1).SingleOrDefault();

                        if (languageDuplicated == null)
                        {

                            //no such value existed in the list_language 
                            //create new value
                            List_Language listLang = new List_Language();
                            listLang.value = item.inputLanguage;
                            listLang.isChecked = 0;
                            listLang.createDateTime = DateTime.Now;
                            _context.ListLanguages.Add(listLang);
                            _context.SaveChanges();

                            shortcutMethod.addLogToDB(null, null, logDescList, 19, null, supervisorID, supervisorID, null, null, null, "list_language", "ALL", null, null, listLang.list_languageID, 1, 0, null);

                            //assign list_languageID to languageListID in the language table
                            var oldLogDatal = new JavaScriptSerializer().Serialize(language);
                            language.languageListID = listLang.list_languageID;
                            var newLogDatal = new JavaScriptSerializer().Serialize(language);

                            _context.SaveChanges();

                            string[] llogVal = shortcutMethod.GetLogVal(oldLogDatal, newLogDatal);

                            string loldLogVal = llogVal[0];
                            string lnewLogVal = llogVal[1];

                            shortcutMethod.addLogToDB(oldLogDatal, newLogDatal, logDesc, 17, null, supervisorID, supervisorID, null, null, null, "language", "languageListID", loldLogVal, lnewLogVal, language.languageID, 1, 0, null);

                        }
                        else
                        {
                            //if such value exist 
                            //change current value of the languageListID in the language table
                            List_Language listLang = _context.ListLanguages.Where(x => x.list_languageID == language.languageListID && x.isDeleted != 1).SingleOrDefault();


                            if (listLang.value != item.inputLanguage)
                            {
                                var oldLogDatall = new JavaScriptSerializer().Serialize(listLang);

                                listLang.value = item.inputLanguage;

                                var newLogDatall = new JavaScriptSerializer().Serialize(listLang);

                                _context.SaveChanges();

                                string[] llogVal = shortcutMethod.GetLogVal(oldLogDatall, newLogDatall);

                                string lloldLogVal = llogVal[0];
                                string llnewLogVal = llogVal[1];

                                shortcutMethod.addLogToDB(oldLogDatall, newLogDatall, logDesc, 17, null, supervisorID, supervisorID, null, null, null, "list_language", "ALL", lloldLogVal, llnewLogVal, listLang.list_languageID, 1, 0, null);

                            }


                        }

                        var languageID = language.languageID;
                        patient.preferredLanguageID = languageID;
                        columnAffected = columnAffected + "preferredLanguageID,";


                    }

                    string gcolumnAffected = "";
                    var oldLogDatapg = new JavaScriptSerializer().Serialize(patientGuardian);

                    ///////////////////////////////////////////
                    ////Guardian Information
                    ///

                    if (patientGuardian.guardianName != item.patientGuardian.guardianName)
                    {
                        patientGuardian.guardianName = item.patientGuardian.guardianName;
                        gcolumnAffected = gcolumnAffected + "guardianName,";
                    }

                    if (patientGuardian.guardianNRIC != item.patientGuardian.guardianNRIC)
                    {
                        patientGuardian.guardianNRIC = item.patientGuardian.guardianNRIC;
                        gcolumnAffected = gcolumnAffected + "guardianNRIC,";

                    }

                    int RSID = Convert.ToInt32(item.inputRSID);
                    if (RSID != -1)
                    {
                        if (patientGuardian.guardianRelationshipID != RSID)
                        {
                            patientGuardian.guardianRelationshipID = RSID;
                            gcolumnAffected = gcolumnAffected + "guardianRelationshipID,";

                        }
                    }
                    else
                    {
                        var rs = _context.ListRelationships.SingleOrDefault(x => x.value == item.inputRS && x.isDeleted != 1);

                        if (rs == null)
                        {
                            List_Relationship listRS = new List_Relationship();
                            listRS.value = item.inputRS;
                            listRS.isChecked = 0;
                            listRS.createDateTime = DateTime.Now;
                            _context.ListRelationships.Add(listRS);
                            _context.SaveChanges();

                            var newLogData = new JavaScriptSerializer().Serialize(listRS);
                            shortcutMethod.addLogToDB(null, newLogData, logDescList, 19, null, supervisorID, supervisorID, null, null, null, "list_relationship", "ALL", null, null, listRS.list_relationshipID, 1, 0, null);
                            patientGuardian.guardianRelationshipID = listRS.list_relationshipID;
                            gcolumnAffected = gcolumnAffected + "guardianRelationshipID,";

                        }
                        else
                        {
                            if (patientGuardian.guardianRelationshipID != rs.list_relationshipID)
                            {
                                patientGuardian.guardianRelationshipID = rs.list_relationshipID;
                                gcolumnAffected = gcolumnAffected + "guardianRelationshipID,";
                            }



                        }


                    }
                    if (patientGuardian.guardianContactNo != item.patientGuardian.guardianContactNo)
                    {
                        patientGuardian.guardianContactNo = item.patientGuardian.guardianContactNo;
                        gcolumnAffected = gcolumnAffected + "guardianContactNo,";

                    }

                    if (patientGuardian.guardianEmail != item.patientGuardian.guardianEmail)
                    {
                        patientGuardian.guardianEmail = item.patientGuardian.guardianEmail;
                        gcolumnAffected = gcolumnAffected + "guardianEmail,";

                    }


                    if (item.patientGuardian.guardianName2 != null)
                    {
                        if (patientGuardian.guardianName2 != item.patientGuardian.guardianName2)
                        {
                            patientGuardian.guardianName = item.patientGuardian.guardianName;
                            gcolumnAffected = gcolumnAffected + "guardianName,";

                        }

                        if (patientGuardian.guardianNRIC2 != item.patientGuardian.guardianNRIC2)
                        {
                            patientGuardian.guardianNRIC2 = item.patientGuardian.guardianNRIC;
                            gcolumnAffected = gcolumnAffected + "guardianNRIC2,";

                        }

                        int RSID2 = Convert.ToInt32(item.input2RSID);
                        if (RSID2 != -1)
                        {
                            if (patientGuardian.guardian2RelationshipID != RSID2 && item.patientGuardian.guardianName2 != null)
                            {
                                patientGuardian.guardian2RelationshipID = RSID2;
                                gcolumnAffected = gcolumnAffected + "guardian2RelationshipID,";

                            }
                        }
                        else
                        {
                            var rs2 = _context.ListRelationships.SingleOrDefault(x => x.value == item.input2RS && x.isDeleted != 1);

                            if (rs2 == null)
                            {
                                List_Relationship list2RS = new List_Relationship();
                                list2RS.value = item.input2RS;
                                list2RS.isChecked = 0;
                                list2RS.createDateTime = DateTime.Now;
                                _context.ListRelationships.Add(list2RS);
                                _context.SaveChanges();

                                var newLogData = new JavaScriptSerializer().Serialize(list2RS);
                                shortcutMethod.addLogToDB(null, newLogData, logDescList, 19, null, supervisorID, supervisorID, null, null, null, "list_relationship", "ALL", null, null, list2RS.list_relationshipID, 1, 0, null);
                                patientGuardian.guardian2RelationshipID = list2RS.list_relationshipID;
                                gcolumnAffected = gcolumnAffected + "guardian2RelationshipID,";

                            }
                            else
                            {
                                patientGuardian.guardian2RelationshipID = rs2.list_relationshipID;
                                gcolumnAffected = gcolumnAffected + "guardian2RelationshipID,";

                            }
                        }


                        if (patientGuardian.guardianContactNo2 != item.patientGuardian.guardianContactNo2)
                        {
                            patientGuardian.guardianContactNo2 = item.patientGuardian.guardianContactNo2;
                            gcolumnAffected = gcolumnAffected + "guardianContactNo2,";

                        }

                        if (patientGuardian.guardianEmail2 != item.patientGuardian.guardianEmail2)
                        {
                            patientGuardian.guardianEmail2 = item.patientGuardian.guardianEmail2;
                            gcolumnAffected = gcolumnAffected + "guardianEmail2,";

                        }
                    }

                    if (!gcolumnAffected.Equals(""))
                    {
                        var newLogDatapg = new JavaScriptSerializer().Serialize(patientGuardian);
                        _context.SaveChanges();


                        string[] pglogVal = shortcutMethod.GetLogVal(oldLogDatapg, newLogDatapg);

                        string pgoldLogVal = pglogVal[0];
                        string pgnewLogVal = pglogVal[1];

                        if (gcolumnAffected.EndsWith(","))
                        {
                            gcolumnAffected = gcolumnAffected.Substring(0, gcolumnAffected.Length - 1);

                        }

                        shortcutMethod.addLogToDB(oldLogDatapg, newLogDatapg, logDesc, 17, null, supervisorID, supervisorID, null, null, null, "patientGuardian", gcolumnAffected, pgoldLogVal, pgnewLogVal, patientGuardian.patientGuardianID, 1, 0, null);
                    }

                    ///////////////////////////////////////////
                    //Dementia Condition
                    //TO BE DONE

                    //if (pad.dementiaID != item.dementiaTypes.dementiaID)
                    //{
                    //    var pOlogData = new JavaScriptSerializer().Serialize(pad);

                    //    pad.dementiaID = item.dementiaTypes.dementiaID;

                    //    var plogData = new JavaScriptSerializer().Serialize(pad);

                    //    string[] plogVal = shortcutMethod.GetLogVal(pOlogData, plogData);

                    //    string poldLogVal = plogVal[0];
                    //    string pnewLogVal = plogVal[1];

                    //    shortcutMethod.addLogToDB(pOlogData, plogData, logDesc, 17, pa.patientAllocationID, supervisorID, supervisorID, null, null, null, "patientAssignedDementia", "dementiaID", poldLogVal, pnewLogVal, pad.padID, 1, 0, null);
                    //}

                    if (item.patient.isRespiteCare != patient.isRespiteCare)
                    {
                        patient.isRespiteCare = item.patient.isRespiteCare;
                        columnAffected = columnAffected + "isRespiteCare,";
                    }

                    if (columnAffected.EndsWith(","))
                    {
                        columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                    }

                    if (!columnAffected.Equals(""))
                    {
                        _context.SaveChanges();
                        TempData["success"] = "Changes saved successfully!!";
                        var logData = new JavaScriptSerializer().Serialize(patient);

                        string[] logVal = shortcutMethod.GetLogVal(oldLogData, logData);

                        string oldLogVal = logVal[0];
                        string newLogVal = logVal[1];

                        shortcutMethod.addLogToDB(oldLogData, logData, logDesc, 17, pa.patientAllocationID, supervisorID, supervisorID, null, null, null, "patient", columnAffected, oldLogVal, newLogVal, patient.patientID, 1, 0, null);
                    }
                }
                else
                {

                    TempData["error"] = errorMsg;
                }



            }

            return RedirectToAction("EditPatient", "Supervisor", new { id = item.patient.patientID });
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditGenInfo(string id)
        {
            int ID = Int32.Parse(id);


            var patient = _context.Patients.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            var pa = _context.PatientAllocations.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();

            var socialHistory = _context.SocialHistories.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault();

            var liveWith = _context.ListLiveWiths.Where(x => x.list_liveWithID == (_context.SocialHistories.Where(
                                                y => y.patientAllocationID == pa.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault()).liveWithID).FirstOrDefault();

            var occupation = _context.ListOccupations.Where(x => x.list_occupationID == (_context.SocialHistories.Where(
                       y => y.patientAllocationID == pa.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault()).occupationID).FirstOrDefault();

            var education = _context.ListEducations.Where(x => x.list_educationID == (_context.SocialHistories.Where(
                       y => y.patientAllocationID == pa.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault()).educationID).FirstOrDefault();

            var diet = _context.ListDiets.Where(x => x.list_dietID == (_context.SocialHistories.Where(
                       y => y.patientAllocationID == pa.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault()).dietID).FirstOrDefault();

            var pet = _context.ListPets.Where(x => x.list_petID == (_context.SocialHistories.Where(
                      y => y.patientAllocationID == pa.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault()).petID).FirstOrDefault();

            var religion = _context.ListReligions.Where(x => x.list_religionID == (_context.SocialHistories.Where(
                        y => y.patientAllocationID == pa.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault()).religionID).FirstOrDefault();

           
            var mobility = _context.ListMobility.Where(x => x.list_mobilityID == (_context.Mobility.Where(y => y.patientAllocationID == pa.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1).OrderByDescending(y => y.createdDateTime).FirstOrDefault().mobilityListID)).FirstOrDefault();



            //var liveWithList = _context.ListLiveWiths.ToList();

            //var religionList = _context.ListReligions.ToList();

            //var occupationList = _context.ListOccupations.ToList();

            //var petList = _context.ListPets.ToList();

            //var educationList = _context.ListEducations.ToList();

            //var dietList = _context.ListDiets.ToList();

            //var mobilityList = _context.ListMobility.ToList();



            var liveWithList = _context.ListLiveWiths.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_LiveWith live = new List_LiveWith();
            live.value = "Others";
            live.list_liveWithID = -1;
            liveWithList.Add(live);


            var religionList = _context.ListReligions.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Religion reg = new List_Religion();
            reg.value = "Others";
            reg.list_religionID = -1;
            religionList.Add(reg);

            var occupationList = _context.ListOccupations.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Occupation occ = new List_Occupation();
            occ.value = "Others";
            occ.list_occupationID = -1;
            occupationList.Add(occ);

            var petList = _context.ListPets.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Pet petL = new List_Pet();
            petL.value = "Others";
            petL.list_petID = -1;
            petList.Add(petL);

            var educationList = _context.ListEducations.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Education ed = new List_Education();
            ed.value = "Others";
            ed.list_educationID = -1;
            educationList.Add(ed);


            var dietList = _context.ListDiets.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Diet dietL = new List_Diet();
            dietL.value = "Others";
            dietL.list_dietID = -1;
            dietList.Add(dietL);

            var mobilityList = _context.ListMobility.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Mobility ml = new List_Mobility();
            ml.value = "Others";
            ml.list_mobilityID = -1;
            mobilityList.Add(ml);




            var viewModel = new ManageSupervisorsViewModel()
            {
                patient = patient,
                liveWithList = liveWithList,
                religionList = religionList,
                occupationList = occupationList,
                petList = petList,
                educationList = educationList,
                dietList = dietList,
                mobilityList = mobilityList,

                otherliveWith = liveWith,
                otherDiet = diet,
                otherOccupation = occupation,
                otherReligion = religion,
                otherEducation = education,
                otherPet = pet,
                otherMobility = mobility,
                socialHistory = socialHistory,

            };

            return View(viewModel);
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManageAllocation(string id)
        {

            int ID = Int32.Parse(id);

            var caregiverList = (from u in _context.Users
                                 join ut in _context.UserTypes on u.userTypeID equals ut.userTypeID
                                 where ut.userTypeName == "Caregiver" || ut.userTypeName == "Supervisor"
                                 where u.isApproved == 1 && u.isDeleted != 1
                                 where ut.isDeleted != 1
                                 select new UserViewModel
                                 {
                                     userID = u.userID,
                                     userFullname = u.lastName + " " + u.firstName,
                                 }).ToList();

            var doctorList = (from u in _context.Users
                              join ut in _context.UserTypes on u.userTypeID equals ut.userTypeID
                              where ut.userTypeName == "Doctor"
                              where u.isApproved == 1 && u.isDeleted != 1
                              where ut.isDeleted != 1
                              select new UserViewModel
                              {
                                  userID = u.userID,
                                  userFullname = u.lastName + " " + u.firstName,
                              }).ToList();

            var gametherapistList = (from u in _context.Users
                                     join ut in _context.UserTypes on u.userTypeID equals ut.userTypeID
                                     where ut.userTypeName == "Game Therapist"
                                     where u.isApproved == 1 && u.isDeleted != 1
                                     where ut.isDeleted != 1
                                     select new UserViewModel
                                     {
                                         userID = u.userID,
                                         userFullname = u.lastName + " " + u.firstName,
                                     }).ToList();

            var supervisorList = (from u in _context.Users
                                  join ut in _context.UserTypes on u.userTypeID equals ut.userTypeID
                                  where ut.userTypeName == "Supervisor"
                                  where u.isApproved == 1 && u.isDeleted != 1
                                  where ut.isDeleted != 1
                                  select new UserViewModel
                                  {
                                      userID = u.userID,
                                      userFullname = u.lastName + " " + u.firstName,
                                  }).ToList();

            var patientAllocated = _context.PatientAllocations.Where(x => x.patientID == ID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            var patientCaregiver = (from p in _context.Patients
                                    join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                    join u in _context.Users on pa.caregiverID equals u.userID
                                    //join ut in _context.UserTypes on u.userTypeID equals ut.userTypeID
                                    //where ut.isDeleted != 1
                                    //where ut.userTypeName == "Caregiver"
                                    where pa.isApproved == 1 && pa.isDeleted != 1
                                    where p.patientID == ID
                                    where p.isApproved == 1 && p.isDeleted != 1
                                    where u.isApproved == 1 && u.isDeleted != 1
                                    select new UserViewModel
                                    {
                                        userFullname = u.firstName + " " + u.lastName,
                                        userID = u.userID,

                                    }).SingleOrDefault();

            UserViewModel tempCaregiver = (from p in _context.Patients
                                           join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                           join u in _context.Users on pa.tempCaregiverID equals u.userID
                                           where pa.isApproved == 1 && pa.isDeleted != 1
                                           where p.patientID == ID
                                           where p.isApproved == 1 && p.isDeleted != 1
                                           where u.isApproved == 1 && u.isDeleted != 1
                                           //select u.userID
                                           select new UserViewModel
                                           {
                                               userFullname = u.firstName + " " + u.lastName,
                                               userID = u.userID,

                                           }

              ).SingleOrDefault();


            var patientDoctor = (from p in _context.Patients
                                 join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                 join u in _context.Users on pa.doctorID equals u.userID
                                 //join ut in _context.UserTypes on u.userTypeID equals ut.userTypeID
                                 //where ut.isDeleted != 1
                                 //where ut.userTypeName == "Doctor"
                                 where pa.isApproved == 1 && pa.isDeleted != 1
                                 where p.patientID == ID
                                 where p.isApproved == 1 && p.isDeleted != 1
                                 where u.isApproved == 1 && u.isDeleted != 1
                                 select new UserViewModel
                                 {
                                     userFullname = u.firstName + " " + u.lastName,
                                     userID = u.userID,

                                 }).SingleOrDefault();


            UserViewModel tempDoctor = (from p in _context.Patients
                                        join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                        join u in _context.Users on pa.tempDoctorID equals u.userID
                                        where pa.isApproved == 1 && pa.isDeleted != 1
                                        where p.patientID == ID
                                        where p.isApproved == 1 && p.isDeleted != 1
                                        where u.isApproved == 1 && u.isDeleted != 1
                                        //select u.userID
                                        select new UserViewModel
                                        {
                                            userFullname = u.firstName + " " + u.lastName,
                                            userID = u.userID,

                                        }

                ).SingleOrDefault();


            var hour = DateTime.Now.Hour;
            var min = DateTime.Now.Minute;
            var sec = DateTime.Now.Second;

            UserViewModel tempSupervisor = (from p in _context.Patients
                                            join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                            join pic in _context.PersonInCharge on pa.supervisorID equals pic.primaryUserTypeID
                                            where pa.isApproved == 1 && pa.isDeleted != 1
                                            where p.patientID == ID
                                            where p.isApproved == 1 && p.isDeleted != 1
                                            where pic.dateStart < DateTime.Today && pic.dateEnd > DateTime.Today && pic.timeStart < EntityFunctions.CreateTime(hour, min, sec) && pic.timeEnd > EntityFunctions.CreateTime(hour, min, sec)
                                            select new UserViewModel
                                            {

                                                userFullname = _context.Users.Where(x => x.userID == pic.tempUserID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault().firstName + " " +
                                                _context.Users.Where(x => x.userID == pic.tempUserID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault().lastName,

                                                userID = _context.Users.Where(x => x.userID == pic.tempUserID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault().userID,

                                            }

               ).FirstOrDefault();

            UserViewModel tempGametherapist = (from p in _context.Patients
                                               join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                               join pic in _context.PersonInCharge on pa.caregiverID equals pic.primaryUserTypeID
                                               where pa.isApproved == 1 && pa.isDeleted != 1
                                               where p.patientID == ID
                                               where p.isApproved == 1 && p.isDeleted != 1
                                               where pic.dateStart < DateTime.Today && pic.dateEnd > DateTime.Today && pic.timeStart < EntityFunctions.CreateTime(hour, min, sec) && pic.timeEnd > EntityFunctions.CreateTime(hour, min, sec)
                                               select new UserViewModel
                                               {

                                                   userFullname = _context.Users.Where(x => x.userID == pic.tempUserID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault().firstName + " " +
                                                    _context.Users.Where(x => x.userID == pic.tempUserID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault().lastName,

                                                   userID = _context.Users.Where(x => x.userID == pic.tempUserID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault().userID,

                                               }

               ).FirstOrDefault();


            var tempGametherapistList = (from p in _context.Patients
                                         join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                         join pic in _context.PersonInCharge on pa.gametherapistID equals pic.primaryUserTypeID
                                         where pa.isApproved == 1 && pa.isDeleted != 1
                                         where p.patientID == ID
                                         where p.isApproved == 1 && p.isDeleted != 1
                                         select new UserViewModel
                                         {

                                             userFullname = _context.Users.Where(x => x.userID == pic.tempUserID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault().firstName + " " +
                                              _context.Users.Where(x => x.userID == pic.tempUserID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault().lastName,

                                             userID = _context.Users.Where(x => x.userID == pic.tempUserID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault().userID,

                                         }

              ).ToList();

            var tempSupervisorList = (from p in _context.Patients
                                      join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                      join pic in _context.PersonInCharge on pa.supervisorID equals pic.primaryUserTypeID
                                      where pa.isApproved == 1 && pa.isDeleted != 1
                                      where p.patientID == ID
                                      where p.isApproved == 1 && p.isDeleted != 1
                                      select new UserViewModel
                                      {

                                          userFullname = _context.Users.Where(x => x.userID == pic.tempUserID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault().firstName + " " +
                                           _context.Users.Where(x => x.userID == pic.tempUserID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault().lastName,

                                          userID = _context.Users.Where(x => x.userID == pic.tempUserID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault().userID,

                                      }

             ).ToList();

            var patientGametherapist = (from p in _context.Patients
                                        join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                        join u in _context.Users on pa.gametherapistID equals u.userID
                                        //join ut in _context.UserTypes on u.userTypeID equals ut.userTypeID
                                        //where ut.isDeleted != 1
                                        //where ut.userTypeName == "Game Therapist"
                                        where pa.isApproved == 1 && pa.isDeleted != 1
                                        where p.patientID == ID
                                        where p.isApproved == 1 && p.isDeleted != 1
                                        where u.isDeleted != 1
                                        where u.isApproved == 1
                                        select new UserViewModel
                                        {
                                            userFullname = u.firstName + " " + u.lastName,
                                            userID = u.userID,

                                        }).SingleOrDefault();

            var patientSupervisor = (from p in _context.Patients
                                     join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                     join u in _context.Users on pa.supervisorID equals u.userID
                                     //join ut in _context.UserTypes on u.userTypeID equals ut.userTypeID
                                     //where ut.isDeleted != 1
                                     //where ut.userTypeName == "Game Therapist"
                                     where pa.isApproved == 1 && pa.isDeleted != 1
                                     where p.patientID == ID
                                     where p.isApproved == 1 && p.isDeleted != 1
                                     where u.isDeleted != 1
                                     where u.isApproved == 1
                                     select new UserViewModel
                                     {
                                         userFullname = u.firstName + " " + u.lastName,
                                         userID = u.userID,

                                     }).SingleOrDefault();







            var patient = _context.Patients.Where(x => x.patientID == ID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            var viewModel = new AllocationViewModel()
            {
                patient = patient,
                caregiverList = caregiverList,
                doctorList = doctorList,
                gametherapistList = gametherapistList,
                supervisorList = supervisorList,
                assignedSupervisor = patientSupervisor,
                assignedCaregiver = patientCaregiver,
                assignedDoctor = patientDoctor,
                assignedGametherapist = patientGametherapist,
                tempCaregiverInfo = tempCaregiver,
                tempDoctorInfo = tempDoctor,
                tempSupervisorInfo = tempSupervisor,
                tempGametherapistInfo = tempGametherapist,
                tempGametherapistList = tempGametherapistList,
                tempSupervisorList = tempSupervisorList,
            };

            return View(viewModel);
        }



        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManageAllocationMethod(ManageSupervisorsViewModel viewModel)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            var patientAllocation = _context.PatientAllocations.Single(x => x.patientID == viewModel.id);
            var gametherapistObj = _context.Users.SingleOrDefault(x => x.userID == viewModel.assignedGametherapist);

            var oldLogData = new JavaScriptSerializer().Serialize(patientAllocation);

            string affectedColumn = "";


            if (patientAllocation != null)
            {
                if (patientAllocation.caregiverID != viewModel.assignedCaregiver)
                {
                    patientAllocation.caregiverID = viewModel.assignedCaregiver;
                    affectedColumn = affectedColumn + "caregiverID";

                }

                if (patientAllocation.doctorID != viewModel.assignedDoctor)
                {

                    patientAllocation.doctorID = viewModel.assignedDoctor;
                    affectedColumn = affectedColumn + "doctorID";

                }


                if (patientAllocation.gametherapistID != viewModel.assignedGametherapist)
                {
                    patientAllocation.gametherapistID = gametherapistObj.userID;
                    affectedColumn = affectedColumn + "gametherapistID";


                }

                _context.SaveChanges();
                TempData["success"] = "Changes successfully saved";


                var newLogData = new JavaScriptSerializer().Serialize(patientAllocation);
                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "patientAllocation", affectedColumn, null, null, patientAllocation.patientAllocationID, 1, 0, null);


            }

            return RedirectToAction("ManageAllocation", "Supervisor", new { id = viewModel.id });
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManageActivities()
        {
            //var activities = _context.CentreActivities.OrderBy(x => x.centreActivityID).ToList();
            //var activities = _context.CentreActivities.Include(x => x.centreActivityID).Where(x => x.centreActivityID.is == 1).Where(a => a.centreActivityID.isDeleted != 1).ToList();

            // Below is must be approve and not deleted
            //var activities = _context.CentreActivities.Where(x => x.isApproved == 1).Where(x => x.isDeleted != 1).OrderBy(x => x.centreActivityID).ToList();

            // Below query either "approve or not" and not deleted
            var activities = _context.CentreActivities.Where(x => x.isDeleted != 1).OrderBy(x => x.centreActivityID).ToList();


            //var ActivityAvailable = new AvailableActivity(
            //{
            //    ListCentreActivities = activities

            //}).ToList();


            var availableActivity = (from act in _context.CentreActivities
                                     where act.isDeleted != 1
                                     where act.isApproved == 1
                                     //where act.activityEndDate >= DateTime.Today || act.activityEndDate == null
                                     select new AvailableActivity
                                     {
                                         centreActivities = act,
                                         listAvailability = _context.ActivityAvailabilities.Where(x => x.centreActivityID == act.centreActivityID && x.isApproved == 1 && x.isDeleted != 1).ToList(),

                                     }).ToList();

            var viewModel = new ManageActivityViewModel()
            {
                AvailableActivity = availableActivity,
            };

            return View(viewModel);
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult AddActivity()
        {
            //List<string> timingDuration = new List<string>(new string[] { "30", "60", "90", "120", "150", "180", "210", "240", "270", "300", "330", "360", "390", "420", "450", "480" });
            List<string> timingDuration = new List<string>(new string[] { "30", "60" });
            ViewBag.SelectedDuration = "30";
            ViewBag.timingDuration = timingDuration;

            List<string> minReq = new List<string>(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
            ViewBag.SelectedDuration = "1";
            ViewBag.minReq = minReq;

            List<string> dayList = new List<string>(new string[] { "Everyday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" });
            ViewBag.SelectedDay = "Everyday";
            ViewBag.ListOfDay = dayList;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult AddActivityMethod(ManageSupervisorsViewModel viewModel, FormCollection form)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            //int supervisorID = Int32.Parse(Session["userID"].ToString());
            string errorMsg = "";

            var isTitleExist = _context.CentreActivities.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.activityTitle == viewModel.title).SingleOrDefault();

            var isshortTitleExist = _context.CentreActivities.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.shortTitle == viewModel.shortTitle).SingleOrDefault();

            if (isTitleExist == null)
            {
                if (viewModel.startDate >= DateTime.Today.Date && viewModel.startDate <= viewModel.endDate)
                {
                    centreActivityMethod.addCentreActivity(supervisorID, viewModel.title, viewModel.shortTitle, viewModel.description, viewModel.minDuration,
                    viewModel.maxDuration, viewModel.minPeopleReq, viewModel.isCompulsory, viewModel.isGroup, viewModel.isFixed, viewModel.startDate, viewModel.endDate, 1);

                    //CentreActivity activity = new CentreActivity();
                    //activity.activityTitle = viewModel.title;
                    //activity.shortTitle = viewModel.shortTitle;
                    //activity.activityDesc = viewModel.description;
                    //activity.minDuration = viewModel.minDuration;
                    //activity.maxDuration = viewModel.maxDuration;
                    //activity.minPeopleReq = viewModel.minPeopleReq;
                    //activity.isCompulsory = viewModel.isCompulsory;
                    //activity.isGroup = viewModel.isGroup;
                    //activity.isFixed = viewModel.isFixed;
                    //activity.isDeleted = 0;
                    //activity.isApproved = 1;

                    //activity.activityStartDate = viewModel.startDate;
                }
                else
                {
                    TempData["error"] = "Failed to add activity. <br/> Invalid start date.";
                    return RedirectToAction("AddActivity", "Supervisor");
                }
                //activity.activityEndDate = viewModel.endDate;
                //activity.createDateTime = DateTime.Now;

                //if (activity != null)
                //{
                //    _context.CentreActivities.Add(activity);
                //    _context.SaveChanges();
                //    string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                //    var newLogData = new JavaScriptSerializer().Serialize(activity);

                //    shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, null, supervisorID, supervisorID, null, null, null, "centreActivity", "ALL", null, null, activity.centreActivityID, 1, 0, null);



            }
            else
            {

                if (isTitleExist != null)
                {
                    errorMsg = "<br/>  An identical activity title exists.";
                }

                if (isshortTitleExist != null)
                {
                    errorMsg = "<br/>  An identical short title exists.";

                }

                TempData["error"] = "Failed to add an activity." + errorMsg;
                return RedirectToAction("AddActivity", "Supervisor");
            }


            TempData["success"] = "Successfully added an activity on " + DateTime.Now;
            return RedirectToAction("ManageActivities", "Supervisor");

        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetActivityPreference(int patientId)
        {
            var patientAllocationID = _context.PatientAllocations.Where(x => x.patientID == patientId && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault().patientAllocationID;

            // Get activities
            var AP = _context.ActivityPreferences.Where(x => x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).ToList();

            foreach (var item in AP)
            {
                item.CentreActivity = _context.CentreActivities.Where(x => x.centreActivityID == item.centreActivityID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault();
            }

            return Json(AP);
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetExclusionById(int Id)
        {

            // Get activities
            var ActivityExclusion = _context.ActivityExclusions.Where(x => x.activityExclusionId == Id && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();


            return Json(ActivityExclusion);
        }



        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetActivityExclusion(int patientId)
        {

            var patientAllocationID = _context.PatientAllocations.Where(x => x.patientID == patientId && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault().patientAllocationID;

            // Get activities
            var ActivityExclusion = _context.ActivityExclusions.Where(x => x.patientAllocationID == patientAllocationID && x.isDeleted != 1 && x.isApproved == 1).ToList();

            foreach (var item in ActivityExclusion)
            {
                item.CentreActivity = _context.CentreActivities.Where(x => x.centreActivityID == item.centreActivityID && x.isDeleted != 1 && x.isApproved == 1).FirstOrDefault();
            }

            return Json(ActivityExclusion);
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetPatientPrescription(int patientId)
        {


            var patientPrescription = (from p in _context.Patients
                                       join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                       join pscp in _context.Prescriptions on pa.patientAllocationID equals pscp.patientAllocationID
                                       //join med in _context.MedicationLog on pa.patientAllocationID equals med.patientAllocationID
                                       where p.isDeleted != 1
                                       where pa.isDeleted != 1
                                       where pscp.isDeleted != 1
                                       where p.isApproved == 1
                                       where pa.isApproved == 1
                                       where pscp.isApproved == 1
                                       where p.patientID == patientId
                                       //where med.dateTaken == DateTime.Today

                                       select new PatientPrescription
                                       {
                                           prescription = pscp,
                                           PrescriptionName = _context.ListPrescriptions.Where(x => x.list_prescriptionID == pscp.drugNameID && x.isDeleted != 1).FirstOrDefault().value,
                                           //prescriptionList = _context.Prescriptions.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault(),

                                           medLog = _context.MedicationLog.Where(x => x.prescriptionID == pscp.prescriptionID && x.dateTaken == DbFunctions.TruncateTime(DateTime.Today)).FirstOrDefault(),
                                           userFullname = _context.MedicationLog.Where(x => x.prescriptionID == pscp.prescriptionID && x.dateTaken == DbFunctions.TruncateTime(DateTime.Today)).FirstOrDefault().User.AspNetUsers.firstName + " " +
                                           _context.MedicationLog.Where(x => x.prescriptionID == pscp.prescriptionID && x.dateTaken == DbFunctions.TruncateTime(DateTime.Today)).FirstOrDefault().User.AspNetUsers.lastName


                                       }).ToList();


            return Json(patientPrescription);

        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetMedication(int id)
        {


            var patientMedication = (from med in _context.MedicationLog
                                     join pscp in _context.Prescriptions on med.prescriptionID equals pscp.prescriptionID
                                     where pscp.isDeleted != 1
                                     where pscp.isApproved == 1
                                     where med.medicationLogID == id


                                     select new PatientMedication
                                     {
                                         med = med,
                                         prescription = pscp,
                                         PrescriptionName = _context.ListPrescriptions.Where(x => x.list_prescriptionID == pscp.drugNameID && x.isDeleted != 1).FirstOrDefault().value,
                                         userFullname = med.User.AspNetUsers.lastName + " " + med.User.AspNetUsers.firstName,

                                     }).SingleOrDefault();


            return Json(patientMedication);

        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ChangePatientDP(HttpPostedFileBase file, ManageSupervisorsViewModel model)
        {

            int patientID = model.patient.patientID;

            try
            {
                if (file != null)
                {

                    int userID = Convert.ToInt32(User.Identity.GetUserID2());
                    var patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                    string firstName = patient.firstName;
                    string lastName = patient.lastName;
                    string maskedNric = patient.maskedNric;

                    string result = patientMethod.uploadProfileImage(Server, file, patientID, userID, firstName, lastName, maskedNric);

                    if (result == null)
                    {
                        TempData["error"] = "Error in uploading to cloudinary";
                    }
                    else
                    {
                        TempData["success"] = "Image Uploaded Successfully";


                    }

                }
                else
                {
                    TempData["error"] = "No file chosen!";

                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex;
            }

            return RedirectToAction("ManagePatient", "Supervisor");


        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetPatientProblemLog(int patientId)
        {
            var patientAllocationID = _context.PatientAllocations.Where(x => x.patientID == patientId && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault().patientAllocationID;
            var problemlog = _context.ProblemLogs.Where(x => x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).ToList();

            return Json(problemlog);

        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetPatientAllocation(int patientId)
        {
            var patientAllocationID = _context.PatientAllocations.Where(x => x.patientID == patientId && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault().patientAllocationID;
            var patientCaregiver = (from p in _context.Patients
                                    join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                    join u in _context.Users on pa.caregiverID equals u.userID
                                    //join ut in _context.UserTypes on u.userTypeID equals ut.userTypeID
                                    //where ut.isDeleted != 1
                                    //where ut.userTypeName == "Caregiver"
                                    where pa.isApproved == 1 && pa.isDeleted != 1
                                    where p.patientID == patientId
                                    where p.isApproved == 1 && p.isDeleted != 1
                                    select new UserViewModel
                                    {
                                        userFullname = u.firstName + " " + u.lastName,
                                        userID = u.userID,

                                    }).SingleOrDefault();

            UserViewModel tempCaregiver = (from p in _context.Patients
                                           join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                           join u in _context.Users on pa.tempCaregiverID equals u.userID
                                           where pa.isApproved == 1 && pa.isDeleted != 1
                                           where p.patientID == patientId
                                           where p.isApproved == 1 && p.isDeleted != 1
                                           //select u.userID
                                           select new UserViewModel
                                           {
                                               userFullname = u.firstName + " " + u.lastName,
                                               userID = u.userID,

                                           }

              ).SingleOrDefault();


            var patientDoctor = (from p in _context.Patients
                                 join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                 join u in _context.Users on pa.doctorID equals u.userID
                                 //join ut in _context.UserTypes on u.userTypeID equals ut.userTypeID
                                 //where ut.isDeleted != 1
                                 //where ut.userTypeName == "Doctor"
                                 where pa.isApproved == 1 && pa.isDeleted != 1
                                 where p.patientID == patientId
                                 where p.isApproved == 1 && p.isDeleted != 1
                                 select new UserViewModel
                                 {
                                     userFullname = u.firstName + " " + u.lastName,
                                     userID = u.userID,

                                 }).SingleOrDefault();


            UserViewModel tempDoctor = (from p in _context.Patients
                                        join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                        join u in _context.Users on pa.tempDoctorID equals u.userID
                                        where pa.isApproved == 1 && pa.isDeleted != 1
                                        where p.patientID == patientId
                                        where p.isApproved == 1 && p.isDeleted != 1
                                        //select u.userID
                                        select new UserViewModel
                                        {
                                            userFullname = u.firstName + " " + u.lastName,
                                            userID = u.userID,

                                        }

                ).SingleOrDefault();

            var patient = _context.Patients.SingleOrDefault(x => x.patientID == patientId && x.isApproved == 1 && x.isDeleted != 1);

            var viewModel = new AllocationViewModel()
            {
                patient = patient,
                assignedCaregiver = patientCaregiver,
                assignedDoctor = patientDoctor,
                tempCaregiverInfo = tempCaregiver,
                tempDoctorInfo = tempDoctor,

            };

            return Json(viewModel);
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetPatientName(int patientId)
        {
            var patient = _context.Patients.Where(x => x.patientID == patientId && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault();
            var patientname = patient.firstName + " " + patient.lastName;
            return Json(patientname);
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GenerateSchedule()
        {
            return View();
        }

        //end Date Changes
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult DeleteActivity1(string id)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            var patients = _context.Patients.Where(x => x.isApproved == 1 && x.isDeleted != 1).ToList();
            //int supervisorID = Int32.Parse(Session["userID"].ToString());

            int ID = Int32.Parse(id);
            var activity = _context.CentreActivities.Single(x => x.centreActivityID == ID);
            if (activity != null)
            {

                string columnAffected = "";
                var oldLogData = new JavaScriptSerializer().Serialize(activity);
                if (activity.activityStartDate > DateTime.Today)
                {
                    activity.activityStartDate = DateTime.Today;
                    columnAffected = "activityStarDate,";
                }

                activity.activityEndDate = DateTime.Now;
                columnAffected = "activityEndDate";

                var newLogData = new JavaScriptSerializer().Serialize(activity);

                //foreach (var x in patients)
                //{
                //    x.updateBit = 1;

                //}
                if (columnAffected.EndsWith(","))
                {
                    columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                }

                _context.SaveChanges();
                TempData["success"] = "Successfully deleted an activity on " + DateTime.Now;

                // Note: the patientID is equal 0 as it does not affect any patient
                // shortcutMethod.addLogToDB(string? oldLogData, string logData, string logDesc, int logCategoryID, int? patientID, int userIDInit, int? userIDApproved, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;


                string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                string oldLogVal = logVal[0];
                string newLogVal = logVal[1];
                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, null, supervisorID, supervisorID, null, null, null, "centreActivity", columnAffected, oldLogVal, newLogVal, activity.centreActivityID, 1, 0, null);

            }

            return RedirectToAction("ManageActivities", "Supervisor");
        }

        //delete bit set
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult DeleteActivity(string id)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            var patients = _context.Patients.Where(x => x.isApproved == 1 && x.isDeleted != 1).ToList();
            //int supervisorID = Int32.Parse(Session["userID"].ToString());

            int ID = Int32.Parse(id);
            var activity = _context.CentreActivities.Single(x => x.centreActivityID == ID);
            if (activity != null)
            {
                var oldLogData = new JavaScriptSerializer().Serialize(activity);

                activity.isDeleted = 1;
                var newLogData = new JavaScriptSerializer().Serialize(activity);

                foreach (var x in patients)
                {
                    x.updateBit = 1;

                }

                _context.SaveChanges();
                TempData["success"] = "Successfully deleted an activity on " + DateTime.Now;

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, null, supervisorID, supervisorID, null, null, null, "centreActivity", "isDeleted", null, null, activity.centreActivityID, 1, 0, null);

                //generate schedule
                scheduler.generateWeeklySchedule(false, false);
            }

            return RedirectToAction("ManageActivities", "Supervisor");
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult DeleteMedicalHistory(string medHId, string patientId)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = Int32.Parse(patientId);

            int ID = Int32.Parse(medHId);
            var medicalHistory = _context.MedicalHistory.Where(x => x.medicalHistoryID == ID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();


            if (medicalHistory != null)
            {
                patientMethod.deleteMedicalHistory(supervisorID, ID, patientAllocation.patientAllocationID, 1);

                TempData["success"] = "Successfully deleted a medical record on " + DateTime.Now + ".";

        
            }
            else
            {
                TempData["error"] = "Failed to delete a medical record.";

            }
            return RedirectToAction("ManageMedicalHistory", "Supervisor", new { id = patientId });

        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult DeletePrescription(string pscpId, string patientId)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = Int32.Parse(patientId);

            int prescriptionID = Convert.ToInt32(pscpId);
            var prescription = _context.Prescriptions.Where(x => x.prescriptionID == prescriptionID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();


            if (prescription != null)
            {
        
                

                patientMethod.deletePrescription(supervisorID, prescriptionID, patientAllocation.patientAllocationID, 1);
                TempData["success"] = "Successfully deleted a prescription record on " + DateTime.Now + ".";
                
            }
            else
            {
                TempData["error"] = "Failed to delete a prescription record.";

            }
            return new RedirectResult(Url.Action("ManagePrescription", "Supervisor", new { id = patientId }) + "#prescription");

            //return RedirectToAction("ManagePrescription", "Supervisor", new { id = patientId });

        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult DeleteVital(string VId, string patientId)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            int vitalID = Int32.Parse(VId);
            var vital = _context.Vitals.Where(x => x.vitalID == vitalID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            int patientID = Int32.Parse(patientId);
            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();


            if (vital != null)
            {
                //var oldLogData = new JavaScriptSerializer().Serialize(vital);
                //vital.isDeleted = 1;
                //var newLogData = new JavaScriptSerializer().Serialize(vital);

                //_context.SaveChanges();

                patientMethod.deleteVital(supervisorID, vitalID, patientAllocation.patientAllocationID, 1);
                TempData["success"] = "Successfully deleted a vital record on " + DateTime.Now + ".";

                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "vital", "isDeleted", null, null, ID, 1, 0, null);


            }
            else
            {
                TempData["error"] = "Failed to delete a vital record.";

            }
            return RedirectToAction("ManageVital", "Supervisor", new { id = patientId });

        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult DeleteAllergy(string alId, string patientId)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = Int32.Parse(patientId);

            int allergyID = Int32.Parse(alId);

            var allergy = _context.Allergies.Where(x => x.allergyID == allergyID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            if (allergy != null)
            {
                patientMethod.deleteAllergy(supervisorID, allergyID, patientAllocation.patientAllocationID, 1);
                TempData["success"] = "Successfully deleted a medical record on " + DateTime.Now + ".";


            }
            else
            {
                TempData["error"] = "Failed to delete a medical record.";

            }
            return RedirectToAction("ManageAllergy", "Supervisor", new { id = patientId });

        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult DeletePersoLike(string lId, string patientId)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = Int32.Parse(patientId);

            int likeID = Int32.Parse(lId);
            var like = _context.Likes.Where(x => x.likeID == likeID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            if (like != null)
            {
                //var oldLogData = new JavaScriptSerializer().Serialize(like);
                //like.isDeleted = 1;
                //var newLogData = new JavaScriptSerializer().Serialize(like);

                //_context.SaveChanges();

                patientMethod.deleteLike(supervisorID, likeID, patientAllocation.patientAllocationID, 1);
                TempData["success"] = "Successfully deleted a personal preference record on " + DateTime.Now + ".";

                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "like", "isDeleted", null, null, ID, 1, 0, null);


            }
            else
            {
                TempData["error"] = "Failed to delete a personal preference record.";

            }
            return RedirectToAction("ManagePersoPreference", "Supervisor", new { id = patientId });

        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult DeletePersoDislike(string dlId, string patientId)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = Int32.Parse(patientId);

            int dislikeID = Int32.Parse(dlId);
            var dislike = _context.Dislikes.Where(x => x.dislikeID == dislikeID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            if (dislike != null)
            {

                //var oldLogData = new JavaScriptSerializer().Serialize(dislike);
                //dislike.isDeleted = 1;
                //var newLogData = new JavaScriptSerializer().Serialize(dislike);

                //_context.SaveChanges();
                patientMethod.deleteDislike(supervisorID, dislikeID, patientAllocation.patientAllocationID, 1);
                TempData["success"] = "Successfully deleted a personal preference record on " + DateTime.Now + ".";

                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "dislike", "isDeleted", null, null, ID, 1, 0, null);


            }
            else
            {
                TempData["error"] = "Failed to delete a personal preference record.";

            }
            return RedirectToAction("ManagePersoPreference", "Supervisor", new { id = patientId });

        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult DeleteProblemLogDetails(string plId, string patientId)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = Int32.Parse(patientId);

            int problemLogID = Int32.Parse(plId);
            var pl = _context.ProblemLogs.Where(x => x.problemLogID == problemLogID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            if (pl != null)
            {
                //var oldLogData = new JavaScriptSerializer().Serialize(pl);
                //pl.isDeleted = 1;
                //var newLogData = new JavaScriptSerializer().Serialize(pl);

                //_context.SaveChanges();

                patientMethod.deleteProblemLog(supervisorID, problemLogID, patientAllocation.patientAllocationID, 1);
                TempData["success"] = "Successfully deleted a problem log record on " + DateTime.Now + ".";

                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "problemLog", "isDeleted", null, null, ID, 1, 0, null);


            }
            else
            {
                TempData["error"] = "Failed to delete a problem log record.";

            }
            return RedirectToAction("ManageProblemLog", "Supervisor", new { id = patientId });

        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult LikeActivityPref(string Id, string patientId, string type)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = Int32.Parse(patientId);

            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1);

            int ID = Int32.Parse(Id);

            var actPref = _context.ActivityPreferences.Where(x => x.activityPreferencesID == ID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            string affectedColumn = "isLike,isNeutral,isDislike";

            var oldLogData = new JavaScriptSerializer().Serialize(actPref);


            if (actPref != null)
            {


                //patient
                var p = _context.Patients.SingleOrDefault(x => x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1);

                var oLogData = new JavaScriptSerializer().Serialize(p);
                p.updateBit = 1;
                _context.SaveChanges();

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                var nLogData = new JavaScriptSerializer().Serialize(p);


                string[] llogVal = shortcutMethod.GetLogVal(oLogData, nLogData);

                string oLogVal = llogVal[0];
                string nLogVal = llogVal[1];

                shortcutMethod.addLogToDB(oLogData, nLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "patient", "updateBit", oLogVal, nLogVal, p.patientID, 1, 0, null);


                //update bit changes

                //scheduler.preferencesUpdateBit(actPref.activityPreferencesID, actPref.isLike, actPref.isDislike, actPref.isNeutral, actPref.doctorRecommendation);


                logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;


                //activity pref
                if (type.Equals("like"))
                {
                    actPref.isLike = 1;
                    actPref.isNeutral = 0;
                    actPref.isDislike = 0;
                }
                else if (type.Equals("neutral"))
                {
                    actPref.isLike = 0;
                    actPref.isNeutral = 1;
                    actPref.isDislike = 0;

                }
                else if (type.Equals("dislike"))
                {
                    actPref.isLike = 0;
                    actPref.isNeutral = 0;
                    actPref.isDislike = 1;

                }




                _context.SaveChanges();
                TempData["success"] = "Successfully changed activity preferences.";
                var newLogData = new JavaScriptSerializer().Serialize(actPref);

                string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                string oldLogVal = logVal[0];
                string newLogVal = logVal[1];

                logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "activityPreference", affectedColumn, oldLogVal, newLogVal, ID, 1, 0, null);


            }
            else
            {
                TempData["error"] = "Failed to change activity preference.";

            }

            var referral = Request.UrlReferrer.ToString();

            if (referral.Contains("ManagePreference"))
            {

                return RedirectToAction("ManagePreference", "Supervisor", new { id = patientId });
            }
            else
            {
                TempData["activityID"] = actPref.centreActivityID;
                return RedirectToAction("ManagePatientPreference", "Supervisor");

            }

        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult DeleteAllocationDetails(AllocationViewModel viewModel)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = viewModel.patient.patientID;

            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            //string columnAffected = "";
            var oldLogData = new JavaScriptSerializer().Serialize(patientAllocation);

            if (patientAllocation != null)
            {
                patientMethod.deleteTemporaryAllocation(supervisorID, patientID, viewModel.usertypeID, 1);

                //if (viewModel.usertypeID == 2)
                //{
                //    patientAllocation.tempCaregiverID = null;
                //    TempData["success"] = "Successfully deleted a temporary caregiver on " + DateTime.Now + ".";
                //    columnAffected = "tempCaregiverID";
                //}

                //if (viewModel.usertypeID == 3)
                //{
                //    patientAllocation.tempDoctorID = null;
                //    TempData["success"] = "Successfully deleted a temporary doctor on " + DateTime.Now + ".";
                //    columnAffected = "tempDoctorID";

                //}
                //var LogData = new JavaScriptSerializer().Serialize(patientAllocation);

                //_context.SaveChanges();

                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                //string[] logVal = shortcutMethod.GetLogVal(oldLogData, LogData);

                //string oldLogVal = logVal[0];
                //string newLogVal = logVal[1];

                //shortcutMethod.addLogToDB(oldLogData, LogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "patientAllocation", columnAffected, oldLogVal, newLogVal, patientAllocation.patientAllocationID, 1, 0, null);


            }
            else
            {
                TempData["error"] = "Failed to delete a temporary allocation.";

            }
            return RedirectToAction("ManageAllocation", "Supervisor", new { id = patientID });

        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult DeleteRoutine1(string plId, string patientId)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = Int32.Parse(patientId);

            int ID = Int32.Parse(plId);
            var ro = _context.Routines.Where(x => x.routineID == ID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            var patient = _context.Patients.Where(x => x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();


            if (ro != null)
            {
                //scheduler.routineUpdateBit(ro.routineID, ro.startDate, ro.endDate, ro.day, ro.startTime, ro.startTime);

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;


                //routine
                var oldLogData = new JavaScriptSerializer().Serialize(ro);

                ro.endDate = DateTime.Today;
                ro.endTime = DateTime.Now.TimeOfDay;
                _context.SaveChanges();
                TempData["success"] = "Successfully deleted a routine on " + DateTime.Now + ".";


                //patient
                var opLogData = new JavaScriptSerializer().Serialize(patient);
                patient.updateBit = 1;
                var pnLogData = new JavaScriptSerializer().Serialize(patient);
                _context.SaveChanges();

                string[] plogVal = shortcutMethod.GetLogVal(opLogData, pnLogData);

                string poldLogVal = plogVal[0];
                string pnewLogVal = plogVal[1];
                shortcutMethod.addLogToDB(opLogData, pnLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "patient", "updateBit", poldLogVal, pnewLogVal, patient.patientID, 1, 0, null);



                //routine
                logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                var newLogData = new JavaScriptSerializer().Serialize(ro);

                string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                string oldLogVal = logVal[0];
                string newLogVal = logVal[1];
                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "routine", "endDate,endTime", oldLogVal, newLogVal, ID, 1, 0, null);


            }
            else
            {
                TempData["error"] = "Failed to delete a routine.";

            }
            return RedirectToAction("ManageRoutine", "Supervisor", new { id = patientId });

        }

        //set delete bit
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult DeleteRoutine(string roId, string patientId)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = Int32.Parse(patientId);

            int routineID = Int32.Parse(roId);
            var ro = _context.Routines.Where(x => x.routineID == routineID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            var patient = _context.Patients.Where(x => x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            if (ro != null)
            {
                //var oldLogData = new JavaScriptSerializer().Serialize(ro);
                //ro.isDeleted = 1;
                //var newLogData = new JavaScriptSerializer().Serialize(ro);

                //_context.SaveChanges();

                patientMethod.deleteRoutine(supervisorID, routineID, patientAllocation.patientAllocationID, 1);
                TempData["success"] = "Successfully deleted a routine on " + DateTime.Now + ".";

                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "routine", "isDeleted", null, null, ID, 1, 0, null);

                if (ro.day.Equals(DateTime.Today.DayOfWeek))
                {
                    //generate schedule
                    scheduler.generateWeeklySchedule(false, true);
                }

            }
            else
            {
                TempData["error"] = "Failed to delete a routine.";

            }
            return RedirectToAction("ManageRoutine", "Supervisor", new { id = patientId });

        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ExcludeRoutine(RoutineViewModel item)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            int patientID = item.patient.patientID;
            int routineID = Convert.ToInt32(Request.Form.Get("hiddenRoutineID"));
            var ro = _context.Routines.Where(x => x.routineID == routineID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            var patient = _context.Patients.Where(x => x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            if (ro != null)
            {

                //ubc
                //scheduler.routineUpdateBit(ro.routineID, ro.startDate, ro.endDate, ro.day, ro.startTime, ro.startTime);


                var oLogData = new JavaScriptSerializer().Serialize(patient);
                patient.updateBit = 1;
                _context.SaveChanges();

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                var nLogData = new JavaScriptSerializer().Serialize(patient);


                string[] llogVal = shortcutMethod.GetLogVal(oLogData, nLogData);

                string oLogVal = llogVal[0];
                string nLogVal = llogVal[1];

                shortcutMethod.addLogToDB(oLogData, nLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "patient", "updateBit", oLogVal, nLogVal, patient.patientID, 1, 0, null);


                //routine
                ro.includeInSchedule = 0;
                ro.reasonForExclude = item.routine.reasonForExclude;
                string columnAffected = "includeInSchedule,reasonForExclude";

                _context.SaveChanges();
                TempData["success"] = "Successfully excluded a routine on " + DateTime.Now + ".";

                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                var newLogData = new JavaScriptSerializer().Serialize(ro);

                shortcutMethod.addLogToDB(null, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "routine", columnAffected, null, null, routineID, 1, 0, null);

                //generate schedule
                //_context.SaveChanges();
                //scheduler.generateWeeklySchedule(false,false);

            }
            else
            {
                TempData["error"] = "Failed to exclude a routine.";

            }
            return RedirectToAction("ManageRoutine", "Supervisor", new { id = patientID });

        }



        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult IncludeRoutine(string roID, string patientId)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            int routineID = Convert.ToInt32(roID);

            int patientID = Convert.ToInt32(patientId);
            var ro = _context.Routines.Where(x => x.routineID == routineID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();


            var patient = _context.Patients.Where(x => x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            if (ro != null)
            {
                //ubc
                //scheduler.routineUpdateBit(ro.routineID, ro.startDate, ro.endDate, ro.day, ro.startTime, ro.endTime);


                var oLogData = new JavaScriptSerializer().Serialize(patient);
                patient.updateBit = 1;
                _context.SaveChanges();

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                var nLogData = new JavaScriptSerializer().Serialize(patient);


                string[] llogVal = shortcutMethod.GetLogVal(oLogData, nLogData);

                string oLogVal = llogVal[0];
                string nLogVal = llogVal[1];

                shortcutMethod.addLogToDB(oLogData, nLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "patient", "updateBit", oLogVal, nLogVal, patient.patientID, 1, 0, null);


                ro.includeInSchedule = 1;
                ro.reasonForExclude = null;
                string columnAffected = "includeInSchedule, reasonForExclude";
                _context.SaveChanges();
                TempData["success"] = "Successfully included a routine on " + DateTime.Now + ".";

                var newLogData = new JavaScriptSerializer().Serialize(ro);
                shortcutMethod.addLogToDB(null, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "routine", columnAffected, null, null, routineID, 1, 0, null);


            }
            else
            {
                TempData["error"] = "Failed to include a routine.";

            }
            return RedirectToAction("ManageRoutine", "Supervisor", new { id = patientID });

        }





        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditMedicalHistory(MedicalHistoryViewModel viewModel)
        {
            int medHistoryID = Convert.ToInt32(Request.Form.Get("medHistID"));
            int ID = viewModel.patient.patientID;
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == ID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            var medicalHistory = _context.MedicalHistory.Where(x => x.medicalHistoryID == medHistoryID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            var medDetails = viewModel.medicalHist.medicalDetails;
            var infoSource = viewModel.medicalHist.informationSource;
            var notes = viewModel.medicalHist.notes;
            var EstDate = Request.Form.Get("medicalEstimatedDate");
            var date = DateTime.ParseExact(EstDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            //string columnAffected = "";

            //var oldLogData = new JavaScriptSerializer().Serialize(medicalHistory);

            if (medicalHistory != null)
            {
                string result = patientMethod.updateMedicalHistory(supervisorID, patientAllocation.patientAllocationID, medHistoryID, medDetails, infoSource, notes, date, 1);

                //    if (medicalHistory.medicalDetails != medDetails)
                //    {
                //        medicalHistory.medicalDetails = medDetails;
                //        columnAffected = columnAffected + "medicalDetails,";

                //    }

                //    if (medicalHistory.informationSource != infoSource)
                //    {
                //        medicalHistory.informationSource = infoSource;
                //        columnAffected = columnAffected + "informationSource,";

                //    }

                //    if (medicalHistory.notes != notes)
                //    {
                //        medicalHistory.notes = notes;
                //        columnAffected = columnAffected + "notes,";

                //    }

                //    if (medicalHistory.medicalEstimatedDate != date)
                //    {
                //        medicalHistory.medicalEstimatedDate = date;
                //        columnAffected = columnAffected + "medicalEstimatedDate,";

                //    }


                //    if (columnAffected.EndsWith(","))
                //    {
                //        columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                //    }


                //    if (!columnAffected.Equals(""))
                //    {
                //        _context.SaveChanges();
                //        TempData["success"] = "Changes saved successfully!!";
                //        var newLogData = new JavaScriptSerializer().Serialize(medicalHistory);
                //        string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //        shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "medicalHistory", columnAffected, null, null, medHistoryID, 1, 0, null);
                //    }

                if (result.Contains("success"))
                {
                    TempData["success"] = result;
                }
                else
                {
                    TempData["error"] = result;
                }
            }
            else
            {
                TempData["error"] = "Failed to save changes.";

            }
            return RedirectToAction("ManageMedicalHistory", "Supervisor", new { id = ID });

        }
        //[HttpPost]
        //[Authorize(Roles = RoleName.isSupervisor)]
        //public ActionResult EditMedicationLog1(PrescriptionViewModel viewModel)
        //{
        //    int medLogID = Convert.ToInt32(Request.Form.Get("medicationLogID"));
        //    int ID = viewModel.patient.patientID;
        //    int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
        //    string columnAffected = "";
        //    PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == ID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

        //    var timeForMed = Request.Form.Get("timeForMed1");
        //    var timeTaken = Request.Form.Get("timeTaken1");

        //    var med = _context.MedicationLog.Where(x => x.medicationLogID == medLogID).SingleOrDefault();
        //    var oldLogData = new JavaScriptSerializer().Serialize(med);

        //    if (med != null)
        //    {

        //        if (med.prescriptionID != viewModel.medication.prescriptionID)
        //        {
        //            med.prescriptionID = viewModel.medication.prescriptionID;
        //            columnAffected = columnAffected + "prescriptionID,";

        //        }

        //        if (med.userID != viewModel.medication.userID)
        //        {
        //            med.userID = viewModel.medication.userID;
        //            columnAffected = columnAffected + "userID,";

        //        }

        //        if (med.dateForMedication != viewModel.medication.dateForMedication)
        //        {
        //            med.dateForMedication = viewModel.medication.dateForMedication;
        //            columnAffected = columnAffected + "dateForMedication,";

        //        }
        //        if (med.dateTaken != viewModel.medication.dateTaken)
        //        {
        //            med.dateTaken = viewModel.medication.dateTaken;
        //            columnAffected = columnAffected + "dateTaken,";

        //        }

        //        TimeSpan timeMed = TimeSpan.Parse(timeForMed);
        //        if (med.timeForMedication != timeMed)
        //        {
        //            med.timeForMedication = timeMed;
        //            columnAffected = columnAffected + "timeForMedication,";

        //        }

        //        TimeSpan timeTake = TimeSpan.Parse(timeTaken);

        //        if (med.timeTaken != timeTake)
        //        {
        //            med.timeTaken = timeTake;
        //            columnAffected = columnAffected + "timeTaken,";

        //        }


        //        if (columnAffected.EndsWith(","))
        //        {
        //            columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

        //        }

        //        if (!columnAffected.Equals(""))
        //        {
        //            _context.SaveChanges();
        //            TempData["success"] = "Changes saved successfully!!";
        //            var newLogData = new JavaScriptSerializer().Serialize(med);

        //            string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

        //            string oldLogVal = logVal[0];
        //            string newLogVal = logVal[1];

        //            string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
        //            shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "medicationLog", columnAffected, oldLogVal, newLogVal, medLogID, 1, 0, null);
        //        }
        //    }
        //    else
        //    {
        //        TempData["error"] = "Failed to save changes.";

        //    }

        //    return RedirectToAction("ManagePrescription", "Supervisor", new { id = ID });

        //}

        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditMedicationLog(PrescriptionViewModel viewModel)
        {
            int medLogID = Convert.ToInt32(Request.Form.Get("medicationLogID"));
            int ID = viewModel.patient.patientID;
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            //string columnAffected = "";
            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == ID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            var timeTaken = Request.Form.Get("timeTaken1");
            var userID = viewModel.medication.userID;
            var dateTaken = viewModel.medication.dateTaken;
            TimeSpan timeTake = TimeSpan.Parse(timeTaken);

            var med = _context.MedicationLog.Where(x => x.medicationLogID == medLogID).SingleOrDefault();
            //var oldLogData = new JavaScriptSerializer().Serialize(med);

            if (med != null)
            {
                string result = patientMethod.updateMedicationLog(supervisorID, patientAllocation.patientAllocationID, medLogID, userID, dateTaken, timeTake, 1);

                if (result.Contains("success"))
                {
                    TempData["success"] = result;
                }
                else
                {
                    TempData["error"] = result;
                }
                //    if (med.userID != userID)
                //    {
                //        med.userID = userID;
                //        columnAffected = columnAffected + "userID,";

                //    }


                //    if (med.dateTaken != dateTaken)
                //    {
                //        med.dateTaken = dateTaken;
                //        columnAffected = columnAffected + "dateTaken,";

                //    }



                //    if (med.timeTaken != timeTake)
                //    {
                //        med.timeTaken = timeTake;
                //        columnAffected = columnAffected + "timeTaken,";

                //    }


                //    if (columnAffected.EndsWith(","))
                //    {
                //        columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                //    }

                //    if (!columnAffected.Equals(""))
                //    {
                //        _context.SaveChanges();
                //        TempData["success"] = "Changes saved successfully!!";
                //        var newLogData = new JavaScriptSerializer().Serialize(med);

                //        string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                //        string oldLogVal = logVal[0];
                //        string newLogVal = logVal[1];

                //        string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //        shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "medicationLog", columnAffected, oldLogVal, newLogVal, medLogID, 1, 0, null);
                //    }
            }
            else
            {
                TempData["error"] = "Failed to save changes.";

            }
            return new RedirectResult(Url.Action("ManagePrescription", "Supervisor", new { id = ID }) + "#med");
            //return RedirectToAction("ManagePrescription", "Supervisor", new { id = ID }+ "#med");

        }



        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditPrescription(PrescriptionViewModel viewModel)
        {
            int prescriptionID = Convert.ToInt32(Request.Form.Get("prescriptionID"));
            int ID = viewModel.patient.patientID;
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == ID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            var pscp = _context.Prescriptions.Where(x => x.prescriptionID == prescriptionID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            string logDescList = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

            var stDate = Request.Form.Get("startDate");
            var endDate = Request.Form.Get("endDate");

            var stdate = DateTime.Parse(stDate);
            var enddate = DateTime.Parse(endDate);
            var drugNameID = viewModel.prescription.drugNameID;
            var otherDrugName = viewModel.inputDrugName;
            var isChronic = viewModel.prescription.isChronic;
            var dosage = viewModel.prescription.dosage;
            var frequencyPerDay = viewModel.prescription.frequencyPerDay;
            var instruction = viewModel.prescription.instruction;
            var timeStart = viewModel.prescription.timeStart;
            var notes = viewModel.prescription.notes;
            var mealID = viewModel.mealID;

            //var stdate = DateTime.ParseExact(stDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //var enddate = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            //string columnAffected = "";

            //var oldLogData = new JavaScriptSerializer().Serialize(pscp);

            if (pscp != null)
            {
                string result = patientMethod.updatePrescription(supervisorID, patientAllocation.patientAllocationID, prescriptionID, drugNameID, otherDrugName, isChronic, dosage, frequencyPerDay, instruction, stdate, enddate, timeStart, notes, mealID, 1);

                if (result.Contains("success"))
                {
                    TempData["success"] = result;
                }
                else
                {
                    TempData["error"] = result;
                }
                //    var meal = viewModel.mealID;
                //    if (meal == 0)
                //    {
                //        if (pscp.beforeMeal != 1 || pscp.afterMeal != 0)
                //        {
                //            pscp.beforeMeal = 1;
                //            pscp.afterMeal = 0;
                //            columnAffected = columnAffected + "afterMeal,beforeMeal,";

                //        }
                //    }
                //    else if (meal == 1)
                //    {
                //        if (pscp.afterMeal != 1 || pscp.beforeMeal != 0)
                //        {
                //            pscp.afterMeal = 1;
                //            pscp.beforeMeal = 0;
                //            columnAffected = columnAffected + "afterMeal,beforeMeal,";

                //        }
                //    }


                //    if (drugNameID != -1)
                //    {
                //        if (pscp.drugNameID != drugNameID)
                //        {
                //            pscp.drugNameID = drugNameID;
                //            columnAffected = columnAffected + "drugNameID,";

                //        }
                //    }
                //    else
                //    {
                //        var drug = _context.ListPrescriptions.SingleOrDefault(x => x.value == otherDrugName && x.isDeleted != 1);

                //        if (drug == null)
                //        {
                //            List_Prescription drugList = new List_Prescription();
                //            drugList.value = otherDrugName;
                //            drugList.isChecked = 0;
                //            drugList.createDateTime = DateTime.Now;
                //            _context.ListPrescriptions.Add(drugList);
                //            _context.SaveChanges();

                //            var newDrugLog = new JavaScriptSerializer().Serialize(drugList);
                //            shortcutMethod.addLogToDB(null, newDrugLog, logDescList, 19, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "list_prescription", "ALL", null, null, drugList.list_prescriptionID, 1, 0, null);

                //            pscp.drugNameID = drugList.list_prescriptionID;
                //            columnAffected = columnAffected + "drugNameID,";

                //        }
                //        else
                //        {
                //            if (pscp.drugNameID != drug.list_prescriptionID)
                //            {

                //                pscp.drugNameID = drug.list_prescriptionID;
                //                columnAffected = columnAffected + "drugNameID,";
                //            }


                //        }
                //    }

                //    if (pscp.isChronic != isChronic)
                //    {
                //        pscp.isChronic = isChronic;
                //        columnAffected = columnAffected + "isChronic,";

                //    }

                //    if (pscp.dosage != dosage)
                //    {
                //        pscp.dosage = dosage;
                //        columnAffected = columnAffected + "dosage,";

                //    }

                //    if (pscp.frequencyPerDay != frequencyPerDay)
                //    {
                //        pscp.frequencyPerDay = frequencyPerDay;
                //        columnAffected = columnAffected + "frequencyPerDay,";

                //    }

                //    if (pscp.instruction != instruction)
                //    {
                //        pscp.instruction = instruction;
                //        columnAffected = columnAffected + "instruction,";

                //    }


                //    if (pscp.startDate != stdate)
                //    {
                //        pscp.startDate = stdate;
                //        columnAffected = columnAffected + "startDate,";

                //    }

                //    if (pscp.endDate != enddate)
                //    {
                //        pscp.endDate = enddate;
                //        columnAffected = columnAffected + "endDate,";

                //    }


                //    if (pscp.timeStart != timeStart)
                //    {
                //        pscp.timeStart = timeStart;
                //        columnAffected = columnAffected + "timeStart,";

                //    }


                //    if (pscp.notes != notes)
                //    {
                //        pscp.notes = notes;
                //        columnAffected = columnAffected + "notes,";

                //    }


                //    if (columnAffected.EndsWith(","))
                //    {
                //        columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                //    }

                //    if (!columnAffected.Equals(""))
                //    {
                //        _context.SaveChanges();
                //        TempData["success"] = "Changes saved successfully!!";
                //        var newLogData = new JavaScriptSerializer().Serialize(pscp);

                //        string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                //        string oldLogVal = logVal[0];
                //        string newLogVal = logVal[1];



                //        string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //        shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "prescription", columnAffected, oldLogVal, newLogVal, prescriptionID, 1, 0, null);
                //    }
            }
            else
            {
                TempData["error"] = "Failed to save changes.";

            }
            return new RedirectResult(Url.Action("ManagePrescription", "Supervisor", new { id = ID }) + "#prescription");

            //return RedirectToAction("ManagePrescription", "Supervisor", new { id = ID });

        }



        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditVital(VitalViewModel viewModel)
        {
            int vitalID = Convert.ToInt32(Request.Form.Get("vitalID"));
            int ID = viewModel.patient.patientID;
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == ID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            var vital = _context.Vitals.Where(x => x.vitalID == vitalID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            var temperature = viewModel.vital.temperature;

            var heartRate = viewModel.vital.heartRate;
            var systolicBP = viewModel.vital.systolicBP;
            var diastolicBP = viewModel.vital.diastolicBP;
            var bloodPressure = systolicBP + "/" + diastolicBP;
            var bloodSugarlevel = viewModel.vital.bloodSugarlevel;
            var spO2 = viewModel.vital.spO2;
            var height = viewModel.vital.height;
            var weight = viewModel.vital.weight;
            var notes = viewModel.vital.notes;
            var afterMeal = viewModel.vital.afterMeal;

            //string columnAffected = "";


            //var oldLogData = new JavaScriptSerializer().Serialize(vital);

            if (vital != null)
            {
                string result = patientMethod.updateVital(supervisorID, patientAllocation.patientAllocationID, vitalID, temperature, heartRate, systolicBP, diastolicBP, bloodPressure, bloodSugarlevel, spO2, height, weight, notes, afterMeal, 1);

                if (result.Contains("success"))
                {
                    TempData["success"] = result;
                }
                else
                {
                    TempData["error"] = result;
                }
                //    if (vital.afterMeal != afterMeal)
                //    {
                //        vital.afterMeal = afterMeal;
                //        columnAffected = columnAffected + "afterMeal,";

                //    }
                //    if (vital.temperature != temperature)
                //    {
                //        vital.temperature = temperature;
                //        columnAffected = columnAffected + "temperature,";

                //    }

                //    if (vital.heartRate != heartRate)
                //    {
                //        vital.heartRate = heartRate;
                //        columnAffected = columnAffected + "heartRate,";

                //    }

                //    if (vital.systolicBP != systolicBP)
                //    {
                //        vital.systolicBP = systolicBP;
                //        columnAffected = columnAffected + "systolicBP,";

                //    }
                //    if (vital.diastolicBP != diastolicBP)
                //    {
                //        vital.diastolicBP = diastolicBP;
                //        columnAffected = columnAffected + "diastolicBP,";

                //    }
                //    if (vital.bloodPressure != bloodPressure)
                //    {
                //        vital.bloodPressure = bloodPressure;
                //        columnAffected = columnAffected + "bloodPressure,";

                //    }

                //    if (vital.bloodSugarlevel != bloodSugarlevel)
                //    {
                //        vital.bloodSugarlevel = bloodSugarlevel;
                //        columnAffected = columnAffected + "bloodSugarlevel,";

                //    }
                //    if (vital.spO2 != spO2)
                //    {
                //        vital.spO2 = spO2;
                //        columnAffected = columnAffected + "spO2,";

                //    }
                //    if (vital.height != height)
                //    {
                //        vital.height = height;
                //        columnAffected = columnAffected + "height,";

                //    }
                //    if (vital.weight != weight)
                //    {
                //        vital.weight = weight;
                //        columnAffected = columnAffected + "weight,";

                //    }
                //    if (vital.notes != notes)
                //    {
                //        vital.notes = notes;
                //        columnAffected = columnAffected + "notes,";

                //    }



                //    if (columnAffected.EndsWith(","))
                //    {
                //        columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                //    }

                //    if (!columnAffected.Equals(""))
                //    {
                //        _context.SaveChanges();
                //        TempData["success"] = "Changes saved successfully!!";

                //        var newLogData = new JavaScriptSerializer().Serialize(vital);

                //        string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                //        string oldLogVal = logVal[0];
                //        string newLogVal = logVal[1];

                //        string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //        shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "vital", columnAffected, oldLogVal, newLogVal, vitalID, 1, 0, null);
            
            }
            else
            {
                TempData["error"] = "Failed to save changes.";

            }
            return RedirectToAction("ManageVital", "Supervisor", new { id = ID });

        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditPersoPreference(PersonalPreferenceViewModel viewModel)
        {
            int ID = viewModel.patient.patientID;
            var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == ID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            int likeID = Convert.ToInt32(Request.Form.Get("likeID"));
            int dislikeID = Convert.ToInt32(Request.Form.Get("dislikeID"));

            var likePref = _context.Likes.Where(x => x.likeID == likeID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            var dislikePref = _context.Dislikes.Where(x => x.dislikeID == dislikeID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            var socialHist = _context.SocialHistories.Where(x => x.patientAllocationID == patientAllocation.patientAllocationID).FirstOrDefault();



            if (viewModel.preference.Equals("like"))
            {

                if (viewModel.likes != null)
                {

                    if (likePref != null)
                    {
                        var likeName = viewModel.otherPreferences;
                        var likeListID = viewModel.likes.likeItemID;

                        List_Like likeList = _context.ListLikes.SingleOrDefault(x => (x.list_likeID == likeListID && x.isChecked == 1 && x.isDeleted != 1));

                        if (likeList == null)
                        {
                            List_Like newLikeList = new List_Like
                            {
                                value = likeName,
                                isChecked = 0,
                                isDeleted = 0,
                                createDateTime = DateTime.Now
                            };
                            _context.ListLikes.Add(newLikeList);
                            _context.SaveChanges();

                            likeListID = newLikeList.list_likeID;

                            var logData = new JavaScriptSerializer().Serialize(newLikeList);
                            string logDescL = "New list item";
                            var logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDescL && x.isDeleted != 1)).logCategoryID;

                            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                            shortcutMethod.addLogToDB(null, logData, logDescL, logCategoryID, patientAllocation.patientAllocationID, supervisorID, null, null, null, null, "list_like", "ALL", null, null, likeListID, 1, 0, null);
                        }

                        var oldLogData = new JavaScriptSerializer().Serialize(likePref);

                        likePref.likeItemID = likeListID;
                        var newLogData = new JavaScriptSerializer().Serialize(likePref);

                        _context.SaveChanges();
                        TempData["success"] = "Changes saved successfully " + DateTime.Now;

                        string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                        string oldLogVal = logVal[0];
                        string newLogVal = logVal[1];


                        string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                        shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "like", "likeItemID", oldLogVal, newLogVal, likeID, 1, 0, null);
                    }
                    else if (dislikePref != null)
                    {

                        patientMethod.deleteDislike(supervisorID, dislikeID, patientAllocation.patientAllocationID, 1);
                        //var oLogData = new JavaScriptSerializer().Serialize(dislikePref);
                        //dislikePref.isDeleted = 1;
                        //var nLogData = new JavaScriptSerializer().Serialize(dislikePref);

                        //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                        //shortcutMethod.addLogToDB(oLogData, nLogData, logDesc, 18, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "dislike", "isDeleted", null, null, likeID, 1, 0, null);

                        //_context.SaveChanges();

                        patientMethod.addLike(supervisorID, patientAllocation.patientAllocationID, viewModel.likes.likeItemID, viewModel.otherPreferences, 1);

                        //Like likes = new Like();
                        //likes.socialHistoryID = socialHist.socialHistoryID;
                        //likes.likeItemID = viewModel.likes.likeItemID;
                        //likes.isApproved = 1;
                        //likes.createdDateTime = DateTime.Now;

                        //_context.Likes.Add(likes);
                        //_context.SaveChanges();
                        TempData["success"] = "Changes saved successfully " + DateTime.Now;
                        //logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                        //var newLogData = new JavaScriptSerializer().Serialize(likes);
                        //shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "like", "ALL", null, null, likeID, 1, 0, null);


                    }

                }
                else
                {
                    TempData["error"] = "Failed to add new preferences.";

                }
            }
            else if (viewModel.preference.Equals("dislike"))
            {
                if (viewModel.dislikes != null)
                {

                    if (dislikePref != null)
                    {

                        var dislikeName = viewModel.otherPreferences;
                        var dislikeListID = viewModel.dislikes.dislikeItemID;

                        List_Dislike dislikeList = _context.ListDislikes.SingleOrDefault(x => (x.list_dislikeID == dislikeListID && x.isChecked == 1 && x.isDeleted != 1));

                        if (dislikeList == null)
                        {
                            List_Dislike newDislikeList = new List_Dislike
                            {
                                value = dislikeName,
                                isChecked = 0,
                                isDeleted = 0,
                                createDateTime = DateTime.Now
                            };
                            _context.ListDislikes.Add(newDislikeList);
                            _context.SaveChanges();

                            dislikeListID = newDislikeList.list_dislikeID;

                            var logData = new JavaScriptSerializer().Serialize(newDislikeList);
                            string logDescL = "New list item";
                            var logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDescL && x.isDeleted != 1)).logCategoryID;

                            shortcutMethod.addLogToDB(null, logData, logDescL, logCategoryID, patientAllocation.patientAllocationID, supervisorID, null, null, null, null, "list_dislike", "ALL", null, null, dislikeListID, 1, 0, null);
                        }

                        var oldLogData = new JavaScriptSerializer().Serialize(dislikePref);

                        dislikePref.dislikeItemID = dislikeListID;
                        var newLogData = new JavaScriptSerializer().Serialize(dislikePref);

                        _context.SaveChanges();

                        string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                        string oldLogVal = logVal[0];
                        string newLogVal = logVal[1];

                        string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                        shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "dislike", "dislikeItemID", oldLogVal, newLogVal, dislikeID, 1, 0, null);
                    }
                    else if (likePref != null)
                    {

                        patientMethod.deleteLike(supervisorID, likeID, patientAllocation.patientAllocationID, 1);
                        //var oLogData = new JavaScriptSerializer().Serialize(likePref);
                        //likePref.isDeleted = 1;
                        //var nLogData = new JavaScriptSerializer().Serialize(likePref);

                        //_context.SaveChanges();
                        //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                        //shortcutMethod.addLogToDB(oLogData, nLogData, logDesc, 18, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "like", "isDeleted", null, null, dislikeID, 1, 0, null);

                        patientMethod.addDislike(supervisorID, patientAllocation.patientAllocationID, viewModel.dislikes.dislikeItemID, viewModel.otherPreferences, 1);

                        //Dislike dislikes = new Dislike();
                        //dislikes.socialHistoryID = socialHist.socialHistoryID;
                        //dislikes.dislikeItemID = viewModel.dislikes.dislikeItemID;
                        //dislikes.isApproved = 1;
                        //dislikes.createdDateTime = DateTime.Now;

                        //_context.Dislikes.Add(dislikes);
                        //_context.SaveChanges();
                        TempData["success"] = "Changes saved successfully " + DateTime.Now;
                        //logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                        //var newLogData = new JavaScriptSerializer().Serialize(dislikes);
                        //shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "dislike", "ALL", null, null, dislikeID, 1, 0, null);
                    }

                }
                else
                {
                    TempData["error"] = "Failed to add new preferences.";

                }
            }
            return RedirectToAction("ManagePersoPreference", "Supervisor", new { id = viewModel.patient.patientID });
        }



        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditAllergy(AllergyViewModel viewModel)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();




            int allergyID = Convert.ToInt32(Request.Form.Get("allergyID"));
            int ID = viewModel.patient.patientID;
            var al = _context.Allergies.Where(x => x.allergyID == allergyID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            var allergyListID = viewModel.allergyInput.allergyListID;
            var notes = viewModel.allergyInput.notes;
            var reaction = viewModel.allergyInput.reaction;
            var otherAllergy = viewModel.otherAllergy;

            //var oldLogData = new JavaScriptSerializer().Serialize(al);
            //string columnAffected = "";

            if (al != null)
            {
               string result = patientMethod.updateAllergy(supervisorID, patientAllocation.patientAllocationID, allergyID, allergyListID, otherAllergy, notes, reaction, 1);
                if (result.Contains("success"))
                {
                    TempData["success"] = result;
                }
                else
                {
                    TempData["error"] = result;
                }
                //    if (allergyListID != -1)
                //    {
                //        if (al.allergyListID != allergyListID)
                //        {
                //            al.allergyListID = allergyListID;
                //            columnAffected = columnAffected + "allergyListID,";

                //        }
                //    }
                //    else
                //    {
                //        var allergyName = _context.ListAllergy.SingleOrDefault(x => x.value == otherAllergy && x.isDeleted != 1);

                //        if (allergyName == null)
                //        {
                //            List_Allergy allergyList = new List_Allergy();
                //            allergyList.value = otherAllergy;
                //            allergyList.isChecked = 0;
                //            allergyList.createDateTime = DateTime.Now;
                //            _context.ListAllergy.Add(allergyList);
                //            _context.SaveChanges();

                //            var nLogData = new JavaScriptSerializer().Serialize(allergyList);
                //            string logDescList = _context.LogCategories.Where(x => x.logCategoryID == 19 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                //            shortcutMethod.addLogToDB(null, nLogData, logDescList, 19, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "list_allergy", "ALL", null, null, allergyList.list_allergyID, 1, 0, null);

                //            al.allergyListID = allergyList.list_allergyID;
                //            columnAffected = columnAffected + "allergyListID,";

                //        }
                //        else
                //        {
                //            if (al.allergyListID != allergyName.list_allergyID)
                //            {
                //                al.allergyListID = allergyName.list_allergyID;
                //                columnAffected = columnAffected + "allergyListID,";
                //            }


                //        }
                //    }


                //    if (al.notes != notes)
                //    {
                //        al.notes = notes;
                //        columnAffected = columnAffected + "notes,";

                //    }

                //    if (al.reaction != reaction)
                //    {
                //        al.reaction = reaction;
                //        columnAffected = columnAffected + "reaction,";

                //    }


                //    if (columnAffected.EndsWith(","))
                //    {
                //        columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                //    }

                //    if (!columnAffected.Equals(""))
                //    {
                //        _context.SaveChanges();
                //        TempData["success"] = "Changes saved successfully!!";
                //        var newLogData = new JavaScriptSerializer().Serialize(al);

                //        string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                //        string oldLogVal = logVal[0];
                //        string newLogVal = logVal[1];

                //        string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //        shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "allergy", columnAffected, oldLogVal, newLogVal, al.allergyID, 1, 0, null);
                //    }
            }
            else
            {
                TempData["error"] = "Failed to save changes.";

            }
            return RedirectToAction("ManageAllergy", "Supervisor", new { id = ID });

        }

        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult RemovePatient(ManageSupervisorsViewModel viewModel)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            int patientID = Convert.ToInt32(Request.Form.Get("hiddenPatientID"));
            var patient = _context.Patients.Where(x => x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1 && x.isActive == 1).SingleOrDefault();

            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();


            var oldLogData = new JavaScriptSerializer().Serialize(patient);
            string columnAffected = "";

            var removalType = Convert.ToInt32(viewModel.removalType);

            if (patient != null)
            {
                //patient.isActive = 0;
                //columnAffected = "isActive, ";

                patient.updateBit = 1;

                //ubc
                //scheduler.inactivePatientUpdateBit(patient.patientID);

                if (removalType == 1)
                {
                    patient.inactiveReason = viewModel.removalReason;
                    columnAffected = columnAffected + "inactiveReason,";

                    //inactiveDate 
                    if (viewModel.inactiveDate >= DateTime.Today && viewModel.inactiveDate <= patient.endDate)
                    {
                        patient.inactiveDate = viewModel.inactiveDate;
                        columnAffected = columnAffected + "inactiveDate,";
                    }
                    else if (viewModel.inactiveDate < DateTime.Today)
                    {
                        TempData["error"] = "Failed to save changes. <br/> Date cannot be before today.";
                        return RedirectToAction("ManagePatient", "Supervisor");

                    }
                    else if (patient.endDate != null && viewModel.inactiveDate > patient.endDate)
                    {
                        TempData["error"] = "Failed to save changes. <br/> Inactive date cannot be after termination date.";
                        return RedirectToAction("ManagePatient", "Supervisor");

                    }

                }
                else if (removalType == 2)
                {

                    patient.endDate = DateTime.Today.AddDays(1);
                    columnAffected = columnAffected + "endDate,";

                    patient.inactiveReason = "Terminated";
                    columnAffected = columnAffected + "inactiveReason,";

                    patient.terminationReason = viewModel.removalReason;
                    columnAffected = columnAffected + "terminationReason,";

                }

                if (columnAffected.EndsWith(","))
                {
                    columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                }



                _context.SaveChanges();
                TempData["success"] = "Changes saved successfully!!";
                var newLogData = new JavaScriptSerializer().Serialize(patient);

                string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                string oldLogVal = logVal[0];
                string newLogVal = logVal[1];

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "patient", columnAffected, oldLogVal, newLogVal, patientID, 1, 0, null);
                //public void addLogToDB(string oldLogData, string logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, string additionalInfo, string remarks, string tableAffected, string columnAffected, string logOldValue, string logNewValue, int? rowAffected, int approved, int userNotified, string rejectReason)

                //generate schedule
                //scheduler.generateWeeklySchedule(false, false);

            }
            else
            {
                TempData["error"] = "Failed to save changes.";

            }
            return RedirectToAction("ManagePatient", "Supervisor");

        }



        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditProblemLogDetails(ProblemLogViewModel viewModel)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            int problemLogID = Convert.ToInt32(Request.Form.Get("problemLogID"));
            int ID = viewModel.patient.patientID;
            var pl = _context.ProblemLogs.Where(x => x.problemLogID == problemLogID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            var categoryID = viewModel.problemLog.categoryID;
            var notes = viewModel.problemLog.notes;

            //var oldLogData = new JavaScriptSerializer().Serialize(pl);
            //string columnAffected = "";

            if (pl != null)
            {
               string result = patientMethod.updateProblemLog(supervisorID, patientAllocation.patientAllocationID, problemLogID, categoryID, notes, 1);
                //if (pl.categoryID != categoryID)
                //{
                //    pl.categoryID = categoryID;
                //    columnAffected = columnAffected + "categoryID,";

                //}

                //if (pl.notes != notes)
                //{
                //    pl.notes = notes;
                //    columnAffected = columnAffected + "notes,";

                //}

                //pl.userID = supervisorID;


                //if (columnAffected.EndsWith(","))
                //{
                //    columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                //}

                //if (!columnAffected.Equals(""))
                //{
                //    _context.SaveChanges();
                //    TempData["success"] = "Changes saved successfully!!";
                //    var newLogData = new JavaScriptSerializer().Serialize(pl);

                //    string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                //    string oldLogVal = logVal[0];
                //    string newLogVal = logVal[1];

                //    string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "problemLog", columnAffected, oldLogVal, newLogVal, problemLogID, 1, 0, null);
                //    //public void addLogToDB(string oldLogData, string logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, string additionalInfo, string remarks, string tableAffected, string columnAffected, string logOldValue, string logNewValue, int? rowAffected, int approved, int userNotified, string rejectReason)
                //}

                if (result.Contains("success"))
                {
                    TempData["success"] = result;
                }
                else {
                    TempData["error"] = result;
                }
            }
            else
            {
                TempData["error"] = "Failed to save changes.";

            }
            return RedirectToAction("ManageProblemLog", "Supervisor", new { id = ID });

        }

        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditActivityExclusion(ManagePreferencesViewModel viewModel)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            int exActivityID = Convert.ToInt32(Request.Form.Get("exActivityID"));
            int ID = viewModel.patient.patientID;
            var dateTimeStart = viewModel.actExInput.dateTimeStart;
            var dateTimeEnd = viewModel.actExInput.dateTimeEnd;
            var notes = viewModel.actExInput.notes;

            var exAct = _context.ActivityExclusions.Where(x => x.activityExclusionId == exActivityID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            var oldLogData = new JavaScriptSerializer().Serialize(exAct);
            //string columnAffected = "";

            string logDescU = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

            if (exAct != null)
            {

                //ubc
                //scheduler.exclusionUpdateBit(exActivityID);


                //patient
                var p = _context.Patients.SingleOrDefault(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 && x.isDeleted != 1);
                var opLogData = new JavaScriptSerializer().Serialize(p);
                p.updateBit = 1;
                var npLogData = new JavaScriptSerializer().Serialize(p);

                string[] llogVal = shortcutMethod.GetLogVal(opLogData, npLogData);
                string poldLogVal = llogVal[0];
                string pnewLogVal = llogVal[1];
                shortcutMethod.addLogToDB(opLogData, npLogData, logDescU, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "patient", "updateBit", poldLogVal, pnewLogVal, p.patientID, 1, 0, null);


                string result = patientMethod.updateActivityExclusion(supervisorID, patientAllocation.patientAllocationID, exActivityID, dateTimeStart, dateTimeEnd, notes, 1);

                if (result.Contains("success"))
                {
                    TempData["success"] = result;
                }
                else
                {
                    TempData["error"] = result;
                }
                //int flag = 0;
                ////exAct
                //if (exAct.dateTimeStart != dateTimeStart)
                //{
                //    exAct.dateTimeStart = dateTimeStart;
                //    columnAffected = "dateTimeStart,";
                //    flag = 1;
                //}


                //if (exAct.dateTimeEnd != dateTimeEnd)
                //{
                //    exAct.dateTimeEnd = dateTimeEnd;
                //    columnAffected = columnAffected + "dateTimeEnd,";
                //    flag = 1;
                //}

                //if (exAct.notes != notes)
                //{
                //    exAct.notes = notes;
                //    columnAffected = columnAffected + "notes,";
                //    flag = 1;
                //}

                //if (columnAffected.EndsWith(","))
                //{
                //    columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                //}

                //if (flag == 1)
                //{

                //    _context.SaveChanges();




                //exAct
                //TempData["success"] = "Changes saved successfully!!";
                //var newLogData = new JavaScriptSerializer().Serialize(exAct);
                //string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);
                //string oldLogVal = logVal[0];
                //string newLogVal = logVal[1];

                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //shortcutMethod.addLogToDB(oldLogData, newLogData, logDescU, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "activityExclusion", columnAffected, oldLogVal, newLogVal, exAct.activityExclusionId, 1, 0, null);
                //public void addLogToDB(string oldLogData, string logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, string additionalInfo, string remarks, string tableAffected, string columnAffected, string logOldValue, string logNewValue, int? rowAffected, int approved, int userNotified, string rejectReason)


                //generate schedule
                //scheduler.generateWeeklySchedule(false, false);

                //}
            }
            else
            {
                TempData["error"] = "Failed to save changes.";

            }
            //return RedirectToAction("ManagePreference", "Supervisor", new { id = ID });

            var referral = Request.UrlReferrer.ToString();

            if (referral.Contains("ManagePreference"))
            {

                return RedirectToAction("ManagePreference", "Supervisor", new { id = ID });
            }
            else
            {
                return RedirectToAction("ManagePatientPreference", "Supervisor");

            }

        }





        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditAllocationDetails(AllocationViewModel viewModel)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            Patient patient = _context.Patients.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();



            string allocationType = Request.Form.Get("allocationType");
            //int patientID = Convert.ToInt32(Request.Form.Get("patientID"));
            int ID = viewModel.patient.patientID;



            var oldLogData = new JavaScriptSerializer().Serialize(patientAllocation);
            string columnAffected = "";

            if (patientAllocation != null)
            {
                if (allocationType.Equals("assigned"))
                {
                    if (viewModel.usertypeID == 2)
                    {
                        if (patientAllocation.caregiverID != viewModel.allocatedCaregiver.caregiverID)
                        {
                            patientAllocation.caregiverID = viewModel.allocatedCaregiver.caregiverID;
                            columnAffected = columnAffected + "caregiverID ";

                        }
                    }

                    if (viewModel.usertypeID == 3)
                    {
                        if (patientAllocation.doctorID != viewModel.allocatedDoctor.doctorID)
                        {
                            patientAllocation.doctorID = viewModel.allocatedDoctor.doctorID;
                            columnAffected = columnAffected + "doctorID";

                        }
                    }

                }
                else if (allocationType.Equals("temp"))
                {
                    if (viewModel.usertypeID == 2)
                    {
                        if (patientAllocation.tempCaregiverID != viewModel.allocatedCaregiver.caregiverID)
                        {
                            patientAllocation.tempCaregiverID = viewModel.allocatedCaregiver.caregiverID;
                            columnAffected = columnAffected + "tempCaregiverID";

                        }
                    }

                    if (viewModel.usertypeID == 3)
                    {
                        if (patientAllocation.tempDoctorID != viewModel.allocatedDoctor.doctorID)
                        {
                            patientAllocation.tempDoctorID = viewModel.allocatedDoctor.doctorID;
                            columnAffected = columnAffected + "tempDoctorID";

                        }
                    }

                }

                var opLogData = new JavaScriptSerializer().Serialize(patient);
                patient.updateBit = 1;
                var npLogData = new JavaScriptSerializer().Serialize(patient);

                string[] llogVal = shortcutMethod.GetLogVal(opLogData, npLogData);
                string poldLogVal = llogVal[0];
                string pnewLogVal = llogVal[1];

                string logDescU = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                shortcutMethod.addLogToDB(opLogData, npLogData, logDescU, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "patient", "updateBit", poldLogVal, pnewLogVal, patient.patientID, 1, 0, null);



                _context.SaveChanges();
                TempData["success"] = "Changes saved successfully!!";
                var newLogData = new JavaScriptSerializer().Serialize(patientAllocation);

                string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                string oldLogVal = logVal[0];
                string newLogVal = logVal[1];

                shortcutMethod.addLogToDB(oldLogData, newLogData, logDescU, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "patientAllocation", columnAffected, oldLogVal, newLogVal, patientAllocation.patientAllocationID, 1, 0, null);

            }
            else
            {
                TempData["error"] = "Failed to save changes.";

            }
            return RedirectToAction("ManageAllocation", "Supervisor", new { id = ID });

        }

        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditRoutine(RoutineViewModel viewModel)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            Patient patient = _context.Patients.Where(x => x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            int routineID = Convert.ToInt32(Request.Form.Get("routineID"));
            int ID = viewModel.patient.patientID;
            var ro = _context.Routines.Where(x => x.routineID == routineID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            var startDate = (Request.Form.Get("startDate"));
            var endDate = (Request.Form.Get("endDate"));
            var stdate = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var edate = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var notes = viewModel.routine.notes;
            var concerningIssues = viewModel.routine.concerningIssues;
            var centreActivityID = viewModel.routine.centreActivityID;
            var eventName = viewModel.routine.eventName;
            var day = viewModel.routine.day;
            var startTime = viewModel.routine.startTime;
            var endTime = viewModel.routine.endTime;


            //  var clashRoutine = _context.Routines.Where(x => (x.startDate >= viewModel.routine.endDate || viewModel.routine.startDate <= x.endDate) &&
            //(x.startTime >= viewModel.routine.endTime || viewModel.routine.startTime <= x.endTime)
            //&& x.isApproved == 1 && x.isDeleted != 1).ToList();

            //var oldLogData = new JavaScriptSerializer().Serialize(ro);
            //string columnAffected = "";

            //var flag = 0;

            if (ro != null)
            {
                string result = patientMethod.updateRoutine(supervisorID, patientAllocation.patientAllocationID, routineID, stdate, edate, concerningIssues, centreActivityID, eventName, notes, day, startTime, endTime, 1);
                //if (clashRoutine.Count() == 0 && viewModel.routine.includeInSchedule == 1)
                //{

                //if excluded

                //if (viewModel.routine.includeInSchedule != 1)
                //{
                //    if (ro.reasonForExclude != viewModel.routine.reasonForExclude)
                //    {
                //        ro.reasonForExclude = viewModel.routine.reasonForExclude;
                //        columnAffected = columnAffected + "reasonForExclude, ";
                //        flag = 1;

                //    }
                //}
              

                //check for changes
                //if (ro.includeInSchedule != viewModel.routine.includeInSchedule)
                //{
                //    ro.includeInSchedule = viewModel.routine.includeInSchedule;
                //    columnAffected = columnAffected + "includeInSchedule, ";
                //    flag = 1;

                //}
                int schedBit = 0;
                if (ro.day.Equals(DateTime.Today.DayOfWeek))
                {
                    schedBit = 1;
                }

                //if (ro.notes != notes)
                //{
                //    ro.notes = notes;
                //    columnAffected = columnAffected + "notes,";
                //    flag = 1;

                //}

                //if (ro.startDate != stdate)
                //{
                //    ro.startDate = stdate;
                //    columnAffected = columnAffected + "startDate,";
                //    flag = 1;

                //}
                //if (ro.endDate != edate)
                //{
                //    ro.endDate = edate;
                //    columnAffected = columnAffected + "endDate,";
                //    flag = 1;

                //}

                //if (ro.concerningIssues != concerningIssues)
                //{
                //    ro.concerningIssues = concerningIssues;
                //    columnAffected = columnAffected + "concerningIssues,";
                //    flag = 1;

                //}


                //if not "others"
                //if (centreActivityID != -1)
                //{
                //    if (ro.centreActivityID != centreActivityID)
                //    {
                //        ro.centreActivityID = centreActivityID;
                //        columnAffected = columnAffected + "centreActivityID,";
                //        flag = 1;

                //    }

                //    //get activity name
                //    var activityName = _context.CentreActivities.Where(x => x.centreActivityID == centreActivityID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault().activityTitle;
                //    ro.eventName = activityName;
                //}
                //else
                //{
                //    if (ro.eventName != eventName)
                //    {
                //        ro.eventName = eventName;
                //        columnAffected = columnAffected + "eventName,";
                //        flag = 1;

                //    }
                //}

                //if (ro.day != day)
                //{
                //    ro.day = day;
                //    columnAffected = columnAffected + "day,";
                //    flag = 1;

                //}

                //if (ro.startTime != startTime)
                //{
                //    ro.startTime = startTime;
                //    columnAffected = columnAffected + "startTime,";
                //    flag = 1;

                //}
                //if (ro.endTime != endTime)
                //{
                //    ro.endTime = endTime;
                //    columnAffected = columnAffected + "endTime,";
                //    flag = 1;

                //}


                //if (columnAffected.EndsWith(","))
                //{
                //    columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                //}

                //if (flag == 1)
                //{

                //    //patient
                //    var opLogData = new JavaScriptSerializer().Serialize(patient);
                //    patient.updateBit = 1;
                //    var npLogData = new JavaScriptSerializer().Serialize(patient);

                //    string[] llogVal = shortcutMethod.GetLogVal(opLogData, npLogData);
                //    string poldLogVal = llogVal[0];
                //    string pnewLogVal = llogVal[1];

                //    string logDescU = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //    shortcutMethod.addLogToDB(opLogData, npLogData, logDescU, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "patient", "updateBit", poldLogVal, pnewLogVal, patient.patientID, 1, 0, null);

                //    //ubc
                //    //scheduler.routineUpdateBit(ro.routineID, ro.startDate, ro.endDate, ro.day, ro.startTime, ro.endTime);


                //}

                //if (!columnAffected.Equals(""))
                //{
                //    _context.SaveChanges();
                //    TempData["success"] = "Changes saved successfully!!";
                //    var newLogData = new JavaScriptSerializer().Serialize(ro);

                //    string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                //    string oldLogVal = logVal[0];
                //    string newLogVal = logVal[1];

                //    string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "routine", columnAffected, oldLogVal, newLogVal, routineID, 1, 0, null);
                //}
                if (result.Contains("success"))
                {
                    TempData["success"] = result;
                    if (schedBit == 1)
                    {
                        scheduler.generateWeeklySchedule(false, true);
                    }
                }
                else
                {
                    TempData["error"] = result;
                }

                
                //}
                //else {
                //    TempData["error"] = "Failed to save changes.<br/>Routine Clashes.";

                //}
            }
            else
            {
                TempData["error"] = "Failed to save changes.";

            }
            return RedirectToAction("ManageRoutine", "Supervisor", new { id = ID });

        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetMedicalHistory(int id)
        {
            var medicalHistory = _context.MedicalHistory.Where(x => x.medicalHistoryID == id && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            return Json(medicalHistory);

        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetAllergy(int id)
        {

            var patientAllergy = (from al in _context.Allergies
                                  where al.isDeleted != 1
                                  where al.isApproved == 1
                                  where al.allergyID == id

                                  select new PatientAllergy
                                  {
                                      allergy = al,
                                      allergyCheck = _context.ListAllergy.Where(x => x.list_allergyID == al.allergyListID && x.isDeleted != 1).FirstOrDefault(),

                                  }).SingleOrDefault();



            return Json(patientAllergy);

        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetProblemLogDetails(int id)
        {
            var problemLog = _context.ProblemLogs.Where(x => x.problemLogID == id && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            return Json(problemLog);

        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetDementiaDetails(int id)
        {
            var pad = _context.PatientAssignedDementias.Where(x => x.padID == id && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            return Json(pad);

        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetAvailabilityDetails(int id)
        {
            var availability = _context.ActivityAvailabilities.Where(x => x.activityAvailabilityID == id && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            return Json(availability);

        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetAllocationDetails(int id)
        {
            var patientAllocated = _context.PatientAllocations.Where(x => x.patientID == id && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            var patientCaregiver = (from p in _context.Patients
                                    join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                    join u in _context.Users on pa.caregiverID equals u.userID
                                    //join ut in _context.UserTypes on u.userTypeID equals ut.userTypeID
                                    //where ut.isDeleted != 1
                                    //where ut.userTypeName == "Caregiver"
                                    where pa.isApproved == 1 && pa.isDeleted != 1
                                    where p.patientID == id
                                    where p.isApproved == 1 && p.isDeleted != 1
                                    select new UserViewModel
                                    {
                                        userFullname = u.firstName + " " + u.lastName,
                                        userID = u.userID,

                                    }).SingleOrDefault();


            var patientDoctor = (from p in _context.Patients
                                 join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                 join u in _context.Users on pa.doctorID equals u.userID
                                 //join ut in _context.UserTypes on u.userTypeID equals ut.userTypeID
                                 //where ut.isDeleted != 1
                                 //where ut.userTypeName == "Doctor"
                                 where pa.isApproved == 1 && pa.isDeleted != 1
                                 where p.patientID == id
                                 where p.isApproved == 1 && p.isDeleted != 1
                                 select new UserViewModel
                                 {
                                     userFullname = u.firstName + " " + u.lastName,
                                     userID = u.userID,

                                 }).SingleOrDefault();

            var tempCaregiver = (from p in _context.Patients
                                 join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                 join u in _context.Users on pa.tempCaregiverID equals u.userID
                                 where pa.isApproved == 1 && pa.isDeleted != 1
                                 where p.patientID == id
                                 where p.isApproved == 1 && p.isDeleted != 1
                                 //select u.userID
                                 select new UserViewModel
                                 {
                                     userFullname = u.firstName + " " + u.lastName,
                                     userID = u.userID,

                                 }

              ).SingleOrDefault();




            var tempDoctor = (from p in _context.Patients
                              join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                              join u in _context.Users on pa.tempDoctorID equals u.userID
                              where pa.isApproved == 1 && pa.isDeleted != 1
                              where p.patientID == id
                              where p.isApproved == 1 && p.isDeleted != 1
                              //select u.userID
                              select new UserViewModel
                              {
                                  userFullname = u.firstName + " " + u.lastName,
                                  userID = u.userID,

                              }

                ).SingleOrDefault();

            var patient = _context.Patients.Where(x => x.patientID == id && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            var viewModel = new AllocationViewModel
            {
                assignedCaregiver = patientCaregiver,
                assignedDoctor = patientDoctor,
                patient = patient,
                tempCaregiverInfo = tempCaregiver,
                tempDoctorInfo = tempDoctor,
            };

            return Json(viewModel);

        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetRoutineDetails(int id)
        {
            var routine = _context.Routines.Where(x => x.routineID == id && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            return Json(routine);

        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetCentreHours(string day)
        {
            var centreHour = _context.CareCentreHours.Where(x => x.centreWorkingDay == day && x.centreID == 1 && x.isDeleted != 1).SingleOrDefault();
            return Json(centreHour);

        }



        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetVital(int id)
        {
            var vital = _context.Vitals.Where(x => x.vitalID == id && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            return Json(vital);

        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetVitalForHighlights(int id, string category)
        {

            //Threshold
            var tempthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "temperature").SingleOrDefault();
            var bpSystolicthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "systolicBP").SingleOrDefault();
            var bpDiastolicthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "diastolicBP").SingleOrDefault();
            var spO2threshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "spO2").SingleOrDefault();

            var bslBthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "bslBeforeMeal").SingleOrDefault();
            var bslAthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "bslAfterMeal").SingleOrDefault();

            var hrthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "heartRate").SingleOrDefault();


            //vital
            var vital = _context.Vitals.Where(x => x.vitalID == id && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            string message = "";

            if (category.Equals("Temperature"))
            {
                if (vital.temperature >= tempthreshold.maxValue)
                {
                    message = "Temperature above normal level";
                }
                else if (vital.temperature < tempthreshold.minValue)
                {

                    message = "Temperature below normal level";
                }

            }

            if (category.Equals("Blood Pressure"))
            {
                if (vital.systolicBP > bpSystolicthreshold.maxValue || vital.diastolicBP > bpDiastolicthreshold.maxValue)
                {
                    message = "Blood Pressure above normal level";
                }
                else if (vital.systolicBP < bpSystolicthreshold.minValue || vital.diastolicBP < bpDiastolicthreshold.minValue)
                {

                    message = "Blood Pressure below normal level";

                }
            }

            if (category.Equals("spO2"))
            {

                if (vital.spO2 > spO2threshold.maxValue)
                {
                    message = "spO2 above normal level";

                }
                else if (vital.spO2 < spO2threshold.minValue)
                {

                    message = "spO2 below normal level";

                }
            }

            if (category.Equals("Blood Sugar Level"))
            {

                if (vital.afterMeal == 1)
                {
                    if (vital.bloodSugarlevel > bslAthreshold.maxValue)
                    {

                        message = "Blood Sugar Level above normal level";

                    }
                    else if (vital.bloodSugarlevel < bslAthreshold.minValue)
                    {

                        message = "Blood Sugar Level below normal level";

                    }
                }
                else
                {
                    if (vital.bloodSugarlevel > bslBthreshold.maxValue)
                    {

                        message = "Blood Sugar Level above normal level";

                    }
                    else if (vital.bloodSugarlevel < bslBthreshold.minValue)
                    {

                        message = "Blood Sugar Level below normal level";

                    }
                }
            }

            if (category.Equals("Heart Rate"))
            {

                if (vital.heartRate > hrthreshold.maxValue)
                {

                    message = "Heart Rate above normal level";

                }
                else if (vital.heartRate < hrthreshold.minValue)
                {

                    message = "Heart Rate below normal level";

                }

            }



            var vitalViewModel = new GetVitalViewModel()
            {

                vital = vital,
                vitalDescription = message,
                category = category,

            };


            return Json(vitalViewModel);


        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetPrescription(int id)
        {
            //var prescription = _context.Prescriptions.Where(x => x.prescriptionID == id && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();


            var patientPrescription = (from pscp in _context.Prescriptions
                                       where pscp.isDeleted != 1
                                       where pscp.isApproved == 1
                                       where pscp.prescriptionID == id

                                       select new PatientPrescription
                                       {
                                           prescription = pscp,
                                           prescriptionCheck = _context.ListPrescriptions.Where(x => x.list_prescriptionID == pscp.drugNameID && x.isDeleted != 1).FirstOrDefault(),
                                           //prescriptionList = _context.Prescriptions.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault(),

                                       }).SingleOrDefault();


            return Json(patientPrescription);

        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetLikePreference(int id)
        {

            var patientPrefLike = new PatientPrefLike
            {

                likesDetails = _context.Likes.Where(x => x.likeID == id && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault(),
                likes = _context.ListLikes.Where(x => x.list_likeID == id && x.isDeleted != 1).FirstOrDefault(),
            };


            return Json(patientPrefLike);

        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetDislikePreference(int id)
        {

            var patientPrefDislike = new PatientPrefDislike
            {

                dislikesDetails = _context.Dislikes.Where(x => x.dislikeID == id && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault(),
                dislikes = _context.ListDislikes.Where(x => x.list_dislikeID == id && x.isDeleted != 1).FirstOrDefault(),
            };


            return Json(patientPrefDislike);

        }



        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetPatientWellnessInfo()
        {
            var patientDetail = (from p in _context.Patients
                                 join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                 join a in _context.AlbumPatient on pa.patientAllocationID equals a.patientAllocationID
                                 join u in _context.Users on pa.caregiverID equals u.userID

                                 where p.isDeleted != 1
                                 where pa.isDeleted != 1
                                 where a.isDeleted != 1
                                 where p.isApproved == 1
                                 where pa.isApproved == 1
                                 where a.isApproved == 1
                                 where p.startDate <= DateTime.Today
                                 where p.endDate > DateTime.Today
                                 select new DashboardViewModel
                                 {
                                     patient = p,
                                     albumPath = a.albumPath,
                                     caregiver = u.firstName + " " + u.lastName,

                                     vitalList = _context.Vitals.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.patientAllocationID == pa.patientAllocationID).ToList(),

                                     logList = _context.ProblemLogs.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).ToList(),




                                 }).ToList();

            return Json(patientDetail.OrderBy(x => x.patient.firstName));

        }




        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditActivity(string id)
        {
            int ID = Int32.Parse(id);

            //List<int> timingDuration = new List<int>(new int[] { 30, 60, 90, 120, 150, 180, 210, 240, 270, 300, 330, 360, 390, 420, 450, 480 });
            //ViewBag.SelectedDuration = 30;
            //ViewBag.timingDuration = timingDuration;

            //List<int> minReq = new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            //ViewBag.SelectedDuration = 1;
            //ViewBag.minReq = minReq;

            List<SelectListItem> timingList = new List<SelectListItem>
            {
                new SelectListItem { Text = "30", Value = "30"},
                new SelectListItem { Text = "60", Value = "60"},
                //new SelectListItem { Text = "90", Value = "90"},
                //new SelectListItem { Text = "120", Value = "120"},
                //new SelectListItem { Text = "150", Value = "150"},
                //new SelectListItem { Text = "180", Value = "180"},
                //new SelectListItem { Text = "210", Value = "180"},
                //new SelectListItem { Text = "240", Value = "240"},
                //new SelectListItem { Text = "270", Value = "270"},
                //new SelectListItem { Text = "300", Value = "300"},
                //new SelectListItem { Text = "330", Value = "330"},
                //new SelectListItem { Text = "360", Value = "360"},
                //new SelectListItem { Text = "390", Value = "390"},
                //new SelectListItem { Text = "420", Value = "420"},
                //new SelectListItem { Text = "450", Value = "450"},
                //new SelectListItem { Text = "480", Value = "480"}
            };

            //List<string> timingDuration = new List<string>(new string[] { "30", "60", "90", "120", "150", "180", "210", "240", "270", "300", "330", "360", "390", "420", "450", "480" });
            //ViewBag.SelectedDuration = "30";
            //ViewBag.timingDuration = timingDuration;
            //ViewBag.timingDuration = new SelectList(timingList, "Value", "Text", "30");
            ViewBag.timingDuration = timingList;

            List<SelectListItem> minReqList = new List<SelectListItem>
            {
                new SelectListItem { Text = "1", Value = "1"},
                new SelectListItem { Text = "2", Value = "2"},
                new SelectListItem { Text = "3", Value = "3"},
                new SelectListItem { Text = "4", Value = "4"},
                new SelectListItem { Text = "5", Value = "5"},
                new SelectListItem { Text = "6", Value = "6"},
                new SelectListItem { Text = "7", Value = "7"},
                new SelectListItem { Text = "8", Value = "8"},
                new SelectListItem { Text = "9", Value = "9"},
                new SelectListItem { Text = "10", Value = "10"}
            };

            //List<string> minReq = new List<string>(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
            //ViewBag.SelectedDuration = "1";
            //ViewBag.minReq = minReq;
            //ViewBag.minReq = new SelectList(minReqList, "Value", "Text", "1");
            ViewBag.minReq = minReqList;

            var activities = _context.CentreActivities.Where(x => x.centreActivityID == ID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();


            //var activities = (from ca in _context.CentreActivities
            //                  join aa in _context.ActivityAvailabilities on ca.centreActivityID equals aa.centreActivityID
            //                  where ca.isApproved == 1 && ca.isDeleted != 1
            //                  where aa.isApproved == 1 && aa.isDeleted != 1
            //                  where ca.centreActivityID == ID
            //                  orderby ca.centreActivityID
            //                  select new AvailableActivitiesViewModel
            //                  {
            //                      day = aa.day,
            //                      CentreActivities = ca
            //                  }).SingleOrDefault();

            //var viewModel = new AvailableActivitiesViewModel()
            //{
            //    CentreActivities = activities.CentreActivities,
            //    day = activities.day
            //};

            var availableAct = (from ca in _context.CentreActivities
                                join aa in _context.ActivityAvailabilities on ca.centreActivityID equals aa.centreActivityID
                                where ca.isApproved == 1 && ca.isDeleted != 1
                                where aa.isApproved == 1 && aa.isDeleted != 1
                                where ca.centreActivityID == ID
                                orderby ca.centreActivityID
                                select new AvailableActivitiesViewModel
                                {
                                    day = aa.day,
                                    startTime = (TimeSpan)aa.timeStart,
                                    endTime = (TimeSpan)aa.timeEnd,
                                    availableID = aa.activityAvailabilityID,
                                    isApproved = aa.isApproved,
                                    CentreActivities = ca
                                }).ToList();

            //var activity = _context.CentreActivities.Where(x => x.centreActivityID == ID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            //var startDate = activity.activityStartDate;
            //var endDate = activity.activityEndDate;
            //var shortTitle = 

            var viewModel = new ManageSupervisorsViewModel()
            {
                Activities = availableAct,
                CentreActivities = activities,
                minDuration = activities.minDuration,
                maxDuration = activities.maxDuration,
                minPeopleReq = activities.minPeopleReq,
                isFixed = activities.isFixed,
                isCompulsory = activities.isCompulsory,
                isGroup = activities.isGroup
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditActivityMethod(ManageSupervisorsViewModel item)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            CentreActivity activity = _context.CentreActivities.SingleOrDefault((x => x.centreActivityID == item.id && x.isApproved == 1 && x.isDeleted != 1));

            string columnAffected = "";

            // Serialize using javascript will have a format something like this { "": "", "": "" } - sample only
            var oldLogData = new JavaScriptSerializer().Serialize(activity);
            var isTitleExist = _context.CentreActivities.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.activityTitle == item.title).SingleOrDefault();

            var isshortTitleExist = _context.CentreActivities.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.shortTitle == item.shortTitle).SingleOrDefault();

            string errorMsg = "";

            if (activity != null)
            {
                if (isTitleExist == null || ((activity.activityTitle.Equals(item.title) && isTitleExist != null)))
                {


                    if (item.shortTitle != null && (item.shortTitle != activity.shortTitle) && isshortTitleExist == null)
                    {
                        activity.shortTitle = item.shortTitle;
                        columnAffected = columnAffected + "shortTitle,";

                    }

                    else if (isshortTitleExist != null && (item.shortTitle != activity.shortTitle))
                    {

                        errorMsg = "<br/>  An identical short title exists.";

                        TempData["error"] = "Failed to save changes." + errorMsg;
                        return RedirectToAction("EditActivity", "Supervisor", new { id = item.id });

                    }

                    if (item.title != null && item.title != activity.activityTitle)
                    {
                        activity.activityTitle = item.title;
                        columnAffected = columnAffected + "title,";
                    }

                    if (item.description != null && item.description != activity.activityDesc)
                    {
                        activity.activityDesc = item.description;
                        columnAffected = columnAffected + "description,";
                    }

                    if ( item.isCompulsory != activity.isCompulsory)
                    {
                        activity.isCompulsory = item.isCompulsory;
                        columnAffected = columnAffected + "isCompulsory,";
                    }

                    if ( item.isFixed != activity.isFixed)
                    {
                        activity.isFixed = item.isFixed;
                        columnAffected = columnAffected + "isFixed,";
                    }

                    if ( item.isGroup != activity.isGroup)
                    {
                        activity.isGroup = item.isGroup;
                        columnAffected = columnAffected + "isGroup,";
                    }

                    if (item.minDuration != activity.minDuration)
                    {
                        activity.minDuration = item.minDuration;
                        columnAffected = columnAffected + "minDuration,";
                    }

                    if (item.maxDuration != activity.maxDuration)
                    {
                        activity.maxDuration = item.maxDuration;
                        columnAffected = columnAffected + "maxDuration,";
                    }

                    if (item.minPeopleReq != activity.minPeopleReq)
                    {
                        activity.minPeopleReq = item.minPeopleReq;
                        columnAffected = columnAffected + "minPeopleReq,";
                    }

                    if (item.startDate != null && item.startDate != activity.activityStartDate)
                    {
                        activity.activityStartDate = item.startDate;
                        columnAffected = columnAffected + "activityStartDate,";

                    }

                    if (item.endDate != null && item.endDate != activity.activityEndDate)
                    {
                        activity.activityEndDate = item.endDate;
                        columnAffected = columnAffected + "activityEndDate,";

                    }


                    if (columnAffected.EndsWith(","))
                    {
                        columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                    }


                    _context.SaveChanges();
                    TempData["success"] = "Changes saved successfully!!";


                    var newLogData = new JavaScriptSerializer().Serialize(activity);

                    string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                    string oldLogVal = logVal[0];
                    string newLogVal = logVal[1];

                    // Note: the patientID is equal 0 as it does not affect any patient
                    // shortcutMethod.addLogToDB(string? oldLogData, string logData, string logDesc, int logCategoryID, int? patientID, int userIDInit, int? userIDApproved, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)

                    string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, null, supervisorID, supervisorID, null, null, null, "centreActivity", columnAffected, oldLogVal, newLogVal, activity.centreActivityID, 1, 0, null);

                    //var patient = _context.Patients.Where(x => x.isActive == 1 && x.isApproved == 1 && x.isDeleted != 1).ToList();
                    //foreach (var x in patient) {
                    //    x.updateBit = 1;
                    //}
                    //_context.SaveChanges();

                    scheduler.generateWeeklySchedule(false, false);
                }
                else
                {
                    if (isTitleExist != null && !activity.activityTitle.Equals(item.title))
                    {
                        errorMsg = "<br/>  An identical activity title exists.";
                    }

                    if (isshortTitleExist != null && !activity.shortTitle.Equals(item.shortTitle))
                    {
                        errorMsg = "<br/> An identical short title exists.";

                    }
                    TempData["error"] = "Failed to save changes." + errorMsg;
                }
            }
            else
            {
                TempData["error"] = "Failed to save changes.";

            }

            return RedirectToAction("EditActivity", "Supervisor", new { id = item.id });
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManageAvailability(string id)
        {
            int ID = Int32.Parse(id);

            var activities = _context.CentreActivities.Where(x => x.centreActivityID == ID).SingleOrDefault();

            //var activities = (from ca in _context.CentreActivities
            //                  join aa in _context.ActivityAvailabilities on ca.centreActivityID equals aa.centreActivityID
            //                  where ca.isApproved == 1 && ca.isDeleted != 1
            //                  where aa.isApproved == 1 && aa.isDeleted != 1
            //                  where ca.centreActivityID == ID
            //                  orderby ca.centreActivityID
            //                  select new AvailableActivitiesViewModel
            //                  {
            //                      day = aa.day,
            //                      CentreActivities = ca
            //                  }).SingleOrDefault();

            //var viewModel = new AvailableActivitiesViewModel()
            //{
            //    CentreActivities = activities.CentreActivities,
            //    day = activities.day
            //};

            List<string> dayList = new List<string>(new string[] { "Everyday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" });
            ViewBag.SelectedDay = "Everyday";
            ViewBag.ListOfDay = dayList;

            var availableAct = (from ca in _context.CentreActivities
                                join aa in _context.ActivityAvailabilities on ca.centreActivityID equals aa.centreActivityID
                                where ca.isApproved == 1 && ca.isDeleted != 1
                                where aa.isApproved == 1 && aa.isDeleted != 1
                                where ca.centreActivityID == ID
                                orderby ca.centreActivityID
                                select new AvailableActivitiesViewModel
                                {
                                    day = aa.day,
                                    startTime = (TimeSpan)aa.timeStart,
                                    endTime = (TimeSpan)aa.timeEnd,
                                    availableID = aa.activityAvailabilityID,
                                    isApproved = aa.isApproved,
                                    CentreActivities = ca
                                }).ToList();


            var cc = _context.CareCentreHours.Where(x => x.isDeleted != 1 && x.centreID == 1);
            TimeSpan earliestOpening = TimeSpan.MaxValue;
            TimeSpan latestClosing = TimeSpan.Zero;

            foreach (var x in cc)
            {
                if (x.centreOpeningHours < earliestOpening)
                {
                    earliestOpening = x.centreOpeningHours;
                }

                if (x.centreClosingHours > latestClosing)
                {
                    latestClosing = x.centreClosingHours;
                }
            }




            var viewModel = new ManageSupervisorsViewModel()
            {
                Activities = availableAct,
                CentreActivities = activities,
                openingHour = earliestOpening,
                closingHour = latestClosing,
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManageAvailabilityMethod(ManageSupervisorsViewModel viewModel, string index)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            //int supervisorID = Int32.Parse(Session["userID"].ToString());

            shortcutMethod.printf(index);
            string[] split = index.Split('/');
            string method = split[0];
            int ID = Int32.Parse(split[1]);
            shortcutMethod.printf(ID.ToString());
            //var PatientDetails = _context.Patients.SingleOrDefault(x => (x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1));
            //var actAvailability = _context.ActivityAvailabilities.SingleOrDefault(x => x.activityAvailabilityID == ID);
            //var actAvailability = 0;

            var actAvailability = _context.ActivityAvailabilities.SingleOrDefault(x => x.activityAvailabilityID == ID && x.isApproved == 1 && x.isDeleted != 1);

            if (method.Equals("Delete"))
            {
                var patients = _context.Patients.Where(x => x.isDeleted != 1 && x.isApproved == 1 && x.isActive == 1).ToList();

                if (actAvailability != null)
                {
                    //UpdateBitChanges ubc = new UpdateBitChanges();
                    //ubc.activityAvailabilityID = actAvailability.activityAvailabilityID;
                    //ubc.availabilityDay = actAvailability.day;
                    //ubc.availabilityTimeStart = actAvailability.timeStart;
                    //ubc.availabilityTimeEnd = actAvailability.timeEnd;
                    //ubc.createDateTime = DateTime.Now;
                    //_context.UpdateBitChanges.Add(ubc);
                    //_context.SaveChanges();
                    //var nLogData = new JavaScriptSerializer().Serialize(ubc);

                    //string logDescU = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                    //shortcutMethod.addLogToDB(null, nLogData, logDescU, 16, null, supervisorID, supervisorID, null, null, null, "updateBitChanges", "ALL", null, null, ubc.updateBitChangesID, 1, 0, null);


                    var oldLogData = new JavaScriptSerializer().Serialize(actAvailability);
                    actAvailability.isDeleted = 1;
                    var newLogData = new JavaScriptSerializer().Serialize(actAvailability);

                    //foreach (var x in patients)
                    //{
                    //    x.updateBit = 1;

                    //}
                    //_context.SaveChanges();

                    scheduler.generateWeeklySchedule(false, false);

                    TempData["success"] = "Successfully deleted an activity availability on " + DateTime.Now;

                    //ViewBag.Error = "Successfully deleted an activity availability on " + DateTime.Now;

                    // Note: the patientID is equal 0 as it does not affect any patient
                    // shortcutMethod.addLogToDB(string? oldLogData, string logData, string logDesc, int logCategoryID, int? patientID, int userIDInit, int? userIDApproved, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)


                    //PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
                    string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;


                    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, null, supervisorID, supervisorID, null, null, null, "activityAvailability", "isDeleted", null, null, actAvailability.activityAvailabilityID, 1, 0, null);

                    //shortcutMethod.addLogToDB(null, "", "Delete Availability info for Activity", 4, 0, supervisorID, supervisorID, null, null, null, "activityAvailability", "ALL", "", "", 0, 0, 1, "");

                }
            }
            else if (method.Equals("Create"))
            {
                //var clashAvailability = _context.ActivityAvailabilities.Where(x => x.day == viewModel.addedDay && (x.timeStart >= viewModel.endTime) && (viewModel.startTime <= x.timeEnd)).ToList();

                ActivityAvailability newAvailableAct = new ActivityAvailability();
                newAvailableAct.centreActivityID = viewModel.id;
                newAvailableAct.day = viewModel.addedDay;
                newAvailableAct.isApproved = 1;
                newAvailableAct.isDeleted = 0;
                newAvailableAct.timeStart = viewModel.startTime;
                newAvailableAct.timeEnd = viewModel.endTime;
                newAvailableAct.centreActivityID = viewModel.id;
                newAvailableAct.createDateTime = DateTime.Now;
                var newLogData = new JavaScriptSerializer().Serialize(newAvailableAct);

                //var checkDuplication = _context.ActivityAvailabilities.Where(x => x.centreActivityID == viewModel.id && x.day == viewModel.addedDay && x.isDeleted != 1 && x.isApproved == 1);


                //if (newAvailableAct != null && clashAvailability.Count() == 0)
                if (newAvailableAct != null)
                {

                    //    if (newAvailableAct != null && checkDuplication.Count() == 0)
                    //{
                    _context.ActivityAvailabilities.Add(newAvailableAct);
                    _context.SaveChanges();
                    TempData["success"] = "Successfully added an activity availability on " + DateTime.Now;


                    string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                    shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, null, supervisorID, supervisorID, null, null, null, "activityAvailability", "ALL", null, null, newAvailableAct.activityAvailabilityID, 1, 0, null);

                    //generate schedule
                    var patients = _context.Patients.Where(x => x.isDeleted != 1 && x.isApproved == 1 && x.isActive == 1).ToList();

                    //foreach (var x in patients)
                    //{
                    //    x.updateBit = 1;

                    //}
                    //_context.SaveChanges();
                    scheduler.generateWeeklySchedule(false, false);
                }
                else if (newAvailableAct == null)
                {
                    //ViewBag.Error = "Invalid activity availability!";
                    TempData["error"] = "Failed to add an activity availability.";

                }
                //else
                //{
                //    TempData["error"] = "Failed to add an activity availability. <br/> Availability clashes with existing activity!";

                //}

                //var PatientSocialHistory = _context.SocialHistories.SingleOrDefault(x => x.patientID == ID);
                //Habit toBeAddedHabit = new Habit();
                //toBeAddedHabit.isApproved = 1;
                //toBeAddedHabit.isDeleted = 0;
                //string habitItem = Updatedmodel.habitList[ID2].habit;
                //toBeAddedHabit.habit = habitItem;
                //toBeAddedHabit.socialHistoryID = PatientSocialHistory.socialHistoryID;
                //var checkDuplicateHabit = _context.Habits.Where(x => (x.habit == habitItem && x.isDeleted != 1 && x.socialHistoryID == PatientSocialHistory.socialHistoryID)).ToList();
                //if (habitItem != null && checkDuplicateHabit.Count() == 0)
                //{
                //    _context.Habits.Add(toBeAddedHabit);
                //    _context.SaveChanges();
                //    ViewBag.Error = "Successfully Inserted Habits for " + PatientDetails.firstName + " " + PatientDetails.lastName + " " + PatientDetails.nric.Remove(1, 4).Insert(1, "xxxx") + " on " + DateTime.Now;
                //}
                //else if (habitItem == null)
                //    ViewBag.Error = "Please select the correct Habits Item.";
                //else
                //    ViewBag.Error = "This Habits item already exists for this patient. ";
            }
            else if (method.Equals("Edit"))
            {

                //var checkDuplication = _context.ActivityAvailabilities.Where(x => x.centreActivityID == viewModel.id && x.day == viewModel.addedDay && x.isDeleted != 1 && x.isApproved == 1);

                int availabilityID = Convert.ToInt32(Request.Form.Get("activityAvailabilityID"));

                var availability = _context.ActivityAvailabilities.SingleOrDefault(x => x.activityAvailabilityID == availabilityID && x.isApproved == 1 && x.isDeleted != 1);

                //var clashAvailability = _context.ActivityAvailabilities.Where(x => x.day == viewModel.addedDay && (x.timeStart >= viewModel.endTime) && (viewModel.startTime <= x.timeEnd)).ToList(); 

                string columnAffected = "";

                //if(clashAvailability.Count() == 0) {
                var oldLogData = new JavaScriptSerializer().Serialize(availability);

                if (availability != null)
                {
                    //UpdateBitChanges ubc = new UpdateBitChanges();
                    //ubc.activityAvailabilityID = availabilityID;
                    //ubc.availabilityDay = availability.day;
                    //ubc.availabilityTimeStart = availability.timeStart;
                    //ubc.availabilityTimeEnd = availability.timeEnd;
                    //ubc.createDateTime = DateTime.Now;
                    //int updateFlag = 0;


                    if (availability.day != viewModel.addedDay)
                    {
                        availability.day = viewModel.addedDay;
                        columnAffected = "addedDay,";
                        //updateFlag = 1;
                    }

                    if (availability.timeStart != viewModel.startTime)
                    {
                        availability.timeStart = viewModel.startTime;
                        columnAffected = "timeStart,";
                        //updateFlag = 1;

                    }

                    if (availability.timeEnd != viewModel.endTime)
                    {
                        availability.timeEnd = viewModel.endTime;
                        columnAffected = "timeEnd,";
                        //updateFlag = 1;

                    }


                    if (columnAffected.EndsWith(","))
                    {
                        columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                    }

                    //if (updateFlag == 1)
                    //{

                    //    _context.UpdateBitChanges.Add(ubc);
                    //    _context.SaveChanges();
                    //    var nLogData = new JavaScriptSerializer().Serialize(ubc);

                    //    string logDescU = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                    //    shortcutMethod.addLogToDB(null, nLogData, logDescU, 16, null, supervisorID, supervisorID, null, null, null, "updateBitChanges", "ALL", null, null, ubc.updateBitChangesID, 1, 0, null);


                    //generate schedule
                    //var patients = _context.Patients.Where(x => x.isDeleted != 1 && x.isApproved == 1 && x.isActive == 1).ToList();

                    //foreach (var x in patients)
                    //{
                    //    x.updateBit = 1;

                    //}
                    _context.SaveChanges();
                    scheduler.generateWeeklySchedule(false, false);

                    //}


                    _context.SaveChanges();
                    TempData["success"] = "Changes saved successfully!!";


                    var newLogData = new JavaScriptSerializer().Serialize(availability);

                    string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                    string oldLogVal = logVal[0];
                    string newLogVal = logVal[1];

                    string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, null, supervisorID, supervisorID, null, null, null, "activityAvailability", columnAffected, oldLogVal, newLogVal, availability.activityAvailabilityID, 1, 0, null);
                }
                else
                {
                    TempData["error"] = "Failed to save changes.";

                }
                //}
                //else {
                //    TempData["error"] = "Failed to save changes. <br/> Availability clashes with exisiting activity!" ;

                //}
            }

            //return RedirectToAction("ManageAvailability", "Supervisor", new { id = actAvailability.centreActivityID });
            return RedirectToAction("ManageAvailability", "Supervisor", new { id = viewModel.id });
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManageVital(string id)
        {

            int ID = Int32.Parse(id);


            var patient = _context.Patients.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            var pa = _context.PatientAllocations.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            var vitalList = _context.Vitals.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).ToList();
            var viewModel = new VitalViewModel()
            {
                vitalList = vitalList,
                patient = patient,
            };

            return View(viewModel);
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ViewMedicationSchedule()
        {

            var otherPatientList = _context.Patients.Where(x => x.isActive == 1 && x.isApproved == 1 && x.isDeleted != 1 && (x.endDate > DateTime.Today  || x.endDate == null));


            var medSchedList = (from p in _context.Patients
                                where p.isApproved == 1 && p.isDeleted != 1 && p.isActive == 1
                                where p.endDate > DateTime.Today || p.endDate == null
                                select new MedSchedule
                                {
                                    patient = p,


                                }).ToList();


            List<PatientMedSched> patientMedSchedList = new List<PatientMedSched>();
            foreach (var item in medSchedList)
            {
                patientMedSchedList = (from pa in _context.PatientAllocations
                                       join med in _context.MedicationLog on pa.patientAllocationID equals med.patientAllocationID
                                       join pscp in _context.Prescriptions on pa.patientAllocationID equals pscp.patientAllocationID
                                       //where p.isApproved == 1 && p.isDeleted != 1 && p.isActive == 1
                                       where pa.isApproved == 1 && pa.isDeleted != 1
                                       where pscp.isApproved == 1 && pscp.isDeleted != 1
                                       where med.isApproved == 1 && med.isDeleted != 1
                                       where med.prescriptionID == pscp.prescriptionID
                                       where med.dateForMedication == DateTime.Today
                                       //where med.dateForMedication == DbFunctions.AddDays(DateTime.Today, -5)  //Change it to Today
                                       where pa.patientID == item.patient.patientID
                                       select new PatientMedSched
                                       {
                                           drugName = _context.ListPrescriptions.FirstOrDefault(x => x.list_prescriptionID == pscp.drugNameID && x.isDeleted != 1).value,
                                           medLog = med,


                                       }).ToList();


                item.patientMedSchedList = patientMedSchedList;

                item.pscpCount = patientMedSchedList.Count();

            }

            var stTime = DateTime.Parse("9:00").TimeOfDay;
            var eTime = DateTime.Parse("17:00").TimeOfDay;

            var centreHours = _context.CareCentreHours.Where(x => x.centreID == 1 && x.centreWorkingDay == DateTime.Today.DayOfWeek.ToString() && x.isDeleted != 1).SingleOrDefault();


            if (centreHours != null)
            {
                stTime = centreHours.centreOpeningHours;
                eTime = centreHours.centreClosingHours;
            }



            //var startTime = DateTime.Parse("9:00");
            //var endTime = DateTime.Parse("17:00");

            //JObject JObjS = new JObject();
            JArray jArray = new JArray();
            JArray jArrayOuter = new JArray();

            foreach (var item in medSchedList)
            {
                JObject JObj = new JObject();

                int pflag = 0;
                if (pflag == 0)
                {
                    if (item.patientMedSchedList.Count() != 0)
                    {
                        JObj["patientName"] = item.patient.firstName + " " + item.patient.lastName;
                        pflag = 1;

                        otherPatientList = otherPatientList.Where(x => x.patientID != item.patient.patientID);
                    }


                }


                var startTime = Convert.ToDateTime(stTime.ToString());
                var endTime = Convert.ToDateTime(eTime.ToString());

                if (item.patientMedSchedList.Count() != 0)
                {
                    JArray drugNameArray = new JArray();

                    while (startTime <= endTime)
                    {

                        JObject message = new JObject();

                        int flag = 0;


                        for (int i = 0; i < item.patientMedSchedList.Count();)
                        {

                            if (item.patientMedSchedList[i].medLog.timeForMedication == startTime.TimeOfDay)
                            {
                                JObject drugObject = new JObject();

                                if (flag == 0)
                                {
                                    message["timeSlot"] = item.patientMedSchedList[i].medLog.timeForMedication;
                                    drugNameArray.Clear();
                                    flag = 1;
                                }

                                TimeSpan timeOfDay = DateTime.Now.TimeOfDay;
                                var medTime = item.patientMedSchedList[i].medLog.timeForMedication;

                                if (item.patientMedSchedList[i].medLog.timeTaken != null) //Taken
                                {
                                    drugObject[item.patientMedSchedList[i].drugName] = 1;
                                }
                                else if (item.patientMedSchedList[i].medLog.timeTaken == null && medTime > timeOfDay) //Allocated
                                {
                                    drugObject[item.patientMedSchedList[i].drugName] = 2;

                                }
                                else if (item.patientMedSchedList[i].medLog.timeTaken == null && medTime < timeOfDay)
                                { //Missed
                                    drugObject[item.patientMedSchedList[i].drugName] = 0;
                                }

                                drugNameArray.Add(drugObject);

                            }
                            i++;
                        }//loop schedule

                        if (drugNameArray.Count() != 0)
                        {
                            message["drugName"] = new JArray(drugNameArray);

                            if (message.ContainsKey("timeSlot"))
                            {
                                jArray.Add(message);

                            }
                            
                          

                        }

                        startTime = startTime.AddMinutes(30);

                    }//loop timeSlot

                    if (jArray.HasValues)
                    {

                        JObj["schedule"] = new JArray(jArray);
                        jArray.Clear();

                    }
                    else {
                        pflag = 0;
                    }

                }//check 

                if (pflag == 1)
                {
             
                        jArrayOuter.Add(JObj);
                   
                }

            }//per patient

            //var otherPatientsList = 

            var viewModel = new MedicationScheduleViewModel()
            {
                MedSchedList = medSchedList,
                schedList = jArrayOuter,
                openingHour = Convert.ToDateTime(stTime.ToString()),
                closingHour = Convert.ToDateTime(eTime.ToString()),
                otherPatientList = otherPatientList.ToList(),
                scheduleDate = DateTime.Today,
            };




            return View(viewModel);
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult FilterMedicationScheduleByDate(MedicationScheduleViewModel itemInput)
        {

            var filteredDate = DateTime.ParseExact(itemInput.filterByDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);


            var otherPatientList = _context.Patients.Where(x => x.isActive == 1 && x.isApproved == 1 && x.isDeleted != 1 && x.startDate <= filteredDate && (x.endDate > filteredDate || x.endDate == null));


            var medSchedList = (from p in _context.Patients
                                where p.isApproved == 1 && p.isDeleted != 1 && p.isActive == 1
                                where p.startDate <= filteredDate 
                                where p.endDate > filteredDate || p.endDate == null
                                select new MedSchedule
                                {
                                    patient = p,


                                }).ToList();


            List<PatientMedSched> patientMedSchedList = new List<PatientMedSched>();
            foreach (var item in medSchedList)
            {
                patientMedSchedList = (from pa in _context.PatientAllocations
                                       join med in _context.MedicationLog on pa.patientAllocationID equals med.patientAllocationID
                                       join pscp in _context.Prescriptions on pa.patientAllocationID equals pscp.patientAllocationID
                                       //where p.isApproved == 1 && p.isDeleted != 1 && p.isActive == 1
                                       where pa.isApproved == 1 && pa.isDeleted != 1
                                       where pscp.isApproved == 1 && pscp.isDeleted != 1
                                       where med.isApproved == 1 && med.isDeleted != 1
                                       where med.prescriptionID == pscp.prescriptionID
                                       where med.dateForMedication == filteredDate
                                       //where med.dateForMedication == DbFunctions.AddDays(DateTime.Today, -5)  //Change it to Today
                                       where pa.patientID == item.patient.patientID
                                       select new PatientMedSched
                                       {
                                           drugName = _context.ListPrescriptions.FirstOrDefault(x => x.list_prescriptionID == pscp.drugNameID && x.isDeleted != 1).value,
                                           medLog = med,


                                       }).ToList();


                item.patientMedSchedList = patientMedSchedList;

                item.pscpCount = patientMedSchedList.Count();

            }

            var centreHours = _context.CareCentreHours.Where(x => x.centreID == 1 && x.centreWorkingDay == filteredDate.DayOfWeek.ToString() && x.isDeleted != 1).SingleOrDefault();
            var stTime = centreHours.centreOpeningHours;
            var eTime = centreHours.centreClosingHours;



            //var startTime = DateTime.Parse("9:00");
            //var endTime = DateTime.Parse("17:00");

            //JObject JObjS = new JObject();
            JArray jArray = new JArray();
            JArray jArrayOuter = new JArray();

            foreach (var item in medSchedList)
            {
                JObject JObj = new JObject();

                int pflag = 0;
                if (pflag == 0)
                {
                    if (item.patientMedSchedList.Count() != 0)
                    {
                        JObj["patientName"] = item.patient.firstName + " " + item.patient.lastName;
                        pflag = 1;

                        otherPatientList = otherPatientList.Where(x => x.patientID != item.patient.patientID);
                    }


                }

                var startTime = Convert.ToDateTime(stTime.ToString());
                var endTime = Convert.ToDateTime(eTime.ToString());

                if (item.patientMedSchedList.Count() != 0)
                {
                    JArray drugNameArray = new JArray();

                    while (startTime <= endTime)
                    {

                        JObject message = new JObject();

                        int flag = 0;


                        for (int i = 0; i < item.patientMedSchedList.Count();)
                        {

                            if (item.patientMedSchedList[i].medLog.timeForMedication == startTime.TimeOfDay)
                            {
                                JObject drugObject = new JObject();

                                if (flag == 0)
                                {
                                    message["timeSlot"] = item.patientMedSchedList[i].medLog.timeForMedication;
                                    drugNameArray.Clear();
                                    flag = 1;
                                }

                                TimeSpan timeOfDay = DateTime.Now.TimeOfDay;
                                var medTime = item.patientMedSchedList[i].medLog.timeForMedication;

                                if (item.patientMedSchedList[i].medLog.timeTaken != null) //Taken
                                {
                                    drugObject[item.patientMedSchedList[i].drugName] = 1;
                                }
                                else if (item.patientMedSchedList[i].medLog.timeTaken == null && medTime > timeOfDay) //Allocated
                                {
                                    drugObject[item.patientMedSchedList[i].drugName] = 2;

                                }
                                else if (item.patientMedSchedList[i].medLog.timeTaken == null && medTime < timeOfDay)
                                { //Missed
                                    drugObject[item.patientMedSchedList[i].drugName] = 0;
                                }

                                drugNameArray.Add(drugObject);

                            }
                            i++;
                        }//loop schedule

                        if (drugNameArray.Count() != 0)
                        {
                            message["drugName"] = new JArray(drugNameArray);

                            if (message.ContainsKey("timeSlot"))
                            {
                                jArray.Add(message);

                            }

                        }

                        startTime = startTime.AddMinutes(30);

                    }//loop timeSlot

                    if (jArray.HasValues)
                    {
                        JObj["schedule"] = new JArray(jArray);
                        jArray.Clear();

                    }

                }//check 

                if (pflag == 1)
                {
                    jArrayOuter.Add(JObj);
                }

            }//per patient

            //var otherPatientsList = 

            var viewModel = new MedicationScheduleViewModel()
            {
                MedSchedList = medSchedList,
                schedList = jArrayOuter,
                openingHour = Convert.ToDateTime(stTime.ToString()),
                closingHour = Convert.ToDateTime(eTime.ToString()),
                otherPatientList = otherPatientList.ToList(),
                scheduleDate = filteredDate,

            };



            return View("ViewMedicationSchedule", viewModel);

            //return View(viewModel);
        }



        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManagePrescription(string id)
        {

            int ID = Int32.Parse(id);


            var patientPrescription = (from p in _context.Patients
                                       join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                       join pscp in _context.Prescriptions on pa.patientAllocationID equals pscp.patientAllocationID
                                       where p.isDeleted != 1
                                       where pa.isDeleted != 1
                                       where pscp.isDeleted != 1
                                       where p.isApproved == 1
                                       where pa.isApproved == 1
                                       where pscp.isApproved == 1
                                       where p.patientID == ID


                                       select new PatientPrescription
                                       {
                                           prescription = pscp,
                                           PrescriptionName = _context.ListPrescriptions.Where(x => x.list_prescriptionID == pscp.drugNameID && x.isDeleted != 1).FirstOrDefault().value,
                                           //prescriptionList = _context.Prescriptions.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault(),

                                       }).ToList();


            var patientMedication = (from p in _context.Patients
                                     join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                     join med in _context.MedicationLog on pa.patientAllocationID equals med.patientAllocationID
                                     join pscp in _context.Prescriptions on med.prescriptionID equals pscp.prescriptionID
                                     where p.isDeleted != 1
                                     where pa.isDeleted != 1
                                     where pscp.isDeleted != 1
                                     where p.isApproved == 1
                                     where pa.isApproved == 1
                                     where pscp.isApproved == 1
                                     where p.patientID == ID


                                     select new PatientMedication
                                     {
                                         med = med,
                                         prescription = pscp,
                                         PrescriptionName = _context.ListPrescriptions.Where(x => x.list_prescriptionID == pscp.drugNameID && x.isDeleted != 1).FirstOrDefault().value,
                                         //prescriptionList = _context.Prescriptions.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault(),
                                         userFullname = med.User.AspNetUsers.firstName + " " + med.User.AspNetUsers.lastName,

                                     }).ToList();


            var patient = _context.Patients.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            //var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            //var prescriptionList = _context.Prescriptions.Where(x => x.patientAllocationID == patientAllocation.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).ToList();
            var drugList = _context.ListPrescriptions.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Prescription drug = new List_Prescription();
            drug.value = "Others";
            drug.list_prescriptionID = -1;
            drugList.Add(drug);



            var caregiverList = (from u in _context.Users
                                 join ut in _context.UserTypes on u.userTypeID equals ut.userTypeID
                                 where ut.isDeleted != 1
                                 where ut.userTypeName == "Caregiver" || ut.userTypeName == "Supervisor"
                                 where u.isApproved == 1 && u.isDeleted != 1
                                 select new UserViewModel
                                 {
                                     userID = u.userID,
                                     userFullname = u.firstName + " " + u.lastName,
                                 }).ToList();


            var viewModel = new PrescriptionViewModel()
            {
                patient = patient,
                drugList = drugList,
                patientPrescriptions = patientPrescription,
                userList = caregiverList,
                patientMedication = patientMedication,
            };

            return View(viewModel);
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManageMedicalHistory(string id)
        {

            int ID = Int32.Parse(id);


            var patient = _context.Patients.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            var pa = _context.PatientAllocations.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            var medHistoryList = _context.MedicalHistory.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).ToList();
            var viewModel = new MedicalHistoryViewModel()
            {
                medicalHistList = medHistoryList,
                patient = patient,
            };

            return View(viewModel);
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManageAllergy(string id)
        {

            int ID = Int32.Parse(id);


            var patient = _context.Patients.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            //var pa = _context.PatientAllocations.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            //var allergy = _context.Allergies.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).ToList();
            var listOfAllergies = _context.ListAllergy.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Allergy listAllergy = new List_Allergy();
            listAllergy.list_allergyID = -1;
            listAllergy.value = "Others";
            listOfAllergies.Add(listAllergy);

            var patientAllergies = (from p in _context.Patients
                                    join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                    join al in _context.Allergies on pa.patientAllocationID equals al.patientAllocationID
                                    where p.isDeleted != 1
                                    where pa.isDeleted != 1
                                    where p.isApproved == 1
                                    where pa.isApproved == 1
                                    where p.isActive == 1
                                    where p.patientID == ID
                                    where al.isApproved == 1
                                    where al.isDeleted != 1
                                    select new PatientAllergy
                                    {
                                        allergy = al,
                                        allergyName = _context.ListAllergy.Where(x => x.list_allergyID == al.allergyListID && x.isDeleted != 1).FirstOrDefault().value,
                                    }
                                    ).ToList();


            var viewModel = new AllergyViewModel()
            {
                //allergyList = allergy,
                patient = patient,
                listOfAllergies = listOfAllergies,
                patientAllergies = patientAllergies,
            };

            return View(viewModel);
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManagePersoPreference(string id)
        {
            int ID = Int32.Parse(id);

            var patientPrefLike = (from p in _context.Patients
                                   join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                   join sh in _context.SocialHistories on pa.patientAllocationID equals sh.patientAllocationID
                                   join likes in _context.Likes on sh.socialHistoryID equals likes.socialHistoryID
                                   where p.isDeleted != 1
                                   where pa.isDeleted != 1
                                   where sh.isDeleted != 1
                                   where likes.isDeleted != 1
                                   where p.isApproved == 1
                                   where pa.isApproved == 1
                                   where sh.isApproved == 1
                                   where likes.isApproved == 1
                                   where p.patientID == ID
                                   where p.isActive == 1

                                   select new PatientPrefLike
                                   {
                                       likesID = likes.likeID,
                                       likes = _context.ListLikes.Where(x => x.list_likeID == likes.likeItemID && x.isDeleted != 1).FirstOrDefault(),



                                   }).ToList();

            var patientPrefDislike = (from p in _context.Patients
                                      join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                      join sh in _context.SocialHistories on pa.patientAllocationID equals sh.patientAllocationID
                                      join dislikes in _context.Dislikes on sh.socialHistoryID equals dislikes.socialHistoryID
                                      where p.isDeleted != 1
                                      where pa.isDeleted != 1
                                      where sh.isDeleted != 1
                                      where dislikes.isDeleted != 1
                                      where p.isApproved == 1
                                      where pa.isApproved == 1
                                      where sh.isApproved == 1
                                      where dislikes.isApproved == 1
                                      where p.patientID == ID
                                      where p.isActive == 1


                                      select new PatientPrefDislike
                                      {
                                          dislikesID = dislikes.dislikeID,
                                          dislikes = _context.ListDislikes.Where(x => x.list_dislikeID == dislikes.dislikeItemID && x.isDeleted != 1).FirstOrDefault(),



                                      }).ToList();


            var likesEnum = _context.ListLikes.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Like like = new List_Like();
            like.value = "Other";
            like.list_likeID = -1;

            likesEnum.Add(like);

            var dislikesEnum = _context.ListDislikes.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();
            List_Dislike dislike = new List_Dislike();
            dislike.value = "Other";
            dislike.list_dislikeID = -1;

            dislikesEnum.Add(dislike);


            var patient = _context.Patients.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();

            var viewModel = new PersonalPreferenceViewModel()
            {
                patient = patient,
                ListOfPatientPrefLike = patientPrefLike,
                ListOfPatientPrefDislike = patientPrefDislike,

                likesEnum = likesEnum,
                dislikesEnum = dislikesEnum,

            };

            return View(viewModel);




        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManagePhotoAlbum(string id)
        {

            int patientID = Int32.Parse(id);

            var patient = _context.Patients.Where(x => x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();


            //var patient = _context.Patients.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            //var pa = _context.PatientAllocations.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            //var logList = _context.ProblemLogs.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).ToList();
            ////var problemLog = _context.ProblemLogs.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            //var probLogList = _context.ListProblemLogs.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();



            var albumPatientList = (from album in _context.AlbumPatient
                                    where album.patientAllocationID == patientAllocation.patientAllocationID
                                    where album.isApproved == 1 && album.isDeleted != 1
                                    select new AlbumPatientInfo
                                    {
                                        albumPatient = album,
                                        albumCategory = _context.AlbumCategories.Where(x => x.albumCatID == album.albumCatID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault(),
                                        holiday = _context.HolidayExperiences.Where(x => x.albumPatientID == album.albumID && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault(),
                                    }).ToList();

            var countryList = _context.ListCountries.Where(x => x.isDeleted != 1 && x.isChecked == 1).ToList();
            List_Country listCountry = new List_Country();
            listCountry.value = "Others";
            listCountry.list_countryID = -1;
            countryList.Add(listCountry);

            var albumCategories = _context.AlbumCategories.Where(x => x.isApproved == 1 && x.isDeleted != 1).ToList();

            AlbumCategory albumCat = new AlbumCategory();
            albumCat.albumCatName = "Others";
            albumCat.albumCatID = -1;
            albumCategories.Add(albumCat);

            var viewModel = new PatientPhotoAlbumModel()
            {
                patient = patient,
                albumPatientList = albumPatientList.OrderByDescending(x => x.albumPatient.createDateTime).ToList(),
                listOfAlbumCategories = albumCategories,
                listOfCountries = countryList,
            };

            return View(viewModel);
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManageProblemLog(string id)
        {

            int ID = Int32.Parse(id);


            var patient = _context.Patients.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            var pa = _context.PatientAllocations.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            var logList = _context.ProblemLogs.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).ToList();
            //var problemLog = _context.ProblemLogs.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            var probLogList = _context.ListProblemLogs.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();


            var viewModel = new ProblemLogViewModel()
            {
                logList = logList,
                problemlogList = probLogList,
                patient = patient,
            };

            return View(viewModel);
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManageDementiaCondition(string id)
        {

            int ID = Int32.Parse(id);


            var patient = _context.Patients.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            var pa = _context.PatientAllocations.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();

            var dementiaCondition = (from pad in _context.PatientAssignedDementias
                                     where pad.patientAllocationID == pa.patientAllocationID
                                     where pad.isApproved == 1 && pad.isDeleted != 1
                                     select new DementiaDetails
                                     {
                                         pad = pad,
                                         dementiaCondition = _context.DementiaTypes.Where(x => x.dementiaID == pad.dementiaID).FirstOrDefault().dementiaType,

                                     }).ToList();



            var ListOfDementiaTypes = _context.DementiaTypes.Where(x => x.isApproved == 1 && x.isDeleted != 1).ToList();

            var viewModel = new DementiaConditionViewModel()
            {
                listOfDementiaTypes = ListOfDementiaTypes,
                patient = patient,
                dementiaConditionList = dementiaCondition
            };

            return View(viewModel);
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManageAttendanceLog()
        {

            DateTime firstDayofWeek = (DateTimeExtensions.FirstDayOfWeek(DateTime.Today)).AddDays(1);
            DateTime lastDayofWeek = (DateTimeExtensions.LastDayOfWeek(DateTime.Today)).AddDays(-1);


            var attendance = (from p in _context.Patients
                              join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                              join att in _context.AttendanceLog on pa.patientAllocationID equals att.patientAllocationID
                              where p.isDeleted != 1
                              where pa.isDeleted != 1
                              where p.isApproved == 1
                              where pa.isApproved == 1
                              where p.isActive == 1
                              where att.isApproved == 1
                              where att.isDeleted != 1
                              //where att.attendanceDate == DateTime.Today
                              where att.attendanceDate >= firstDayofWeek
                              where att.attendanceDate <= lastDayofWeek


                              select new PatientAttendance
                              {

                                  attendance = att,
                                  patient = p,

                              }).ToList();


            List<string> weekdays = new List<string>(new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" });

            var patientList = attendance.GroupBy(x => x.patient.nric).Select(x => x.First());



            var viewModel = new AttendanceLogViewModel()
            {
                patientAttendances = attendance,
                weekdays = weekdays,
                firstDayOfWeek = firstDayofWeek,
                lastDayOfWeek = lastDayofWeek,
                patientList = patientList,
            };

            return View(viewModel);
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult FilterAttendanceByWeek(AttendanceLogViewModel item)
        {

            var start = item.startDate;
            var end = item.endDate;

            var attendance = (from p in _context.Patients
                              join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                              join att in _context.AttendanceLog on pa.patientAllocationID equals att.patientAllocationID
                              where p.isDeleted != 1
                              where pa.isDeleted != 1
                              where p.isApproved == 1
                              where pa.isApproved == 1
                              where p.isActive == 1
                              where att.isApproved == 1
                              where att.isDeleted != 1
                              //where att.attendanceDate == DateTime.Today
                              where att.attendanceDate >= start
                              where att.attendanceDate <= end


                              select new PatientAttendance
                              {

                                  attendance = att,
                                  patient = p,

                              }).ToList();


            List<string> weekdays = new List<string>(new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" });

            var patientList = attendance.GroupBy(x => x.patient.nric).Select(x => x.First());



            var viewModel = new AttendanceLogViewModel()
            {
                patientAttendances = attendance,
                weekdays = weekdays,
                firstDayOfWeek = start,
                lastDayOfWeek = end,
                patientList = patientList,
            };


            return View("ManageAttendanceLog", viewModel);


        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetAttendanceByWeek(string startDate, string endDate)
        {

            var start = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var end = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);



            var attendance = (from p in _context.Patients
                              join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                              join att in _context.AttendanceLog on pa.patientAllocationID equals att.patientAllocationID
                              where p.isDeleted != 1
                              where pa.isDeleted != 1
                              where p.isApproved == 1
                              where pa.isApproved == 1
                              where p.isActive == 1
                              where att.isApproved == 1
                              where att.isDeleted != 1
                              where att.attendanceDate >= start
                              where att.attendanceDate <= end

                              select new PatientAttendance
                              {

                                  attendance = att,
                                  patient = p,

                              }).ToList();



            var viewModel = new AttendanceLogViewModel()
            {
                patientAttendances = attendance,
            };

            return Json(viewModel);


        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetAttendanceByDay(string day)
        {

            var dateFilter = DateTime.ParseExact(day, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (day != "")
            {



                var attendance = (from p in _context.Patients
                                  join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                  join att in _context.AttendanceLog on pa.patientAllocationID equals att.patientAllocationID
                                  where p.isDeleted != 1
                                  where pa.isDeleted != 1
                                  where p.isApproved == 1
                                  where pa.isApproved == 1
                                  where p.isActive == 1
                                  where att.isApproved == 1
                                  where att.isDeleted != 1
                                  where att.attendanceDate == dateFilter

                                  select new PatientAttendance
                                  {

                                      attendance = att,
                                      patient = p,

                                  }).ToList();



                var viewModel = new AttendanceLogViewModel()
                {
                    patientAttendances = attendance,
                };

                return Json(viewModel);
            }
            else
            {
                return null;
            }
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetAttendanceDetails(string attID, string patientID)
        {
            int ID = Int32.Parse(patientID);

            int attendanceID = Int32.Parse(attID);

            var patient = _context.Patients.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            var pa = _context.PatientAllocations.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            var att = _context.AttendanceLog.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1
                        && x.attendanceLogID == attendanceID).SingleOrDefault();

            //return Json(att);

            //var attendance = (from p in _context.Patients
            //                  join pa in _context.PatientAllocations on p.patientID equals pa.patientID
            //                  join att in _context.AttendanceLog on pa.patientAllocationID equals att.patientAllocationID
            //                  where p.isDeleted != 1
            //                  where pa.isDeleted != 1
            //                  where p.isApproved == 1
            //                  where pa.isApproved == 1
            //                  where p.isActive == 1
            //                  where att.isApproved == 1
            //                  where att.isDeleted != 1
            //                  where att.attendanceLogID == attendanceID
            //                  where p.patientID == ID

            //                  select new PatientAttendance
            //                  {

            //                      attendance = att,
            //                      patient = p,

            //                  }).ToList();



            var viewModel = new AttendanceLogViewModel()
            {
                attendanceInput = att,
                patient = patient,
            };

            return Json(viewModel);
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ViewAttendanceLog(string id)
        {

            int ID = Int32.Parse(id);


            var patient = _context.Patients.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            var pa = _context.PatientAllocations.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            var attList = _context.AttendanceLog.Where(x => x.patientAllocationID == pa.patientAllocationID && x.attendanceDate.Month == DateTime.Today.Month && x.isApproved == 1 && x.isDeleted != 1).ToList();

            var attMonthYear =_context.AttendanceLog.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).ToList();

            List<string> monthList = new List<string>();

            string value;
            foreach (var item in attMonthYear)
            {
                value = item.attendanceDate.ToString("MMMM yyyy");
                if (!monthList.Contains(item.attendanceDate.ToString("MMMM yyyy")))
                {
                    monthList.Add(value);
                }
            }

            var viewModel = new AttendanceLogViewModel()
            {
                attendanceLog = attList,
                patient = patient,
                monthList = monthList,
            };

            return View(viewModel);
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetAttendanceByMonth(string month, string id)
        {
            if (month != "")
            {
                int ID = Int32.Parse(id);

                string[] date = month.Split(' ');

                string monthDate = date[0];
                string yearDate = date[1];

                int monthInteger = DateTime.ParseExact(monthDate, "MMMM", CultureInfo.CurrentCulture).Month;

                int yearInteger = Int32.Parse(yearDate);

                var patient = _context.Patients.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
                var pa = _context.PatientAllocations.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
                var attList = _context.AttendanceLog.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1
                            && x.attendanceDate.Month == monthInteger && x.attendanceDate.Year == yearInteger).ToList();



                return Json(attList);
            }
            else
            {
                return null;
            }
        }
        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditAttendanceLog(AttendanceLogViewModel viewModel)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            int patientID = Convert.ToInt32(Request.Form.Get("patientID"));
            int attendanceLogID = Convert.ToInt32(Request.Form.Get("attendanceLogID"));
            var arrivalTime = viewModel.attendanceInput.arrivalTime;
            var departureTime = viewModel.attendanceInput.departureTime;

            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            var att = _context.AttendanceLog.Where(x => x.attendanceLogID == attendanceLogID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();



            if (att != null)
            {
                string result = patientMethod.updateAttendanceLog(supervisorID, patientAllocation.patientAllocationID, attendanceLogID, arrivalTime, departureTime, 1);
                if (result.Contains("success"))
                {
                    TempData["success"] = result;
                }
                else
                {
                    TempData["error"] = result;
                }

                //    var oldLogData = new JavaScriptSerializer().Serialize(att);
                //    string columnAffected = "";

                //    if (att.arrivalTime != viewModel.attendanceInput.arrivalTime)
                //    {
                //        att.arrivalTime = viewModel.attendanceInput.arrivalTime;
                //        columnAffected = columnAffected + "arrivalTime,";

                //    }

                //    if (att.departureTime != viewModel.attendanceInput.departureTime)
                //    {
                //        att.departureTime = viewModel.attendanceInput.departureTime;
                //        columnAffected = columnAffected + "departureTime,";

                //    }


                //    if (columnAffected.EndsWith(","))
                //    {
                //        columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                //    }

                //    if (!columnAffected.Equals(""))
                //    {
                //        _context.SaveChanges();
                //        TempData["success"] = "Changes saved successfully!!";
                //        var newLogData = new JavaScriptSerializer().Serialize(att);

                //        string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                //        string oldLogVal = logVal[0];
                //        string newLogVal = logVal[1];

                //        string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //        shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "attendanceLog", columnAffected, oldLogVal, newLogVal, attendanceLogID, 1, 0, null);
                //    }
            }
            else
            {
                TempData["error"] = "Failed to save changes.";

            }
            return RedirectToAction("ManageAttendanceLog", "Supervisor");

        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManageRoutine(string id)
        {

            int ID = Int32.Parse(id);

            var RoutineIncluded = (from p in _context.Patients
                                   join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                   join ri in _context.Routines on pa.patientAllocationID equals ri.patientAllocationID
                                   where p.isDeleted != 1
                                   where pa.isDeleted != 1
                                   where ri.isDeleted != 1
                                   where ri.isApproved == 1
                                   where p.isApproved == 1
                                   where pa.isApproved == 1
                                   where p.patientID == ID
                                   where ri.includeInSchedule == 1
                                   where p.isActive == 1
                                   //where ri.endDate >= DateTime.Today
                                   //where ri.endDate >= DateTime.Today || ri.endDate == null


                                   select new RoutineIncluded
                                   {
                                       routineIncluded = ri,


                                   }).ToList();

            var RoutineExcluded = (from p in _context.Patients
                                   join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                   join re in _context.Routines on pa.patientAllocationID equals re.patientAllocationID
                                   where p.isDeleted != 1
                                   where pa.isDeleted != 1
                                   where re.isDeleted != 1
                                   where re.isApproved == 1
                                   where p.isApproved == 1
                                   where pa.isApproved == 1
                                   where p.patientID == ID
                                   where re.includeInSchedule == 0
                                   where p.isActive == 1
                                   //where re.endDate >= DateTime.Today
                                   //where re.endDate >= DateTime.Today || re.endDate == null

                                   select new RoutineExcluded
                                   {
                                       routineExcluded = re,


                                   }).ToList();
            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => x.patientID == ID && x.isApproved == 1 && x.isDeleted != 1);

            //var ListOfPastRoutines = _context.Routines.Where(x => x.patientAllocationID == patientAllocation.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1 && DbFunctions.TruncateTime(x.endDate) < DbFunctions.TruncateTime(DateTime.Today)).ToList();


            var patient = _context.Patients.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();

            var activityList = _context.CentreActivities.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.isApproved == 1).ToList();

            CentreActivity activity = new CentreActivity();
            activity.activityTitle = "Others";
            activity.centreActivityID = -1;
            activityList.Add(activity);




            var viewModel = new RoutineViewModel()
            {
                patient = patient,
                ListOfRoutineExcluded = RoutineExcluded,
                ListOfRoutineIncluded = RoutineIncluded,
                activityList = activityList,
                //ListOfPastRoutines = ListOfPastRoutines,
            };

            return View(viewModel);



            //int ID = Int32.Parse(id);


            //var patient = _context.Patients.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            //var pa = _context.PatientAllocations.Where(x => x.patientID == ID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
            //var routineList = _context.Routines.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).ToList();
            ////var problemLog = _context.ProblemLogs.Where(x => x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            ////var probLogList = _context.CentreActivities.Where(x => x.isChecked == 1 && x.isDeleted != 1).ToList();


            //var viewModel = new RoutineViewModel()
            //{
            //    routineList =routineList,
            //    patient = patient,
            //};

            //return View(viewModel);
        }




        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult Adhoc()
        {
            //DateTime Date = DateTime.Now;
            // var activities = _context.CentreActivities.Select(x => x.activityTitle).Distinct().ToList();
            //ViewBag.ListOfActivities = activities;

            var patients = _context.Patients.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.isActive == 1 && (x.endDate > DateTime.Today || x.endDate == null)).Distinct().ToList();

            var activities = _context.CentreActivities.Where(x => x.isDeleted != 1).OrderBy(x => x.centreActivityID).ToList();

            var availableActivity = (from act in _context.CentreActivities
                                     where act.isDeleted != 1
                                     where act.isApproved == 1
                                     //where act.activityEndDate >= DateTime.Today || act.activityEndDate == null
                                     select new AvailableActivity
                                     {
                                         centreActivities = act,
                                         listAvailability = _context.ActivityAvailabilities.Where(x => x.centreActivityID == act.centreActivityID && x.isApproved == 1 && x.isDeleted != 1).ToList(),

                                     }).ToList();


            var viewModel = new ManageSupervisorsViewModel()
            {
                ListCentreActivities = activities,
                patientList = patients,
                AvailableActivity = availableActivity,
            };

            return View(viewModel);
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult AddMedicalHistory(MedicalHistoryViewModel viewModel)
        {
            var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            if (viewModel.medicalHist != null)
            {
                patientMethod.addMedicalHistory(supervisorID, patientAllocation.patientAllocationID, viewModel.medicalHist.informationSource, viewModel.medicalHist.medicalDetails, viewModel.medicalHist.notes, viewModel.medicalHist.medicalEstimatedDate, 1);
                TempData["success"] = "Successfully created a new medical record on " + DateTime.Now;
            }
            else
            {
                TempData["error"] = "Failed to add medical record";

            }
            return RedirectToAction("ManageMedicalHistory", "Supervisor", new { id = viewModel.patient.patientID });
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult AddVital(VitalViewModel viewModel)
        {
            var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            if (viewModel.vital != null)
            {
                int vitalID = patientMethod.addVital(supervisorID, patientAllocation.patientAllocationID, viewModel.vital.afterMeal, viewModel.vital.temperature, viewModel.vital.heartRate, viewModel.vital.systolicBP, viewModel.vital.diastolicBP, viewModel.vital.bloodSugarlevel, viewModel.vital.spO2, viewModel.vital.height, viewModel.vital.weight, viewModel.vital.notes, 1);

                //Vital vital = new Vital();
                //vital.isApproved = 1;
                //vital.patientAllocationID = patientAllocation.patientAllocationID;
                //vital.afterMeal = viewModel.vital.afterMeal;
                //vital.temperature = viewModel.vital.temperature;
                //vital.heartRate = viewModel.vital.heartRate;
                //vital.systolicBP = viewModel.vital.systolicBP;
                //vital.diastolicBP = viewModel.vital.diastolicBP;
                //vital.bloodPressure = viewModel.vital.systolicBP + "/" + viewModel.vital.diastolicBP; ;
                //vital.bloodSugarlevel = viewModel.vital.bloodSugarlevel;
                //vital.spO2 = viewModel.vital.spO2;
                //vital.height = viewModel.vital.height;
                //vital.weight = viewModel.vital.weight;
                //vital.notes = viewModel.vital.notes;

                //vital.createDateTime = DateTime.Now;
                //_context.Vitals.Add(vital);
                //_context.SaveChanges();
                //TempData["success"] = "Added new vital record successfully on " + DateTime.Now;

                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //var newLogData = new JavaScriptSerializer().Serialize(vital);

                //shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "vital", "ALL", null, null, vital.vitalID, 1, 0, null);

                TempData["success"] = "Successfully created a new vital record on " + DateTime.Now;

                //////highlights
                //Highlight hl = new Highlight();
                //var hltype = _context.HighlightType.SingleOrDefault(x => x.highlightType == "Abnormal Vital" && x.isApproved == 1 && x.isDeleted == 0);
                //hl.highlightTypeID = hltype.highlightTypeID;
                //hl.patientAllocationID = patientAllocation.patientAllocationID;

                ////Threshold
                //var tempthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "temperature").SingleOrDefault();
                //var bpSystolicthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "systolicBP").SingleOrDefault();
                //var bpDiastolicthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "diastolicBP").SingleOrDefault();
                //var spO2threshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "spO2").SingleOrDefault();

                //var bslBthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "bslBeforeMeal").SingleOrDefault();
                //var bslAthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "bslAfterMeal").SingleOrDefault();

                //var hrthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "heartRate").SingleOrDefault();


                ////JOBJECT
                //JObject JObj = new JObject();

                //JArray listOfAnormaly = new JArray();

                //JObj["vitalID"] = vitalID;

                //if (viewModel.vital.temperature >= tempthreshold.maxValue)
                //{
                //    JObject message = new JObject();
                //    message["category"] = "Temperature";
                //    //message["description"] = "Temperature above normal level";
                //    listOfAnormaly.Add(message);
                //}
                //else if (viewModel.vital.temperature < tempthreshold.minValue)
                //{
                //    JObject message = new JObject();

                //    message["category"] = "Temperature";
                //    //message["description"] = "Temperature below normal level";
                //    listOfAnormaly.Add(message);
                //}

                //if (viewModel.vital.systolicBP > bpSystolicthreshold.maxValue || viewModel.vital.diastolicBP > bpDiastolicthreshold.maxValue)
                //{
                //    JObject message = new JObject();
                //    message["category"] = "Blood Pressure";
                //    //message["description"] = "Blood Pressure above normal level";
                //    listOfAnormaly.Add(message);
                //}
                //else if (viewModel.vital.systolicBP < bpSystolicthreshold.minValue || viewModel.vital.diastolicBP < bpDiastolicthreshold.minValue)
                //{
                //    JObject message = new JObject();
                //    message["category"] = "Blood Pressure";
                //    //message["description"] = "Blood Pressure below normal level";
                //    listOfAnormaly.Add(message);


                //}


                //if (viewModel.vital.spO2 > spO2threshold.maxValue)
                //{
                //    JObject message = new JObject();
                //    message["category"] = "spO2";
                //    //message["description"] = "spO2 above normal level";
                //    listOfAnormaly.Add(message);

                //}
                //else if (viewModel.vital.spO2 < spO2threshold.minValue)
                //{
                //    JObject message = new JObject();
                //    message["category"] = "spO2";
                //    //message["description"] = "spO2 below normal level";
                //    listOfAnormaly.Add(message);

                //}

                //if (viewModel.vital.afterMeal == 1)
                //{
                //    if (viewModel.vital.bloodSugarlevel > bslAthreshold.maxValue)
                //    {
                //        JObject message = new JObject();
                //        message["category"] = "Blood Sugar Level";
                //        //message["description"] = "Blood Sugar Level above normal level";
                //        listOfAnormaly.Add(message);

                //    }
                //    else if (viewModel.vital.bloodSugarlevel < bslAthreshold.minValue)
                //    {
                //        JObject message = new JObject();
                //        message["category"] = "Blood Sugar Level";
                //        //message["description"] = "Blood Sugar Level below normal level";
                //        listOfAnormaly.Add(message);

                //    }
                //}
                //else
                //{
                //    if (viewModel.vital.bloodSugarlevel > bslBthreshold.maxValue)
                //    {
                //        JObject message = new JObject();
                //        message["category"] = "Blood Sugar Level";
                //        //message["description"] = "Blood Sugar Level above normal level";
                //        listOfAnormaly.Add(message);

                //    }
                //    else if (viewModel.vital.bloodSugarlevel < bslBthreshold.minValue)
                //    {
                //        JObject message = new JObject();
                //        message["category"] = "Blood Sugar Level";
                //        //message["description"] = "Blood Sugar Level below normal level";
                //        listOfAnormaly.Add(message);

                //    }
                //}

                //if (viewModel.vital.heartRate > hrthreshold.maxValue)
                //{
                //    JObject message = new JObject();
                //    message["category"] = "Heart Rate";
                //    //message["description"] = "Heart Rate above normal level";
                //    listOfAnormaly.Add(message);

                //}
                //else if (viewModel.vital.heartRate < hrthreshold.minValue)
                //{
                //    JObject message = new JObject();
                //    message["category"] = "Heart Rate";
                //    //message["description"] = "Heart Rate below normal level";
                //    listOfAnormaly.Add(message);

                //}

                //JObj["vital"] = new JArray(listOfAnormaly);
                ////JObj.Add("vital",listOfAnormaly);

                //hl.highlightData = JObj.ToString(Newtonsoft.Json.Formatting.None);

                //////date  
                //hl.startDate = DateTime.Today;
                //hl.endDate = DateTime.Today.AddDays(1);

                //hl.isApproved = 1;
                //hl.isDeleted = 0;
                //hl.createDateTime = DateTime.Now;
                //_context.Highlight.Add(hl);
                //_context.SaveChanges();
                //var newLogData = new JavaScriptSerializer().Serialize(hl);
                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                //shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "highlight", "ALL", null, null, hl.highlightID, 1, 0, null);



            }
            else
            {
                TempData["error"] = "Failed to add vital record.";

            }
            return RedirectToAction("ManageVital", "Supervisor", new { id = viewModel.patient.patientID });
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult AddPersoPreference(PersonalPreferenceViewModel viewModel)
        {
            var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            var socialHist = _context.SocialHistories.Where(x => x.patientAllocationID == patientAllocation.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            if (viewModel.preference.Equals("like"))
            {
                if (viewModel.likes != null)
                {

                    patientMethod.addLike(supervisorID, patientAllocation.patientAllocationID, viewModel.likes.likeItemID, viewModel.otherPreferences, 1);
                    //Like likes = new Like();

                    //likes.socialHistoryID = socialHist.socialHistoryID;
                    //likes.likeItemID = viewModel.likes.likeItemID;
                    //likes.isApproved = 1;
                    //likes.createdDateTime = DateTime.Now;

                    //_context.Likes.Add(likes);
                    //_context.SaveChanges();
                    TempData["success"] = "Successfully created a new preferences on " + DateTime.Now;
                    //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                    //var newLogData = new JavaScriptSerializer().Serialize(likes);

                    //shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "like", "ALL", null, null, likes.likeID, 1, 0, null);

                }
                else
                {
                    TempData["error"] = "Failed to add new preferences.";

                }
            }
            else if (viewModel.preference.Equals("dislike"))
            {
                if (viewModel.dislikes != null)
                {

                    patientMethod.addDislike(supervisorID, patientAllocation.patientAllocationID, viewModel.dislikes.dislikeItemID, viewModel.otherPreferences, 1);

                    //Dislike dislikes = new Dislike();

                    //dislikes.socialHistoryID = socialHist.socialHistoryID;
                    //dislikes.dislikeItemID = viewModel.dislikes.dislikeItemID;
                    //dislikes.isApproved = 1;
                    //dislikes.createdDateTime = DateTime.Now;

                    //_context.Dislikes.Add(dislikes);
                    //_context.SaveChanges();
                    TempData["success"] = "Successfully created a new preferences on " + DateTime.Now;
                    //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                    //var newLogData = new JavaScriptSerializer().Serialize(dislikes);
                    //shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "dislike", "ALL", null, null, dislikes.dislikeID, 1, 0, null);

                }
                else
                {
                    TempData["error"] = "Failed to add new preferences.";

                }
            }
            return RedirectToAction("ManagePersoPreference", "Supervisor", new { id = viewModel.patient.patientID });
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult AddAllergy(AllergyViewModel viewModel)
        {
            var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID 
                                                        && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            if (viewModel.allergyInput != null)
            {
                patientMethod.addAllergy(supervisorID, patientAllocation.patientAllocationID, viewModel.allergyInput.allergyListID, 
                    viewModel.otherAllergy, viewModel.allergyInput.reaction, viewModel.allergyInput.notes, 1);

                TempData["success"] = "Successfully created a new allergy on " + DateTime.Now;

                }
            else
            {
                TempData["error"] = "Failed to add allergy.";
            }
            return RedirectToAction("ManageAllergy", "Supervisor", new { id = viewModel.patient.patientID });
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult AddDementiaCondition(DementiaConditionViewModel viewModel)
        {
            var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            if (viewModel.dementiaInput != null)
            {
                patientMethod.addPatientAssignedDementia(supervisorID, patientAllocation.patientAllocationID, viewModel.dementiaInput.dementiaID, 1);
                TempData["success"] = "Successfully created a new problem log on " + DateTime.Now;

            }

            return RedirectToAction("ManageDementiaCondition", "Supervisor", new { id = viewModel.patient.patientID });
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult EditDementiaCondition(DementiaConditionViewModel viewModel)
        {
            var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            var pad = _context.PatientAssignedDementias.Where(x => x.padID == viewModel.dementiaInput.padID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            //var oldLogData = new JavaScriptSerializer().Serialize(pad);
            //string columnAffected = "";


            if (pad != null)
            {
                string result = patientMethod.updatePatientAssignedDementia(supervisorID, patientAllocation.patientAllocationID, viewModel.dementiaInput.padID, viewModel.dementiaInput.dementiaID, 1);

                if (result.Contains("success"))
                {
                    TempData["success"] = result;
                }
                else
                {
                    TempData["error"] = result;
                }

                //pad.dementiaID = viewModel.dementiaInput.dementiaID;
                //columnAffected = columnAffected + "dementiaID";

                //_context.SaveChanges();
                //TempData["success"] = "Changes saved successfully!!";
                //var newLogData = new JavaScriptSerializer().Serialize(pad);

                //string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                //string oldLogVal = logVal[0];
                //string newLogVal = logVal[1];

                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "patientAssignedDementia", columnAffected, oldLogVal, newLogVal, pad.padID, 1, 0, null);

            }
            else
            {
                TempData["error"] = "Failed to save changes.";

            }

            return RedirectToAction("ManageDementiaCondition", "Supervisor", new { id = viewModel.patient.patientID });
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult DeleteDementiaCondition(string padId, string patientId)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = Int32.Parse(patientId);

            int padID = Int32.Parse(padId);
            var pad = _context.PatientAssignedDementias.Where(x => x.padID == padID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            if (pad != null)
            {
                //var oldLogData = new JavaScriptSerializer().Serialize(pad);
                //pad.isDeleted = 1;
                //var newLogData = new JavaScriptSerializer().Serialize(pad);

                //_context.SaveChanges();
                patientMethod.deletePatientAssignedDementia(supervisorID, padID, patientAllocation.patientAllocationID, 1);
                TempData["success"] = "Successfully deleted a problem log record on " + DateTime.Now + ".";

                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "patientAssignedDementia", "isDeleted", null, null, ID, 1, 0, null);

            }
            else
            {
                TempData["error"] = "Failed to delete a problem log record.";

            }
            return RedirectToAction("ManageDementiaCondition", "Supervisor", new { id = patientId });

        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult UploadPhoto(HttpPostedFileBase file, PatientPhotoAlbumModel viewModel)
        {
            int patientID = viewModel.patient.patientID;
            var patient = _context.Patients.Where(x => x.patientID == patientID && x.isActive == 1 && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            var albumCatID = viewModel.inputAlbumPatient.albumCatID;

            var albumCategory = _context.AlbumCategories.Where(x => x.albumCatID == albumCatID && x.isDeleted != 1).SingleOrDefault();

            var countryID = viewModel.inputHoliday.countryID;
            var country = _context.ListCountries.Where(x => x.list_countryID == countryID && x.isDeleted != 1).SingleOrDefault();

            try
            {


                if (file != null)
                {

                    if (viewModel.inputAlbumPatient.albumCatID != 6)
                    {
                        if (albumCategory == null)
                        {
                            AlbumCategory albumCat = new AlbumCategory
                            {
                                albumCatName = viewModel.otherAlbumName,
                                isApproved = 1,
                                isDeleted = 0,

                                createDateTime = DateTime.Now
                            };
                            _context.AlbumCategories.Add(albumCat);
                            _context.SaveChanges();

                            albumCatID = albumCat.albumCatID;

                            var logData = new JavaScriptSerializer().Serialize(albumCat);
                            var logDesc = "New list item";
                            var logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocation.patientAllocationID, supervisorID, null, null, null, null, "albumCategory", "ALL", null, null, albumCatID, 1, 0, null);
                        }

                        var result = account.uploadPatientImage(Server, file, albumCatID, patientID, supervisorID, patient.firstName, patient.lastName, patient.maskedNric);

                        if (result == null)
                        {
                            TempData["error"] = "Error in uploading to cloudinary!";
                        }
                        TempData["success"] = "Image Uploaded Successfully!";


                    }
                    else {

                        if (country == null)
                        {
                            List_Country countryList = new List_Country
                            {
                                value = viewModel.otherCountry,
                                isChecked = 0,
                                isDeleted = 0,
                                createDateTime = DateTime.Now
                            };
                            _context.ListCountries.Add(countryList);
                            _context.SaveChanges();

                            countryID = countryList.list_countryID;

                            var logData = new JavaScriptSerializer().Serialize(countryList);
                            var logDesc = "New list item";
                            var logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocation.patientAllocationID, supervisorID, null, null, null, null, "list_country", "ALL", null, null, countryID, 1, 0, null);

                        }
                        var result = patientMethod.addHolidayExperience(Server, file, supervisorID, patientID, patientAllocation.patientAllocationID, viewModel.inputHoliday.countryID, viewModel.inputHoliday.holidayExp, viewModel.inputHoliday.startDate, viewModel.inputHoliday.endDate, 1);


                        if (result == null)
                        {
                            TempData["error"] = "Error in uploading to cloudinary!";
                        }
                        TempData["success"] = "Image Uploaded Successfully!";

                    }
                }
                else
                {

                    TempData["error"] = "No file chosen!";



                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }


            return RedirectToAction("ManagePhotoAlbum", "Supervisor", new { id = patientID });


        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult DeletePhoto(int albumID)
        {
            var patientAlbum = _context.AlbumPatient.Where(x => x.albumID == albumID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();

            var patientAllocation = _context.PatientAllocations.Where(x => x.patientAllocationID == patientAlbum.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            var patient = _context.Patients.Where(x => x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            if (patientAlbum != null)
            {
                string result = patientMethod.deleteImage(albumID);

                    TempData["success"] = result;
                
                //patientAlbum.isDeleted = 1;
                //_context.SaveChanges();

                //if (patientAlbum.albumPath.Contains("cloudinary"))
                //{
                //    var cloudinary = new Cloudinary(
                //          new CloudinaryDotNet.Account(
                //            "dbpearfyp",    // cloud name
                //            "996749534463792",  // api key
                //            "n7tw0oBbGMD1efIR-XhSsK4pw1s"));    // api secret


                //    string[] words = patientAlbum.albumPath.Split('/');
                //    var flag = 0;
                //    string imageIdentity = "";
                //    foreach (string word in words)
                //    {
                //        if (flag == 1)
                //        {
                //            string item = System.IO.Path.GetFileNameWithoutExtension(word);
                //            imageIdentity += "/" + item;
                //        }

                //        if (word.Equals("Patient"))
                //        {
                //            flag = 1;
                //            imageIdentity = "Patient";
                //        }
                //        else if (word.Equals("HolidayExperience"))
                //        {
                //            imageIdentity = "HolidayExperience";
                //            flag = 1;

                //        }



                //    }
                //    var deletionParams = new DeletionParams(imageIdentity)
                //    {
                //        PublicId = imageIdentity,
                //        Invalidate = true,
                //    };

                //    var delResult = cloudinary.Destroy(deletionParams);
                //    TempData["success"] = "Successfully deleted an image.";
                    //if (delResult.Result.Contains("ok"))
                    //{
                    //    TempData["success"] = "Successfully deleted an image.";
                    //}
                    //else
                    //{
                    //    TempData["error"] = "Error in deleting an image";
                    //}

                //}
            }
            else
            {
                TempData["error"] = "Failed to delete an image";
            }

            return RedirectToAction("ManagePhotoAlbum", "Supervisor", new { id = patient.patientID });


        }



        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult AddProblemLog(ProblemLogViewModel viewModel)
        {
            var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            if (viewModel.problemLog != null)
            {
                int problemLogID = patientMethod.addProblemLog(supervisorID, patientAllocation.patientAllocationID, viewModel.problemLog.categoryID, viewModel.problemLog.notes, 1);
                TempData["success"] = "Successfully created a new problem log on " + DateTime.Now;


                ////highlights
                //Highlight hl = new Highlight();
                //var hltype = _context.HighlightType.SingleOrDefault(x => x.highlightType == "Problem" && x.isApproved == 1 && x.isDeleted == 0);
                //hl.highlightTypeID = hltype.highlightTypeID;
                //hl.patientAllocationID = patientAllocation.patientAllocationID;

                //JObject JObj = new JObject();

                //JObj["problemLogID"] = problemLogID.ToString();

                //var probCat = _context.ListProblemLogs.SingleOrDefault(x => x.list_problemLogID == viewModel.problemLog.categoryID);
                //JObj["category"] = probCat.value;
                ////JObj["notes"] = pl.notes;



                //hl.highlightData = JObj.ToString(Newtonsoft.Json.Formatting.None);

                ////date  
                //hl.startDate = DateTime.Today;
                //hl.endDate = DateTime.Today.AddDays(1);

                //hl.isApproved = 1;
                //hl.isDeleted = 0;
                //hl.createDateTime = DateTime.Now;
                //_context.Highlight.Add(hl);
                //_context.SaveChanges();
                //var newLogData = new JavaScriptSerializer().Serialize(hl);
                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                //shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "highlight", "ALL", null, null, hl.highlightID, 1, 0, null);

            }
            else
            {
                TempData["error"] = "Failed to add problem log.";

            }
            return RedirectToAction("ManageProblemLog", "Supervisor", new { id = viewModel.patient.patientID });
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult AddActivityExclusion(ManagePreferencesViewModel viewModel)
        {
            var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            if (viewModel.actExInput != null)
            {
                int centreActivityID = Convert.ToInt32(Request.Form.Get("activityID"));

                //ActivityExclusion ae = new ActivityExclusion();
                //ae.centreActivityID = centreActivityID;
                //ae.dateTimeStart = viewModel.actExInput.dateTimeStart;
                //ae.dateTimeEnd = viewModel.actExInput.dateTimeEnd;
                //ae.patientAllocationID = patientAllocation.patientAllocationID;
                //ae.routineID = null;
                //ae.notes = viewModel.actExInput.notes;
                //ae.isApproved = 1;
                //ae.isDeleted = 0;
                //ae.createDateTime = DateTime.Now;

                //_context.ActivityExclusions.Add(ae);
                //_context.SaveChanges();

                int activityExclusionID = patientMethod.addActivityExclusion(supervisorID, patientAllocation.patientAllocationID, centreActivityID, viewModel.actExInput.dateTimeStart, viewModel.actExInput.dateTimeEnd, viewModel.actExInput.notes, 1);

                var p = _context.Patients.SingleOrDefault(x => x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1);
                p.updateBit = 1;
                _context.SaveChanges();


                TempData["success"] = "Successfully excluded an activity on " + DateTime.Now;
            
                ////highlights
                //Highlight hl = new Highlight();
                //var hltype = _context.HighlightType.SingleOrDefault(x => x.highlightType == "New Activity Exclusion" && x.isApproved == 1 && x.isDeleted == 0);
                //hl.highlightTypeID = hltype.highlightTypeID;
                //hl.patientAllocationID = patientAllocation.patientAllocationID;

                //JObject JObj = new JObject();

                //JObj["activityExclusionID"] = activityExclusionID;

                //var centreActivity = _context.CentreActivities.SingleOrDefault(x => x.centreActivityID == centreActivityID && x.isApproved == 1 && x.isDeleted != 1).activityTitle;
                //JObj["activityTitle"] = centreActivity;

                //hl.highlightData = JObj.ToString(Newtonsoft.Json.Formatting.None);

                ////date  
                //hl.startDate = viewModel.actExInput.dateTimeStart;

                //if ((viewModel.actExInput.dateTimeStart - viewModel.actExInput.dateTimeEnd).Days > 7)
                //{
                //    hl.endDate = DateTime.Today.AddDays(7);

                //}
                //else
                //{
                //    hl.endDate = viewModel.actExInput.dateTimeEnd;
                //}

                //hl.isApproved = 1;
                //hl.isDeleted = 0;
                //hl.createDateTime = DateTime.Now;
                //_context.Highlight.Add(hl);
                //_context.SaveChanges();
                //var newLogData = new JavaScriptSerializer().Serialize(hl);

                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                //shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "highlight", "ALL", null, null, hl.highlightID, 1, 0, null);


            }
            else
            {
                TempData["error"] = "Failed to exclude an activity.";

            }

            var referral = Request.UrlReferrer.ToString();

            if (referral.Contains("ManagePreference"))
            {

                return RedirectToAction("ManagePreference", "Supervisor", new { id = viewModel.patient.patientID });
            }
            else
            {
                return RedirectToAction("ManagePatientPreference", "Supervisor");

            }
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult IncludeActivityPref(string Id, string patientId)
        {

            int patientID = Convert.ToInt32(patientId);
            int exActID = Convert.ToInt32(Id);

            var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            var ae = _context.ActivityExclusions.SingleOrDefault(x => x.activityExclusionId == exActID && x.isApproved == 1 && x.isDeleted != 1);

            string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
            string logDescU = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

            if (ae != null)
            {

                string result = patientMethod.includeActivityPreference(supervisorID, patientAllocation.patientAllocationID, Convert.ToInt32(Id), 1);

                ////ae
                //oldLogData = new JavaScriptSerializer().Serialize(ae);
                //ae.dateTimeEnd = DateTime.Now;
                //newLogData = new JavaScriptSerializer().Serialize(ae);

                //_context.SaveChanges();

                //string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                //string oldLogVal = logVal[0];
                //string newLogVal = logVal[1];

                //shortcutMethod.addLogToDB(oldLogData, newLogData, logDescU, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "activityExclusion", "dateTimeEnd", oldLogVal, newLogVal, ae.activityExclusionId, 1, 0, null);


                ////patient

                //var p = _context.Patients.SingleOrDefault(x => x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1);
                //oldLogData = new JavaScriptSerializer().Serialize(p);
                //p.updateBit = 1;
                //newLogData = new JavaScriptSerializer().Serialize(p);

                //logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                //oldLogVal = logVal[0];
                //newLogVal = logVal[1];

                //shortcutMethod.addLogToDB(oldLogData, newLogData, logDescU, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "patient", "updateBit", oldLogVal, newLogVal, patientID, 1, 0, null);

                //TempData["success"] = "Successfully included an activity on " + DateTime.Now;

                TempData["success"] = result;

            }
            else
            {
                TempData["error"] = "Failed to include an activity.";

            }
            return RedirectToAction("ManagePreference", "Supervisor", new { id = patientID });
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult AddAllocation(AllocationViewModel viewModel)
        {
            var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            var oldLogData = new JavaScriptSerializer().Serialize(patientAllocation);

            if (viewModel.tempCaregiverID != null)
            {
                patientMethod.addTemporaryAllocation(supervisorID, patientAllocation.patientAllocationID, "tempCaregiverID", viewModel.tempCaregiverID, 1);


                //patientAllocation.tempCaregiverID = viewModel.tempCaregiver.tempCaregiverID;

                //_context.SaveChanges();
                //var LogData = new JavaScriptSerializer().Serialize(patientAllocation);

                TempData["success"] = "Successfully added a temporary caregiver on " + DateTime.Now;
                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //shortcutMethod.addLogToDB(oldLogData, LogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "patientAllocation", "tempCaregiverID", null, null, patientAllocation.patientAllocationID, 1, 0, null);

            }
            else if (viewModel.tempDoctorID != null)
            {
                patientMethod.addTemporaryAllocation(supervisorID, patientAllocation.patientAllocationID, "tempDoctorID", viewModel.tempDoctorID, 1);

                //patientAllocation.tempDoctorID = viewModel.tempCaregiver.tempDoctorID;

                //_context.SaveChanges();
                //var LogData = new JavaScriptSerializer().Serialize(patientAllocation);

                TempData["success"] = "Successfully added a temporary doctor on " + DateTime.Now;
                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                //shortcutMethod.addLogToDB(oldLogData, LogData, logDesc, 17, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "patientAllocation", "tempDoctorID", null, null, patientAllocation.patientAllocationID, 1, 0, null);


            }

            else
            {
                TempData["error"] = "Failed to add temporary allocation.";

            }
            return RedirectToAction("ManageAllocation", "Supervisor", new { id = viewModel.patient.patientID });
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult AddPrescription(PrescriptionViewModel viewModel)
        {
            var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
            string logDescList = _context.LogCategories.Where(x => x.logCategoryID == 19 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
            //var newLogData = "";

            if (viewModel.prescription != null)
            {
                int pscpID = patientMethod.addPrescription(supervisorID, patientAllocation.patientAllocationID, viewModel.mealID, viewModel.prescription.dosage, viewModel.prescription.drugNameID, viewModel.inputDrugName, viewModel.prescription.startDate, viewModel.prescription.endDate, viewModel.prescription.frequencyPerDay, viewModel.prescription.instruction, viewModel.prescription.isChronic, viewModel.prescription.notes, viewModel.prescription.timeStart, 1);
                TempData["success"] = "Successfully created a new prescription on " + DateTime.Now;
                

            }
            else
            {
                TempData["error"] = "Failed to add new prescription.";

            }
            return new RedirectResult(Url.Action("ManagePrescription", "Supervisor", new { id = viewModel.patient.patientID }) + "#prescription");

            //return RedirectToAction("ManagePrescription", "Supervisor", new { id = viewModel.patient.patientID });
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult AddMedicationLog(PrescriptionViewModel viewModel)
        {
            var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            if (viewModel.medication != null)
            {
                MedicationLog med = new MedicationLog();
                med.patientAllocationID = patientAllocation.patientAllocationID;

                med.prescriptionID = viewModel.medication.prescriptionID;
                med.userID = viewModel.medication.userID;
                med.dateTaken = viewModel.medication.dateTaken;
                med.timeForMedication = viewModel.medication.timeForMedication;
                med.dateForMedication = viewModel.medication.dateForMedication;
                med.timeTaken = viewModel.medication.timeTaken;
                med.createDateTime = DateTime.Now;
                _context.MedicationLog.Add(med);
                _context.SaveChanges();
                TempData["success"] = "Successfully created a new medication log on " + DateTime.Now;
                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                var newLogData = new JavaScriptSerializer().Serialize(med);

                shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "medicationLog", "ALL", null, null, med.medicationLogID, 1, 0, null);

            }
            else
            {
                TempData["error"] = "Failed to add new medication log.";

            }
            return RedirectToAction("ManagePrescription", "Supervisor", new { id = viewModel.patient.patientID });
        }



        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult AddRoutine(RoutineViewModel viewModel)
        {
            var patientAllocation = _context.PatientAllocations.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());

            var patient = _context.Patients.Where(x => x.patientID == viewModel.patient.patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();


            //var clashRoutine = _context.Routines.Where(x => (x.startDate >= viewModel.routine.endDate || viewModel.routine.startDate <= x.endDate) &&
            //(x.startTime >= viewModel.routine.endTime || viewModel.routine.startTime <= x.endTime)
            //&& x.isApproved == 1 && x.isDeleted != 1).ToList();

            string error = "Failed to add new routine.";
            if (viewModel.routine != null)
            {

                //if (clashRoutine.Count() == 0 && viewModel.routine.includeInSchedule == 1)
                //{


                //var arrayofDays = Request.Form["day[]"].Split(new[] { ',' });

                foreach (var day in viewModel.day)
                {

                    if (viewModel.day.Contains("Everyday"))
                    {
                        patientMethod.addRoutine(supervisorID, patientAllocation.patientAllocationID, viewModel.routine.centreActivityID, viewModel.routine.eventName,
                        viewModel.routine.startDate, viewModel.routine.endDate, viewModel.routine.startTime, viewModel.routine.endTime, "Everyday",
                        viewModel.routine.notes, viewModel.routine.reasonForExclude, viewModel.routine.concerningIssues, viewModel.routine.includeInSchedule, 1);

                        TempData["success"] = "Successfully created a new routine on " + DateTime.Now;

                        break;
                    }

                    patientMethod.addRoutine(supervisorID, patientAllocation.patientAllocationID, viewModel.routine.centreActivityID, viewModel.routine.eventName,
                        viewModel.routine.startDate, viewModel.routine.endDate, viewModel.routine.startTime, viewModel.routine.endTime, day,
                        viewModel.routine.notes, viewModel.routine.reasonForExclude, viewModel.routine.concerningIssues, viewModel.routine.includeInSchedule, 1);

                    TempData["success"] = "Successfully created a new routine on " + DateTime.Now;


                    //Routine routine = new Routine();

                    //routine.day = day;
                    //routine.patientAllocationID = patientAllocation.patientAllocationID;

                    //routine.includeInSchedule = viewModel.routine.includeInSchedule;

                    //if (viewModel.routine.includeInSchedule != 1)
                    //{
                    //    routine.reasonForExclude = viewModel.routine.reasonForExclude;

                    //}


                    //if (viewModel.routine.centreActivityID != -1)
                    //{
                    //    routine.centreActivityID = viewModel.routine.centreActivityID;
                    //    var activityName = _context.CentreActivities.Where(x => x.centreActivityID == viewModel.routine.centreActivityID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault().activityTitle;
                    //    routine.eventName = activityName;
                    //}
                    //else
                    //{
                    //    routine.eventName = viewModel.routine.eventName;


                    //}

                    //routine.startTime = viewModel.routine.startTime;
                    //routine.endTime = viewModel.routine.endTime;

                    //if (viewModel.routine.concerningIssues != null)
                    //{
                    //    routine.concerningIssues = viewModel.routine.concerningIssues;
                    //}


                    //routine.startDate = viewModel.routine.startDate;
                    //routine.endDate = viewModel.routine.endDate;
                    //routine.notes = viewModel.routine.notes;
                    //routine.isApproved = 1;
                    //routine.createDateTime = DateTime.Now;

                    //if (day.Equals("Everyday"))
                    //{
                    //    break;
                    //}
                    //_context.Routines.Add(routine);

                    //_context.SaveChanges();
                    //TempData["success"] = "Added new routine successfully on " + DateTime.Now;
                    //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                    //var newLogData = new JavaScriptSerializer().Serialize(routine);

                    //shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "routine", "ALL", null, null, routine.routineID, 1, 0, null);

                }
                patient.updateBit = 1;
                scheduler.generateWeeklySchedule(false, false);
                //}
                //else {
                //    TempData["error"] += error+"<br/>Routine clashes.";
                //}

            }
            else
            {
                TempData["error"] = error;


            }
            return RedirectToAction("ManageRoutine", "Supervisor", new { id = viewModel.patient.patientID });
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManagePatientAdhoc(int patientId)
        {
            //var activitiesList = _context.CentreActivities.Where(x => x.isDeleted != 1 && x.isApproved == 1).Distinct().OrderBy(x => x.activityTitle).ToList();
            //var activities = _context.CentreActivities.Select(x => x.activityTitle).Distinct().ToList();
            //ViewBag.ListOfActivities = activities;

            var actAvail = _context.ActivityAvailabilities.Where(x => x.isApproved == 1 && x.isDeleted != 1).GroupBy(x => x.centreActivityID).Select(g => g.FirstOrDefault()).ToList();

            List<CentreActivity> activitiesList = new List<CentreActivity>();
            //var activitiesList = _context.CentreActivities.Where(x => x.isDeleted != 1 && x.isApproved == 1).Distinct().OrderBy(x=>x.activityTitle).ToList();

            foreach (var item in actAvail)
            {
                CentreActivity centreActivity = new CentreActivity();
                centreActivity = _context.CentreActivities.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.centreActivityID == item.centreActivityID).SingleOrDefault();
                activitiesList.Add(centreActivity);
            }


            //TO BE EDITED:
            DateTime date = DateTime.Today;
            //DateTime date = DateTime.Today.AddDays(-5);

            DateTime firstDayofWeek = (DateTimeExtensions.FirstDayOfWeek(date)).AddDays(1);
            DateTime lastDayofWeek = (DateTimeExtensions.LastDayOfWeek(date)).AddDays(-1);


            var activity = (from s in _context.Schedules
                            join pa in _context.PatientAllocations on s.patientAllocationID equals pa.patientAllocationID
                            join p in _context.Patients on pa.patientID equals p.patientID
                            join c in _context.CentreActivities on s.centreActivityID equals c.centreActivityID
                            //join r in _context.Routines on p.patientID equals r.patientID
                            where s.isApproved == 1 && s.isDeleted != 1
                            where pa.isApproved == 1 && pa.isDeleted != 1
                            where p.isApproved == 1 && p.isDeleted != 1 && p.isActive == 1
                            where s.dateStart >= firstDayofWeek
                            where s.dateEnd <= lastDayofWeek
                            where p.patientID == patientId
                            where s.Routine == null
                            where c.isApproved == 1 && c.isDeleted != 1
                            select new PatientSchedule
                            {
                                scheduleId = s.scheduleID,
                                centreActivityID = c.centreActivityID,
                                centreActivityTitle = c.activityTitle,
                                patientId = p.patientID,
                                //routineName = r.eventName,
                                dateStart = (DateTime)s.dateStart,
                                dateEnd = (DateTime)s.dateEnd,
                                timeStart = (TimeSpan)s.timeStart,
                                timeEnd = (TimeSpan)s.timeEnd,
                            }).ToList();



            List<CentreActivityDetails> scheduledActivity = new List<CentreActivityDetails>();

            foreach (var x in activity.GroupBy(x => x.centreActivityID).Select(y => y.First()))
            {

                CentreActivityDetails cad = new CentreActivityDetails();
                cad.activityID = x.centreActivityID;
                cad.activityTitle = x.centreActivityTitle;
                scheduledActivity.Add(cad);
            }



            var viewModel = new ManageSupervisorsViewModel()
            {
                scheduleList = activity,
                ListCentreActivities = activitiesList,
                scheduledActivityList = scheduledActivity,
            };

            viewModel.patient = _context.Patients.Where(x => x.patientID == patientId && x.isActive == 1 && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            return View(viewModel);
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetScheduleInfo(int patientID, int activityID, string dateStart, string dateEnd)
        {
            //var prescription = _context.Prescriptions.Where(x => x.prescriptionID == id && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            ////TO BE EDITED:
            DateTime date = DateTime.Today;
            //DateTime date = DateTime.Today.AddDays(-7);

            DateTime firstDayofWeek = (DateTimeExtensions.FirstDayOfWeek(date)).AddDays(1);
            DateTime lastDayofWeek = (DateTimeExtensions.LastDayOfWeek(date)).AddDays(-1);


            var start = firstDayofWeek;
            var end = lastDayofWeek;

            if (dateStart != "")
            {
                start = DateTime.ParseExact(dateStart, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                //start = start.AddDays(-7);
            }

            if (dateEnd != "")
            {
                end = DateTime.ParseExact(dateEnd, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //end = end.AddDays(-7);

            }


            var activity = (from s in _context.Schedules
                            join pa in _context.PatientAllocations on s.patientAllocationID equals pa.patientAllocationID
                            join p in _context.Patients on pa.patientID equals p.patientID
                            join c in _context.CentreActivities on s.centreActivityID equals c.centreActivityID
                            where s.isApproved == 1 && s.isDeleted != 1
                            where pa.isApproved == 1 && pa.isDeleted != 1
                            where p.isApproved == 1 && p.isDeleted != 1 && p.isActive == 1
                            where s.dateStart >= start
                            where s.dateEnd <= end
                            where p.patientID == patientID
                            where s.Routine == null
                            where c.isApproved == 1 && c.isDeleted != 1
                            where c.centreActivityID == activityID
                            select new PatientSchedule
                            {
                                scheduleId = s.scheduleID,
                                centreActivityID = c.centreActivityID,
                                centreActivityTitle = c.activityTitle,
                                patientId = p.patientID,
                                dateStart = (DateTime)s.dateStart,
                                dateEnd = (DateTime)s.dateEnd,
                                dayName = System.Data.Entity.SqlServer.SqlFunctions.DateName("dw", s.dateStart),
                                timeStart = (TimeSpan)s.timeStart,
                                timeEnd = (TimeSpan)s.timeEnd,
                            }).ToList();


            return Json(activity);

        }

        //Original Code for ManagePatientAdhoc
        //[Authorize(Roles = RoleName.isSupervisor)]
        //public ActionResult ManagePatientAdhoc(int patientId)
        //{
        //    var activitiesList = _context.CentreActivities.Where(x => x.isDeleted != 1 && x.isApproved == 1).Distinct().OrderBy(x => x.activityTitle).ToList();
        //    //var activities = _context.CentreActivities.Select(x => x.activityTitle).Distinct().ToList();
        //    //ViewBag.ListOfActivities = activities;


        //    DateTime date = DateTime.Now;

        //    var activity = (from s in _context.Schedules
        //                    join pa in _context.PatientAllocations on s.patientAllocationID equals pa.patientAllocationID
        //                    join p in _context.Patients on pa.patientID equals p.patientID
        //                    join c in _context.CentreActivities on s.centreActivityID equals c.centreActivityID
        //                    //join r in _context.Routines on p.patientID equals r.patientID
        //                    where s.isApproved == 1 && s.isDeleted != 1
        //                    where pa.isApproved == 1 && pa.isDeleted != 1
        //                    where p.isApproved == 1 && p.isDeleted != 1 && p.isActive == 1
        //                    where s.dateStart == date.Date
        //                    where p.patientID == patientId
        //                    where s.Routine == null
        //                    where c.isApproved == 1 && c.isDeleted != 1
        //                    select new PatientSchedule
        //                    {
        //                        scheduleId = s.scheduleID,
        //                        centreActivityID = c.centreActivityID,
        //                        centreActivityTitle = c.activityTitle,
        //                        patientId = p.patientID,
        //                        //routineName = r.eventName,
        //                        dateStart = (DateTime)s.dateStart,
        //                        dateEnd = (DateTime)s.dateEnd,
        //                        timeStart = (TimeSpan)s.timeStart,
        //                        timeEnd = (TimeSpan)s.timeEnd,
        //                    }).ToList();

        //    var viewModel = new ManageSupervisorsViewModel()
        //    {
        //        scheduleList = activity,
        //        ListCentreActivities = activitiesList,
        //    };

        //    viewModel.patient = _context.Patients.Where(x => x.patientID == patientId && x.isActive == 1 && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

        //    return View(viewModel);
        //}

        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManagePatientAdhocMethod(ManageSupervisorsViewModel viewModel)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            //int supervisorID = Int32.Parse(Session["userID"].ToString());

            Schedule schedule = _context.Schedules.Where(x => x.scheduleID == viewModel.newScheduleId).SingleOrDefault();
            var oldData = new JavaScriptSerializer().Serialize(schedule);

            var patientAffected = (from s in _context.Schedules
                                   join pa in _context.PatientAllocations on s.patientAllocationID equals pa.patientAllocationID
                                   join p in _context.Patients on pa.patientID equals p.patientID
                                   where s.isApproved == 1 && s.isDeleted != 1
                                   where pa.isApproved == 1 && pa.isDeleted != 1
                                   where p.isApproved == 1 && p.isDeleted != 1
                                   where p.patientID == pa.patientID
                                   where s.scheduleID == viewModel.newScheduleId
                                   select p).SingleOrDefault();
            ViewBag.Error = "ViewModel: " + viewModel.id;

            schedule.centreActivityID = viewModel.id;

            _context.SaveChanges();

            var logData = new JavaScriptSerializer().Serialize(schedule);
            // shortcutMethod.addLogToDB(string? oldLogData, string logData, string logDesc, int logCategoryID, int? patientID, int userIDInit, int? userIDApproved, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            //shortcutMethod.addLogToDB(oldData, logData, "Update Adhoc info", 4, patientAffected.patientID, supervisorID, supervisorID, null, null, null, "Schedule", "centreActivityID ", "", "", 0, 0, 1, "");

            return RedirectToAction("ManagePatientAdhoc", "Supervisor", new { patientId = viewModel.newPatientId });
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public PartialViewResult GetAffectedPatient(int id, string endDate = null)
        {
            // ActivitiesList will be use for dropdown list
            var actAvail = _context.ActivityAvailabilities.Where(x => x.isApproved == 1 && x.isDeleted != 1).GroupBy(x => x.centreActivityID).Select(g => g.FirstOrDefault()).ToList();

            List<CentreActivity> activitiesList = new List<CentreActivity>();
            //var activitiesList = _context.CentreActivities.Where(x => x.isDeleted != 1 && x.isApproved == 1).Distinct().OrderBy(x=>x.activityTitle).ToList();

            foreach (var item in actAvail)
            {
                CentreActivity centreActivity = new CentreActivity();
                centreActivity = _context.CentreActivities.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.centreActivityID == item.centreActivityID).SingleOrDefault();
                activitiesList.Add(centreActivity);
            }

            //   var activitiesList = _context.CentreActivities.Where(x => x.isDeleted != 1 && x.isApproved == 1
            //&& x.centreActivityID == (_context.ActivityAvailabilities.Where(y => y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault().centreActivityID)).Distinct().OrderBy(x => x.activityTitle).ToList();

            //ViewBag.ListOfActivities = activities;

            DateTime Date, EndDate;

            if (endDate == null)
            {
                Date = DateTime.Now;
                EndDate = DateTime.Now;
            }
            else
            {
                Date = DateTime.Today;
                EndDate = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                //EndDate = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            var activities = _context.CentreActivities.Where(x => x.centreActivityID == id).SingleOrDefault();

            var patientList = (from s in _context.Schedules
                               join pa in _context.PatientAllocations on s.patientAllocationID equals pa.patientAllocationID
                               join p in _context.Patients on pa.patientID equals p.patientID
                               join c in _context.CentreActivities on s.centreActivityID equals c.centreActivityID
                               where s.isApproved == 1 && s.isDeleted != 1
                               where pa.isApproved == 1 && pa.isDeleted != 1
                               where p.isApproved == 1 && p.isDeleted != 1
                               where s.dateStart >= Date.Date && s.dateStart <= EndDate.Date
                               where s.centreActivityID == id
                               //where c.centreActivityID == id
                               select new PatientAdhocViewModel
                               {
                                   patientID = p.patientID,
                                   firstName = p.firstName,
                                   lastName = p.lastName,
                                   nric = p.nric,
                                   activityID = c.centreActivityID,
                                   scheduleID = s.scheduleID,
                               }).ToList();

            var dPatientList = patientList.GroupBy(x => x.patientID).Select(y => y.First()).ToList();

            //var activities = _context.CentreActivities.Where(x => x.isDeleted != 1).OrderBy(x => x.centreActivityID).ToList();

            var viewModel = new ManageSupervisorsViewModel()
            {
                adhocPatientList = dPatientList,
                ListCentreActivities = activitiesList,
                CentreActivities = activities,
            };

            return PartialView("_ManageAdhocPartial", viewModel);
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManageAdhoc(int id)
        {
            DateTime Date = DateTime.Now;
            DateTime EndDate = DateTime.Now;

            //DateTime Date = DateTime.Now;
            var activitiesList = _context.CentreActivities.Where(x => x.isDeleted != 1 && x.isApproved == 1).Distinct().OrderBy(x => x.activityTitle).ToList();

            //var activitiesList = _context.CentreActivities.Where(x => x.isDeleted != 1 && x.isApproved == 1
            //&& x.centreActivityID == (_context.ActivityAvailabilities.Where(y=>y.isApproved == 1 && y.isDeleted != 1).FirstOrDefault().centreActivityID)).Distinct().OrderBy(x => x.activityTitle).ToList();

            //var availableActivity = (from act in _context.CentreActivities
            //                         where act.isDeleted != 1
            //                         where act.isApproved == 1
            //                         //where act.activityEndDate >= DateTime.Today || act.activityEndDate == null
            //                         select new CentreActivity
            //                         {
            //                             centreActivities = act,
            //                             listAvailability = _context.ActivityAvailabilities.Where(x => x.centreActivityID == act.centreActivityID && x.isApproved == 1 && x.isDeleted != 1).ToList(),

            //                         }).ToList();


            //var actAvail = _context.ActivityAvailabilities.Where(x => x.isApproved == 1 && x.isDeleted != 1).GroupBy(x => x.centreActivityID).FirstOrDefault().ToList();

            //List<CentreActivity> activitiesList = new List<CentreActivity>();
            ////var activitiesList = _context.CentreActivities.Where(x => x.isDeleted != 1 && x.isApproved == 1).Distinct().OrderBy(x=>x.activityTitle).ToList();

            //foreach (var item in actAvail) {
            //    CentreActivity centreActivity= new CentreActivity();
            //    centreActivity = _context.CentreActivities.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.centreActivityID == item.centreActivityID).SingleOrDefault();
            //    activitiesList.Add(centreActivity);
            //}


            var activities = _context.CentreActivities.Where(x => x.centreActivityID == id).SingleOrDefault();

            var patientList = (from s in _context.Schedules
                               join pa in _context.PatientAllocations on s.patientAllocationID equals pa.patientAllocationID
                               join p in _context.Patients on pa.patientID equals p.patientID
                               join c in _context.CentreActivities on s.centreActivityID equals c.centreActivityID
                               where s.isApproved == 1 && s.isDeleted != 1
                               where pa.isApproved == 1 && pa.isDeleted != 1
                               where p.isApproved == 1 && p.isDeleted != 1
                               where s.dateStart >= Date.Date && s.dateStart <= EndDate.Date
                               where s.centreActivityID == id

                               //where c.centreActivityID == id
                               select new PatientAdhocViewModel
                               {
                                   patientID = p.patientID,
                                   firstName = p.firstName,
                                   lastName = p.lastName,
                                   nric = p.nric,
                                   activityID = c.centreActivityID,
                                   scheduleID = s.scheduleID,
                               }).ToList();





            var viewModel = new ManageSupervisorsViewModel()
            {
                adhocPatientList = patientList,
                ListCentreActivities = activitiesList,
                CentreActivities = activities,
            };

            return View(viewModel);
        }
        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManageAdhocByActivity(ManageSupervisorsViewModel viewModel, FormCollection form)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            PatientAllocation pa = _context.PatientAllocations.Where(x => x.patientID == viewModel.newPatientId && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            Patient p = _context.Patients.Where(x => x.patientID == viewModel.newPatientId && x.isActive == 1 && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            if (viewModel.inputAdhoc.oldCentreActivityID != viewModel.inputAdhoc.newCentreActivityID)
            {

                p.updateBit = 1;

                patientMethod.addAdHoc(supervisorID, pa.patientAllocationID, viewModel.inputAdhoc.newCentreActivityID, viewModel.inputAdhoc.oldCentreActivityID, viewModel.inputAdhoc.date, viewModel.inputAdhoc.endDate, 1);
                //AdHoc adhoc = new AdHoc();
                //adhoc.newCentreActivityID = viewModel.inputAdhoc.newCentreActivityID;
                //adhoc.oldCentreActivityID = viewModel.inputAdhoc.oldCentreActivityID;
                //adhoc.patientAllocationID = pa.patientAllocationID;
                //adhoc.endDate = viewModel.inputAdhoc.endDate;
                //adhoc.date = viewModel.inputAdhoc.date;
                //adhoc.isActive = 1;
                //adhoc.isApproved = 1;
                //adhoc.dateCreated = DateTime.Today;

                //_context.AdHocs.Add(adhoc);
                //_context.SaveChanges();

                //var newLogData = new JavaScriptSerializer().Serialize(adhoc);
                //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                //shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, pa.patientAllocationID, supervisorID, supervisorID, null, null, null, "adHoc", "ALL", null, null, adhoc.adhocID, 1, 0, null);

                ////generate schedule
                scheduler.generateWeeklySchedule(false, false);

                TempData["Success"] = "Successfully made an adhoc change.";
            }
            else
            {
                TempData["Error"] = "Failed to made an adhoc change.";

            }
            return RedirectToAction("ManagePatientAdhoc", "Supervisor", new { patientId = p.patientID });

        }


        [HttpPost]
        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManageAdhocMethod(ManageSupervisorsViewModel viewModel, FormCollection form)
        {
            int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
            //int supervisorID = Int32.Parse(Session["userID"].ToString());

            if (viewModel.adhocPatientList != null)
            {
                foreach (var scheduleAct in viewModel.adhocPatientList)
                {
                    // Check if the schedule is changed or the same, if the activity is not changed, there's no need to set updatebit = 1
                    if (viewModel.id != scheduleAct.activityID)
                    {
                        var pa = _context.PatientAllocations.Where(x => x.patientID == scheduleAct.patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

                        // Retrieve the date for start and end date ( adhoc multiple day range)
                        string startDateForm = form["startDate"];
                        string endDateForm = form["endDate"];


                        shortcutMethod.printf(startDateForm);
                        shortcutMethod.printf(endDateForm);

                        // Convert the date from string to DateTime format - "yy-mm-dd" instead of "MM/dd/yyyy"
                        DateTime startDate = DateTime.ParseExact(startDateForm, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime endDate = DateTime.ParseExact(endDateForm, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        // Find the number of day different to loop through 
                        double dayDiff = (endDate - startDate).TotalDays;
                        shortcutMethod.printf(dayDiff.ToString());

                        DateTime currentDay = DateTime.Now;

                        patientMethod.addAdHoc(supervisorID, pa.patientAllocationID, scheduleAct.activityID, viewModel.inputAdhoc.oldCentreActivityID, startDate, endDate, 1);

                        //AdHoc adhoc = new AdHoc();
                        //adhoc.newCentreActivityID = scheduleAct.activityID;
                        //adhoc.oldCentreActivityID = viewModel.inputAdhoc.oldCentreActivityID;
                        //adhoc.patientAllocationID = pa.patientAllocationID;
                        //adhoc.isActive = 1;
                        //adhoc.date = startDate;
                        //adhoc.endDate = endDate;
                        //adhoc.dateCreated = DateTime.Today;
                        //adhoc.isApproved = 1;
                        //_context.AdHocs.Add(adhoc);
                        //_context.SaveChanges();

                        //var logData = new JavaScriptSerializer().Serialize(adhoc);

                        //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                        //shortcutMethod.addLogToDB(null, logData, logDesc, 16, pa.patientAllocationID, supervisorID, supervisorID, null, null, null, "adHoc", "ALL", null, null, adhoc.adhocID, 1, 0, null);

                        //UpdateBitChanges ubc = new UpdateBitChanges();

                        // Loop through all the date
                        //for (int i = 0; i <= dayDiff; i++)
                        //{
                        //    //Schedule schedule = _context.Schedules.Where(x => x.scheduleID == scheduleAct.scheduleID).SingleOrDefault();

                        //    Patient patient = _context.Patients.Where(x => x.patientID == scheduleAct.patientID && x.isActive == 1 && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();
                        //    var schedule = (from s in _context.Schedules
                        //                    join pa in _context.PatientAllocations on s.patientAllocationID equals pa.patientAllocationID
                        //                    join p in _context.Patients on pa.patientID equals p.patientID
                        //                    join c in _context.CentreActivities on s.centreActivityID equals c.centreActivityID
                        //                    where s.isApproved == 1 && s.isDeleted != 1
                        //                    where pa.isApproved == 1 && pa.isDeleted != 1
                        //                    where p.isApproved == 1 && p.isDeleted != 1
                        //                    where s.dateStart == currentDay.Date
                        //                    where s.centreActivityID == viewModel.id
                        //                    where p.patientID == patient.patientID
                        //                    select s).SingleOrDefault();


                        //    if (schedule!= null)
                        //    {

                        //        Models.AdHoc adhoc = new Models.AdHoc();

                        //        // If it's today, you need to override without going in the midnight 
                        //        if (currentDay.Date == DateTime.Now.Date)
                        //        {
                        //            schedule.centreActivityID = scheduleAct.activityID;
                        //            _context.SaveChanges();

                        //            // Update the ad-hoc even after override
                        //            // Create Ad-hoc entry for midnight schedule override
                        //            var now = currentDay;

                        //            // Get Supervisor ID - Set inside HomeController.cs when loging in
                        //            //string userIDstring = Session["userID"].ToString();
                        //            int userID = Convert.ToInt32(User.Identity.GetUserID2());
                        //            //int userID = Int32.Parse(userIDstring);

                        //            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => x.patientID == patient.patientID && x.isApproved == 1 && x.isDeleted != 1);

                        //            adhoc.dateCreated = DateTime.Now;
                        //            adhoc.oldCentreActivityID = (int)schedule.centreActivityID;
                        //            adhoc.newCentreActivityID = scheduleAct.activityID;
                        //            adhoc.patientAllocationID = patientAllocation.patientAllocationID;
                        //            adhoc.isActive = 1;
                        //            adhoc.isApproved = 1;
                        //            adhoc.isDeleted = 0;
                        //            adhoc.date = now;       // This is the date for multiple day generation

                        //            patient.updateBit = 1;

                        //            _context.AdHocs.Add(adhoc);

                        //            _context.SaveChanges();
                        //            var logData = new JavaScriptSerializer().Serialize(adhoc);

                        //            string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                        //            shortcutMethod.addLogToDB(null, logData, logDesc, 16, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "adHoc", "ALL", null, null, adhoc.adhocID, 1, 0, null);


                        //            //ubc.adHocID = adhoc.adhocID;
                        //            //ubc.adhocIsActive = adhoc.isActive;
                        //            //ubc.createDateTime = DateTime.Now;
                        //            //_context.UpdateBitChanges.Add(ubc);
                        //            //_context.SaveChanges();

                        //            //string logDescC = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                        //            //var newLogData = new JavaScriptSerializer().Serialize(ubc);

                        //            //shortcutMethod.addLogToDB(null, newLogData, logDescC, 16, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "updateBitChanges", "ALL", null, null, ubc.updateBitChangesID, 1, 0, null);

                        //            //_context.SaveChanges();

                        //            //adhoc ubc


                        //        }
                        //        else
                        //        {
                        //            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => x.patientID == patient.patientID && x.isApproved == 1 && x.isDeleted != 1);

                        //            TempData["Success"] = "Successfully changed " + schedule.scheduleID + " activity from " + schedule.centreActivityID + " to " + scheduleAct.activityID;
                        //            //schedule.centreActivityID = scheduleAct.activityID;
                        //            patient.updateBit = 1;
                        //            _context.SaveChanges();

                        //            // Create Ad-hoc entry for midnight schedule override
                        //            var now = currentDay;

                        //            // Get Supervisor ID - Set inside HomeController.cs when loging in
                        //            //string userIDstring = Session["userID"].ToString();
                        //            int userID = Convert.ToInt32(User.Identity.GetUserID2());
                        //            //int userID = Int32.Parse(userIDstring);

                        //            adhoc.dateCreated = DateTime.Now;
                        //            adhoc.oldCentreActivityID = (int)schedule.centreActivityID;
                        //            adhoc.newCentreActivityID = scheduleAct.activityID;
                        //            adhoc.patientAllocationID = patientAllocation.patientAllocationID;
                        //            adhoc.isApproved = 1;
                        //            adhoc.isDeleted = 0;
                        //            adhoc.date = now;       // This is the date for multiple day generation

                        //            _context.AdHocs.Add(adhoc);
                        //            _context.SaveChanges();

                        //            //ubc.adHocID = adhoc.adhocID;
                        //            //ubc.adhocIsActive = adhoc.isActive;
                        //            //ubc.createDateTime = DateTime.Now;
                        //            //_context.UpdateBitChanges.Add(ubc);
                        //            //_context.SaveChanges();

                        //            //string logDescC = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                        //            //var newLogData = new JavaScriptSerializer().Serialize(ubc);

                        //            //    shortcutMethod.addLogToDB(null, newLogData, logDescC, 16, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "updateBitChanges", "ALL", null, null, ubc.updateBitChangesID, 1, 0, null);
                        //            //}

                        //            var logData = new JavaScriptSerializer().Serialize(adhoc);
                        //            string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                        //            shortcutMethod.addLogToDB(null, logData, logDesc, 16, patientAllocation.patientAllocationID, supervisorID, supervisorID, null, null, null, "adHoc", "ALL", null, null, adhoc.adhocID, 1, 0, null);
                        //            // shortcutMethod.addLogToDB(string? oldLogData, string logData, string logDesc, int logCategoryID, int? patientID, int userIDInit, int? userIDApproved, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                        //            //shortcutMethod.addLogToDB(null, logData, "Insert Adhoc info", 2, patient.patientID, supervisorID, supervisorID, null, null, null, "adHoc", "ALL", "", "", 0, 0, 1, "");

                        //            // Update Log Table
                        //            //var alia = Convert.ToInt32(User.Identity.GetUserID2());
                        //            //var now = DateTime.Now;
                        //            //var insertIntoLogTable = new Log()
                        //            //{
                        //            //    oldLogData = null,
                        //            //    logDesc = "UserID:" + user.userID + ";Action:DELETED USER;UserType:" + userType2 + ";",
                        //            //    tableAffected = "AspNetUsers Table",
                        //            //    columnAffected = "ALL",
                        //            //    patientID =  scheduleAct.patientID,  //For Temp only
                        //            //    rowAffected = user.userID,
                        //            //    userIDInit = alia,
                        //            //    userIDApproved = alia,
                        //            //    additionalInfo = null,
                        //            //    remarks = "ADHOC CHANGES - ACTIVITY",
                        //            //    logCategoryID = 4,            // TYPE_UPDATE_INFO_FIELD
                        //            //    createDateTime = now,
                        //            //    isDeleted = 0,
                        //            //    approved = 1,               //For Now
                        //            //    reject = 0,
                        //            //    supNotified = 0,
                        //            //    userNotified = 0,
                        //            //    rejectReason = null

                        //            //};

                        //            //_context.Logs.Add(insertIntoLogTable);
                        //            //_context.SaveChanges();
                        //        }

                        //        // Increment the day
                        //        currentDay = currentDay.AddDays(1);
                        //    }



                        //}

                    }
                }
                //generate schedule
                scheduler.generateWeeklySchedule(false, false);

                TempData["success"] = "Successfully made adhoc changes.";
            }
            else
            {
                TempData["error"] = "Failed to make adhoc changes.";

            }

            return RedirectToAction("Adhoc", "Supervisor");
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetPatientSchedule(int patientId)
        {
            List<List<PatientSchedule>> viewModel = new List<List<PatientSchedule>>();

            for (int i = -1; i <= 1; i++)
            {
                DateTime date = DateTime.Now;
                date = date.AddDays(i);

                var routine = (from s in _context.Schedules
                               join pa in _context.PatientAllocations on s.patientAllocationID equals pa.patientAllocationID
                               join p in _context.Patients on pa.patientID equals p.patientID
                               //join c in _context.CentreActivities on s.centreActivityID equals c.centreActivityID
                               join r in _context.Routines on p.patientID equals r.patientAllocationID
                               where s.isApproved == 1 && s.isDeleted != 1
                               where pa.isApproved == 1 && pa.isDeleted != 1
                               where p.isApproved == 1 && p.isDeleted != 1
                               where s.dateStart == date.Date
                               where p.patientID == patientId
                               where r.isApproved == 1 && r.isDeleted != 1
                               where s.CentreActivity == null
                               //from c in sc.DefaultIfEmpty()
                               select new PatientSchedule
                               {
                                   //centreActivityID = c.centreActivityID,
                                   //centreActivityTitle = c.activityTitle,
                                   scheduleId = s.scheduleID,
                                   routineName = r.eventName,
                                   dateStart = (DateTime)s.dateStart,
                                   dateEnd = (DateTime)s.dateEnd,
                                   timeStart = (TimeSpan)s.timeStart,
                                   timeEnd = (TimeSpan)s.timeEnd,
                               }).ToList();

                var activity = (from s in _context.Schedules
                                join pa in _context.PatientAllocations on s.patientAllocationID equals pa.patientAllocationID
                                join p in _context.Patients on pa.patientID equals p.patientID
                                join c in _context.CentreActivities on s.centreActivityID equals c.centreActivityID
                                //join r in _context.Routines on p.patientID equals r.patientID
                                where s.isApproved == 1 && s.isDeleted != 1
                                where pa.isApproved == 1 && pa.isDeleted != 1
                                where p.isApproved == 1 && p.isDeleted != 1
                                where s.dateStart == date.Date
                                where p.patientID == patientId
                                where s.Routine == null
                                where c.isApproved == 1 && c.isDeleted != 1
                                select new PatientSchedule
                                {
                                    scheduleId = s.scheduleID,
                                    centreActivityID = c.centreActivityID,
                                    centreActivityTitle = c.activityTitle,
                                    //routineName = r.eventName,
                                    dateStart = (DateTime)s.dateStart,
                                    dateEnd = (DateTime)s.dateEnd,
                                    timeStart = (TimeSpan)s.timeStart,
                                    timeEnd = (TimeSpan)s.timeEnd,
                                    patientname = p.firstName,

                                }).ToList();

                //var patient = activity.Union(routine).OrderBy( m => m.scheduleId ).ToList();
                var patient = activity.Concat(routine).OrderBy(m => m.scheduleId).ToList();

                if (patient.Count > 0)
                {
                    // Split the result into interval before adding it into the list
                    // This will make the front end side easier to work with

                    List<PatientSchedule> newSlot = new List<PatientSchedule>();

                    foreach (PatientSchedule data in patient)
                    {
                        TimeSpan timeDiff = data.timeEnd - data.timeStart;
                        // Getting Number of half hour slot for this activity
                        double noOfSlot = timeDiff.TotalMilliseconds / 1800000;

                        for (int x = 0; x < noOfSlot; x++)
                        {
                            newSlot.Add(data);
                        }

                    }

                    viewModel.Add(newSlot);
                    //viewModel.Add(patient);
                }
            }
            var pat = _context.Patients.SingleOrDefault(x => x.patientID == patientId && x.isApproved == 1 && x.isDeleted != 1);
            List<PatientSchedule> test = new List<PatientSchedule>();
            PatientSchedule d = new PatientSchedule();
            d.patientname = pat.firstName;
            test.Add(d);
            viewModel.Add(test);

            return Json(viewModel);
        }

        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManagePreference(int id)
        {
            // Get selected patients
            var p = _context.Patients.Where(x => x.patientID == id && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            var pa = _context.PatientAllocations.Where(x => x.patientID == id && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            //var ae = _context.ActivityExclusions.Where(x =>x.patientAllocationID == pa.patientAllocationID && x.isDeleted != 1 && x.isApproved == 1).ToList();

            var actPref = (from ca in _context.CentreActivities
                           join ap in _context.ActivityPreferences on ca.centreActivityID equals ap.centreActivityID
                           where ca.isApproved == 1
                           where ca.isDeleted != 1
                           where ap.isApproved == 1
                           where ap.isDeleted != 1
                           where ap.patientAllocationID == pa.patientAllocationID
                           select new PatientActivityPref
                           {
                               activityID = ap.centreActivityID,
                               actPreference = ap,
                               activityExcluded = _context.ActivityExclusions.FirstOrDefault(x => x.centreActivityID == ca.centreActivityID &&
                                                                    x.patientAllocationID == pa.patientAllocationID && x.isDeleted != 1 && x.isApproved == 1
                                                                    && x.dateTimeEnd > DateTime.Now),

                           }).ToList();


            var viewModel = new ManagePreferencesViewModel()
            {
                patient = p,
                ListOfActivity = actPref,
            };

            return View(viewModel);
        }




        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult ManagePatientPreference()
        {

            var centreActivity = _context.CentreActivities.FirstOrDefault(x => x.isApproved == 1 && x.isDeleted != 1);
            //var ae = _context.ActivityExclusions.Where(x =>x.patientAllocationID == pa.patientAllocationID && x.isDeleted != 1 && x.isApproved == 1).ToList();

            var centreActivityList = _context.CentreActivities.Where(x => x.isApproved == 1 && x.isDeleted != 1).ToList();

            var actPref = (from p in _context.Patients
                           join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                           join ap in _context.ActivityPreferences on pa.patientAllocationID equals ap.patientAllocationID
                           join ca in _context.CentreActivities on ap.centreActivityID equals ca.centreActivityID
                           where ca.isApproved == 1
                           where ca.isDeleted != 1
                           where ap.isApproved == 1
                           where ap.isDeleted != 1
                           where ap.patientAllocationID == pa.patientAllocationID
                           where p.isApproved == 1 && p.isDeleted != 1 && p.isActive == 1
                           where pa.isApproved == 1 && pa.isDeleted != 1
                           where ca.centreActivityID == centreActivity.centreActivityID
                           where p.endDate > DateTime.Today || p.endDate == null
                           select new PatientActivityPrefOverall
                           {
                               patient = p,
                               actPreference = ap,
                               activityExcluded = _context.ActivityExclusions.FirstOrDefault(x => x.centreActivityID == ca.centreActivityID &&
                                                                    x.patientAllocationID == pa.patientAllocationID && x.isDeleted != 1 && x.isApproved == 1
                                                                    && x.dateTimeEnd > DateTime.Now),

                           }).ToList();


            var viewModel = new ManagePreferencesViewModel()
            {
                centreActivity = centreActivity,
                ListOfActPref = actPref,
                activityList = centreActivityList,
            };

            return View(viewModel);
        }


        [Authorize(Roles = RoleName.isSupervisor)]
        public ActionResult GetPreferenceByActivity(int activityID)
        {



            var actPref = (from p in _context.Patients
                           join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                           join ap in _context.ActivityPreferences on pa.patientAllocationID equals ap.patientAllocationID
                           join ca in _context.CentreActivities on ap.centreActivityID equals ca.centreActivityID
                           where ca.isApproved == 1
                           where ca.isDeleted != 1
                           where ap.isApproved == 1
                           where ap.isDeleted != 1
                           where ap.patientAllocationID == pa.patientAllocationID
                           where p.isApproved == 1 && p.isDeleted != 1 && p.isActive == 1
                           where pa.isApproved == 1 && pa.isDeleted != 1
                           where ca.centreActivityID == activityID
                           where p.endDate > DateTime.Today || p.endDate == null

                           select new PatientActivityPrefOverall
                           {
                               patient = p,
                               actPreference = ap,
                               activityExcluded = _context.ActivityExclusions.FirstOrDefault(x => x.centreActivityID == ca.centreActivityID &&
                                                                    x.patientAllocationID == pa.patientAllocationID && x.isDeleted != 1 && x.isApproved == 1
                                                                    && x.dateTimeEnd > DateTime.Today),

                           }).ToList().OrderBy(x => x.patient.firstName);


            return Json(actPref);
        }



        

        // Save tick and cross event
        public ActionResult SetActivityPreference(int patientId, int centreActivityID, int status)
        {
            var patient = _context.Patients.Where(x => x.patientID == patientId && x.isApproved == 1 && x.isDeleted != 1 && x.isActive == 1).SingleOrDefault();
            patient.updateBit = 1;
            _context.SaveChanges();

            try
            {
                /// Get existing activity if exist
                var activityPreference = _context.ActivityPreferences.FirstOrDefault(x => x.patientAllocationID == patientId && x.centreActivityID == centreActivityID);
                bool isNew = false;
                /// Check activity preference exist or not, if not exist then create new
                if (activityPreference == null)
                {
                    isNew = true;
                    activityPreference = new ActivityPreference();
                    activityPreference.patientAllocationID = patientId;
                    activityPreference.centreActivityID = centreActivityID;
                    activityPreference.isApproved = 1;
                }

                // This string will be used for both
                string s1 = new JavaScriptSerializer().Serialize(activityPreference);
                int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
                string affectedColumn = "";

                //Update remark and log for unrecommend
                if (status == 2)
                {
                    if (activityPreference.isDislike == 1)
                    {
                        affectedColumn = affectedColumn + "isDislike ";
                        affectedColumn = affectedColumn + "isNeutral ";
                        activityPreference.isLike = 0;
                        activityPreference.isDislike = 0;
                        activityPreference.isNeutral = 1;
                    }
                    else
                    {
                        affectedColumn = affectedColumn + "isDislike ";
                        if (activityPreference.isLike == 1)
                            affectedColumn = affectedColumn + "isLike ";
                        else
                            affectedColumn = affectedColumn + "isNeutral ";
                        activityPreference.isLike = 0;
                        activityPreference.isDislike = 1;
                        activityPreference.isNeutral = 0;
                    }

                    //InsertLog(patientId, centreActivityID, status, remarks, isNew);
                }
                //Update remark and log for recommend
                if (status == 1)
                {
                    if (activityPreference.isLike == 1)
                    {
                        affectedColumn = affectedColumn + "isLike ";
                        affectedColumn = affectedColumn + "isNeutral ";

                        activityPreference.isLike = 0;
                        activityPreference.isDislike = 0;
                        activityPreference.isNeutral = 1;
                    }
                    else
                    {
                        affectedColumn = affectedColumn + "isLike ";
                        if (activityPreference.isDislike == 1)
                            affectedColumn = affectedColumn + "isDislike ";
                        else
                            affectedColumn = affectedColumn + "isNeutral ";
                        activityPreference.isLike = 1;
                        activityPreference.isDislike = 0;
                        activityPreference.isNeutral = 0;
                    }
                    //InsertLog(patientId, centreActivityID, status, remarks, isNew);
                }

                //Add new record
                if (isNew)
                {
                    activityPreference.isLike = 0;
                    activityPreference.isDislike = 0;
                    activityPreference.isNeutral = 1;
                    // Note:    s2 is difference from s1 as it does not store the like/dislike and neutral 
                    //          however, s1 must be initialized before the update of like, dislike and nuetral
                    string s2 = new JavaScriptSerializer().Serialize(activityPreference);

                    _context.ActivityPreferences.Add(activityPreference);
                    // shortcutMethod.addLogToDB(string? oldLogData, string logData, string logDesc, int logCategoryID, int? patientID, int userIDInit, int? userIDApproved, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)

                    string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;


                    shortcutMethod.addLogToDB(null, s2, logDesc, 16, activityPreference.patientAllocationID, supervisorID, supervisorID, null, null, null, "activityPreferences", "ALL", null, null, activityPreference.activityPreferencesID, 1, 0, null);

                }
                else
                {

                    Log logActPref = _context.Logs.FirstOrDefault(x => (x.isDeleted != 1 && x.approved == 0 && x.reject == 0 && x.tableAffected.Contains("ActivityPreferences") && x.rowAffected == activityPreference.activityPreferencesID));
                    if (logActPref != null)
                        return Json(false); //Send result to frontend. So, based on the result frontend can prompt a error message.

                    string s2 = new JavaScriptSerializer().Serialize(activityPreference);

                    int approved = 1;
                    int userIDApproved = 3; // Supervisor

                    string oldLogData = s1;
                    string logData = s2;

                    //string logDesc = "Update activityPreferences info for patient"; // Short details
                    int logCategoryID = 5; // choose categoryID
                    int userIDInit = Convert.ToInt32(User.Identity.GetUserID2());

                    string additionalInfo = null;
                    string remarks = null;
                    //string tableAffected = "ActivityPreferences";
                    string columnAffected = affectedColumn;
                    int rowAffected = activityPreference.activityPreferencesID;
                    int supNotified = 1;
                    int userNotified = 1;
                    // shortcutMethod.addLogToDB(string? oldLogData, string logData, string logDesc, int logCategoryID, int? patientID, int userIDInit, int? userIDApproved, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                    //shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, activityPreference.patientAllocationID, userIDInit, userIDApproved, additionalInfo,
                    //       remarks, tableAffected, columnAffected, "", "", supNotified, userNotified, approved, "");
                }

                _context.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        // Save tick and cross event
        public ActionResult SetExclusion(int patientId, int centreActivityID, int status, string startDate, string endDate, string notes)
        {
            try
            {
                if (status == 3)
                {
                    ActivityExclusion newActExclude = null;
                    // If activity is excluded
                    newActExclude = new ActivityExclusion();

                    newActExclude.patientAllocationID = patientId;
                    newActExclude.centreActivityID = centreActivityID;
                    //newActExclude.routineID = routineID;

                    newActExclude.notes = notes;
                    newActExclude.dateTimeStart = DateTime.Parse(startDate);
                    newActExclude.dateTimeEnd = DateTime.Parse(endDate);

                    newActExclude.isDeleted = 0;
                    newActExclude.isApproved = 1;

                    _context.ActivityExclusions.Add(newActExclude);

                    int supervisorID = Convert.ToInt32(User.Identity.GetUserID2());
                    // shortcutMethod.addLogToDB(string? oldLogData, string logData, string logDesc, int logCategoryID, int? patientID, int userIDInit, int? userIDApproved, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)

                    string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;


                    //shortcutMethod.addLogToDB(null, null, logDesc, 2, newActExclude.patientAllocationID, supervisorID, 3, null, null, "activityExclusions", "ALL", "", "", 1, 1, 1, "");
                }
                else if (status == 1 || status == 2)
                {
                    var excluded = _context.ActivityExclusions.Where(x => x.patientAllocationID == patientId && x.isApproved == 1 && x.isDeleted != 1 && x.centreActivityID == centreActivityID).SingleOrDefault();

                    if (excluded != null)
                    {
                        // Check if there's is a request from notification
                        Log logActExclsusion = _context.Logs.FirstOrDefault(x => (x.isDeleted != 1 && x.approved == 0 && x.reject == 0 && x.tableAffected.Contains("ActivityExclusion") && x.rowAffected == excluded.activityExclusionId));
                        if (logActExclsusion != null)
                            return Json(false);

                        // Store the unchanged excluded for logging
                        string s1 = new JavaScriptSerializer().Serialize(excluded);

                        excluded.isDeleted = 1;
                        String exclusionCol = "";
                        exclusionCol = exclusionCol + "isDeleted ";

                        string s2 = new JavaScriptSerializer().Serialize(excluded);

                        string oldLogDataExclusion = s1;
                        string logDataExclusion = s2;

                        //string logDescExclusion = "Delete ActivityExclusion info for patient"; // Short details
                        int logCategoryIDExclusion = 5; // choose categoryID
                        int userIDInit = Convert.ToInt32(User.Identity.GetUserID2());

                        string additionalInfo = null;
                        string remarks = null;
                        //string tableAffected = "activityExclusion";
                        string columnAffected = exclusionCol;
                        int rowAffected = excluded.activityExclusionId;
                        int supNotified = 1;
                        int userNotified = 1;
                        int approved = 1;
                        int userIDApproved = 3; // Supervisor
                        // shortcutMethod.addLogToDB(string? oldLogData, string logData, string logDesc, int logCategoryID, int? patientID, int userIDInit, int? userIDApproved, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                        //shortcutMethod.addLogToDB(oldLogDataExclusion, logDataExclusion, logDescExclusion, logCategoryIDExclusion, excluded.patientAllocationID, userIDInit, userIDApproved, additionalInfo,
                        //       remarks, tableAffected, columnAffected, "", "", supNotified, userNotified, approved, "");
                    }

                }

                Patient patient = _context.Patients.Where(x => x.patientID == patientId && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
                patient.updateBit = 1;

                _context.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        //public string[] GetLogVal(string oldLogData, string newLogData)
        //{
        //    JObject diffDatajOBJ = new JObject();
        //    JObject oldDatajObJ = new JObject();
        //    if (oldLogData.ToString() != "")
        //    {

        //        JObject sourceJObject = JsonConvert.DeserializeObject<JObject>(oldLogData);
        //        JObject targetJObject = JsonConvert.DeserializeObject<JObject>(newLogData);

        //        if (!JToken.DeepEquals(sourceJObject, targetJObject))
        //        {
        //            foreach (KeyValuePair<string, JToken> sourceProperty in sourceJObject)
        //            {
        //                JProperty targetProp = targetJObject.Property(sourceProperty.Key);

        //                if (!JToken.DeepEquals(sourceProperty.Value, targetProp.Value))
        //                {
        //                    diffDatajOBJ.Add(sourceProperty.Key, targetProp.Value);
        //                    oldDatajObJ.Add(sourceProperty.Key, sourceProperty.Value);
        //                }
        //            }
        //        }

        //    }

        //    string oldLogVal = oldDatajObJ.ToString();
        //    string newLogVal = diffDatajOBJ.ToString();


        //    string[] data = new string[2]; ;
        //    data[0] = oldLogVal;
        //    data[1] = newLogVal;

        //    return data;

        //}



    }
}

public static class DateTimeExtensions
{
    public static DateTime FirstDayOfWeek(this DateTime dt)
    {
        var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
        var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
        if (diff < 0)
            diff += 7;
        return dt.AddDays(-diff).Date;
    }

    public static DateTime LastDayOfWeek(this DateTime dt)
    {
        return dt.FirstDayOfWeek().AddDays(6);
    }

    public static DateTime FirstDayOfMonth(this DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, 1);
    }

    public static DateTime LastDayOfMonth(this DateTime dt)
    {
        return dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);
    }

    public static DateTime FirstDayOfNextMonth(this DateTime dt)
    {
        return dt.FirstDayOfMonth().AddMonths(1);
    }
}
