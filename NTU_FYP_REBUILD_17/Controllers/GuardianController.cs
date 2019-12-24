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
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Web.Routing;

namespace NTU_FYP_REBUILD_17.Controllers
{
    public class GuardianController : Controller
    {
        private ApplicationDbContext _context;
        private App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        Controllers.Synchronization.AccountMethod account = new Controllers.Synchronization.AccountMethod();
        Controllers.Synchronization.ScheduleMethod scheduler = new Controllers.Synchronization.ScheduleMethod();
        Controllers.Synchronization.PatientMethod patientMethod = new Controllers.Synchronization.PatientMethod();
        Controllers.Admin_page.ExportPatientScheduleController export = new Controllers.Admin_page.ExportPatientScheduleController();
        Controllers.Synchronization.ListMethod list = new Controllers.Synchronization.ListMethod();

        public GuardianController()
        {
            _context = new ApplicationDbContext();
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class NoDirectAccessAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (filterContext.HttpContext.Request.UrlReferrer == null ||
                            filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
                {
                    filterContext.Result = new RedirectToRouteResult(new
                                   RouteValueDictionary(new { controller = "Home", action = "Index", area = "" }));
                }
            }
        }

        // convert datetime to string
        public string getDate(DateTime? date)
        {
            if (date == null)
                return null;

            string year = ((DateTime)date).Year.ToString();
            string month = ((DateTime)date).Month.ToString();
            string day = ((DateTime)date).Day.ToString();

            string result = shortcutMethod.leadingZero(day) + "/" + shortcutMethod.leadingZero(month) + "/" + year;
            return result;
        }

        // GET: /Guardian/ManageGuardian
        [HttpGet]
        [Authorize(Roles = RoleName.isGuardian)]
        public ActionResult ManageGuardian(bool? showTAC)
        {
            ManageGuardiansViewModel model = new ManageGuardiansViewModel();
            model.patient = new List<PatientViewModel>();

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            List<PatientAllocation> mainPatient = _context.PatientAllocations.Where(x => (x.guardianID == userID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            List<PatientAllocation> secPatient = _context.PatientAllocations.Where(x => (x.guardian2ID == userID && x.isApproved == 1 && x.isDeleted != 1)).ToList();

            AlbumCategory profilePicture = _context.AlbumCategories.SingleOrDefault(x => (x.albumCatName == "Profile Picture" && x.isApproved == 1 && x.isDeleted != 1));
            int albumID = profilePicture.albumCatID;

            for (int i = 0; i < mainPatient.Count(); i++)
            {
                PatientViewModel patientViewModel = new PatientViewModel();

                int patientID = mainPatient[i].patientID;
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                if (patient == null)
                    continue;

                int patientAllocationID = mainPatient[i].patientAllocationID;
                patientViewModel.patientID = patient.patientID;
                patientViewModel.name = patient.firstName + " " + patient.lastName;
                patientViewModel.preferredName = patient.preferredName;
                patientViewModel.nric = patient.maskedNric;
                patientViewModel.startDate = getDate(patient.startDate);
                patientViewModel.endDate = getDate(patient.endDate);
                patientViewModel.isMainGuardian = 1;
                patientViewModel.warningBit = 0;

                List<AlbumPatient> mainAlbumPatient = _context.AlbumPatient.Where(x => (x.patientAllocationID == patientAllocationID && x.albumCatID == albumID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                if (mainAlbumPatient.Count > 0)
                {
                    string albumPath = mainAlbumPatient[mainAlbumPatient.Count - 1].albumPath;
                    patientViewModel.imagePath = albumPath;
                }

                if (patient.inactiveDate != null)
                {
                    patientViewModel.inactiveDate = getDate(patient.inactiveDate);
                }
                model.patient.Add(patientViewModel);
            }

            for (int i = 0; i < secPatient.Count(); i++)
            {
                PatientViewModel patientViewModel = new PatientViewModel();

                int patientID = secPatient[i].patientID;
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                if (patient == null)
                    continue;

                int patientAllocationID = secPatient[i].patientAllocationID;
                patientViewModel.patientID = patient.patientID;
                patientViewModel.name = patient.firstName + " " + patient.lastName;
                patientViewModel.preferredName = patient.preferredName;
                patientViewModel.nric = patient.maskedNric;
                patientViewModel.startDate = getDate(patient.startDate);
                patientViewModel.endDate = getDate(patient.endDate);
                patientViewModel.isMainGuardian = 0;
                patientViewModel.warningBit = 0;

                List<AlbumPatient> secAlbumPatient = _context.AlbumPatient.Where(x => (x.patientAllocationID == patientAllocationID && x.albumCatID == albumID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                if (secAlbumPatient.Count > 0)
                {
                    string albumPath = secAlbumPatient[secAlbumPatient.Count - 1].albumPath;
                    patientViewModel.imagePath = albumPath;
                }

                if (patient.inactiveDate != null)
                {
                    patientViewModel.inactiveDate = getDate(patient.inactiveDate);
                }
                model.patient.Add(patientViewModel);
            }
            if (showTAC != null && (showTAC == true))
                ViewBag.TAC = "true";

            model.patientID = 1;
            TempData["Message"] = "";
            return View(model);
        }

        // GET: /Guardian/ViewPatient
        [HttpGet]
        [Authorize(Roles = RoleName.isGuardian)]
        public ActionResult ViewPatient(int patientID)
        {
            //ViewData["Message"] = TempData["Message"];
            ViewData["Error1"] = TempData["Error1"];

            ViewBag.Modal = TempData["Modal"];
            ViewBag.Modal3 = TempData["Modal3"];

            ViewBag.Info = TempData["Info"];
            if (ViewBag.Info != "inactive")
                ViewBag.Info = "active";

            ViewBag.History = TempData["History"];
            if (ViewBag.History != "active")
                ViewBag.History = "inactive";

            ViewBag.Album = TempData["Album"];
            if (ViewBag.Album != "active")
                ViewBag.Album = "inactive";

            ViewBag.Privacy = TempData["Privacy"];
            if (ViewBag.Privacy != "active")
                ViewBag.Privacy = "inactive";

            DateTime date = DateTime.Today;

            Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            int patientAllocationID = patientAllocation.patientAllocationID;

            Language lang = _context.Languages.SingleOrDefault(x => (x.languageID == patient.preferredLanguageID && x.isApproved == 1 && x.isDeleted != 1));
            List_Language listLanguage = _context.ListLanguages.SingleOrDefault(x => (x.list_languageID == lang.languageListID && x.isDeleted != 1));
            string language = listLanguage.value;

            AlbumCategory profilePicture = _context.AlbumCategories.SingleOrDefault(x => (x.albumCatName == "Profile Picture" && x.isApproved == 1 && x.isDeleted != 1));
            int albumID = profilePicture.albumCatID;

            AlbumPatient patientProfilePicture = _context.AlbumPatient.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.albumCatID == albumID && x.isApproved == 1 && x.isDeleted != 1));

            var albumCat = (from ap in _context.AlbumPatient
                            join ac in _context.AlbumCategories on ap.albumCatID equals ac.albumCatID
                            where ap.patientAllocationID == patientAllocationID && ap.isApproved == 1 && ap.isDeleted != 1
                            where ac.isApproved == 1 && ac.isDeleted != 1
                            select new
                            {
                                albumCategory = ac,
                            }).Distinct().ToList();

            List<PatientAlbumViewModel> albumList = new List<PatientAlbumViewModel>();
            foreach (var cat in albumCat)
            {
                int albumCatID = cat.albumCategory.albumCatID;
                string albumName = cat.albumCategory.albumCatName;

                List<AlbumPatient> albumPatient = _context.AlbumPatient.Where(x => (x.albumCatID == albumCatID && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                albumList.Add(new PatientAlbumViewModel
                {
                    albumName = albumName,
                    albumPatient = albumPatient
                });
            }

            List<Vital> vital = _context.Vitals.Where(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            int count = 1;

            var diagnosedDementia = (from pad in _context.PatientAssignedDementias
                                     join d in _context.DementiaTypes on pad.dementiaID equals d.dementiaID
                                     where pad.patientAllocationID == patientAllocationID && pad.isApproved == 1 && pad.isDeleted != 1
                                     where d.isApproved == 1 && d.isDeleted != 1
                                     select new DementiaViewModel
                                     {
                                         pad = pad,
                                         dementia = d
                                     }).ToList();

            var patientInactivePrescription = (from p in _context.Patients
                                               join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                               join pscp in _context.Prescriptions on pa.patientAllocationID equals pscp.patientAllocationID
                                               where p.isDeleted != 1
                                               where pa.isDeleted != 1
                                               where pscp.isDeleted != 1
                                               where p.isApproved == 1
                                               where pa.isApproved == 1
                                               where pscp.isApproved == 1
                                               where p.patientID == patientID
                                               where pscp.endDate < date

                                               select new PatientPrescription
                                               {
                                                   prescription = pscp,
                                                   PrescriptionName = _context.ListPrescriptions.Where(x => x.list_prescriptionID == pscp.drugNameID && x.isDeleted != 1).FirstOrDefault().value,
                                               }).OrderByDescending(x => x.prescription.endDate).ToList();

            var patientActivePrescription = (from p in _context.Patients
                                             join pa in _context.PatientAllocations on p.patientID equals pa.patientID
                                             join pscp in _context.Prescriptions on pa.patientAllocationID equals pscp.patientAllocationID
                                             where p.isDeleted != 1
                                             where pa.isDeleted != 1
                                             where pscp.isDeleted != 1
                                             where p.isApproved == 1
                                             where pa.isApproved == 1
                                             where pscp.isApproved == 1
                                             where p.patientID == patientID
                                             where (pscp.endDate == null || pscp.endDate >= date)

                                             select new PatientPrescription
                                             {
                                                 prescription = pscp,
                                                 PrescriptionName = _context.ListPrescriptions.Where(x => x.list_prescriptionID == pscp.drugNameID && x.isDeleted != 1).FirstOrDefault().value,
                                             }).OrderBy(x => x.prescription.endDate).ToList();

            count = 1;
            List<PatientPrescription> patientPrescription = new List<PatientPrescription>();
            foreach (var prescription in patientActivePrescription)
            {
                prescription.status = 1;
                patientPrescription.Add(prescription);
                patientPrescription[count - 1].index = count++;
            }

            foreach (var prescription in patientInactivePrescription)
            {
                prescription.status = 0;
                patientPrescription.Add(prescription);
                patientPrescription[count - 1].index = count++;
            }

            var mobility = (from m in _context.Mobility
                            join ml in _context.ListMobility on m.mobilityListID equals ml.list_mobilityID
                            where m.patientAllocationID == patientAllocationID && m.isDeleted != 1
                            where ml.isDeleted != 1
                            select new MobilityViewModel
                            {
                                mobilityType = ml.value,
                                mobility = m
                            }).ToList();

            var medicationHistory = _context.MedicalHistory.Where(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();

            var dailyUpdates = (from du in _context.DailyUpdates
                                join u in _context.Users on du.userID equals u.userID
                                where du.patientAllocationID == patientAllocationID && du.isApproved == 1 && du.isDeleted != 1
                                select new PatientDailyUpdatesNoteViewModel
                                {
                                    dailyUpdates = du,
                                    user = u
                                }).OrderByDescending(x => x.dailyUpdates.createDateTime).ToList();

            var doctorNote = (from dn in _context.DoctorNotes
                              join u in _context.Users on dn.doctorID equals u.userID
                              where dn.patientAllocationID == patientAllocationID && dn.isApproved == 1 && dn.isDeleted != 1
                              select new PatientDoctorNoteViewModel
                              {
                                  doctorNote = dn,
                                  doctor = u
                              }).OrderByDescending(x => x.doctorNote.createDateTime).ToList();

            var socialHistory = (from s in _context.SocialHistories
                                 join d in _context.ListDiets on s.dietID equals d.list_dietID
                                 join e in _context.ListEducations on s.educationID equals e.list_educationID
                                 join lw in _context.ListLiveWiths on s.liveWithID equals lw.list_liveWithID
                                 join o in _context.ListOccupations on s.occupationID equals o.list_occupationID
                                 join p in _context.ListPets on s.petID equals p.list_petID
                                 join r in _context.ListReligions on s.religionID equals r.list_religionID
                                 where s.patientAllocationID == patientAllocationID && s.isApproved == 1 && s.isDeleted != 1
                                 where s.isDeleted != 1
                                 where s.isDeleted != 1
                                 where s.isDeleted != 1
                                 where s.isDeleted != 1
                                 where s.isDeleted != 1
                                 select new SocialHistoryViewModel
                                 {
                                     alcoholUse = s.alcoholUse == 1 ? "Yes" : s.alcoholUse == 0 ? "No": "N/A",
                                     caffeineUse = s.caffeineUse == 1 ? "Yes" : s.caffeineUse == 0 ? "No" : "N/A",
                                     drugUse = s.drugUse == 1 ? "Yes" : s.drugUse == 0 ? "No" : "N/A",
                                     exercise = s.exercise == 1 ? "Yes" : s.exercise == 0 ? "No" : "N/A",
                                     retired = s.retired == 1 ? "Yes" : s.retired == 0 ? "No" : "N/A",
                                     tobaccoUse = s.tobaccoUse == 1 ? "Yes" : s.tobaccoUse == 0 ? "No" : "N/A",
                                     secondhandSmoker = s.secondhandSmoker == 1 ? "Yes" : s.secondhandSmoker == 0 ? "No" : "N/A",
                                     sexuallyActive = s.sexuallyActive == 1 ? "Yes" : s.sexuallyActive == 0 ? "No" : "N/A",
                                     diet = d.value,
                                     education = e.value,
                                     liveWith = lw.value,
                                     occupation = o.value,
                                     pet = p.value,
                                     religion = r.value
                                 }).Single();

            var includedRoutine = (from r in _context.Routines
                                   where r.isDeleted != 1
                                   where r.patientAllocationID == patientAllocationID && r.includeInSchedule == 1
                                   select r).OrderBy(x => x.endDate).ToList();

            var excludedRoutine = (from r in _context.Routines
                                   where r.isDeleted != 1
                                   where r.patientAllocationID == patientAllocationID && r.includeInSchedule == 0
                                   select r).OrderByDescending(x => x.endDate).ToList();

            List<Routine> routineList = new List<Routine>();
            foreach (var routine in includedRoutine)
                routineList.Add(routine);
            

            foreach (var routine in excludedRoutine)
                routineList.Add(routine);
            

            var actPref = (from ca in _context.CentreActivities
                           join ap in _context.ActivityPreferences on ca.centreActivityID equals ap.centreActivityID
                           where ca.isApproved == 1
                           where ca.isDeleted != 1
                           where ap.isApproved == 1
                           where ap.isDeleted != 1
                           where ap.patientAllocationID == patientAllocationID
                           select new PatientActivityPref
                           {
                               activityID = ap.centreActivityID,
                               actPreference = ap,
                               activityExcluded = _context.ActivityExclusions.FirstOrDefault(x => x.centreActivityID == ca.centreActivityID &&
                                                                    x.patientAllocationID == patientAllocationID && x.isDeleted != 1 && x.isApproved == 1
                                                                    && x.dateTimeEnd > DateTime.Today),
                               preference = ap.isLike == 1 ? "Like" : ap.isDislike == 1 ? "Dislike" : "Neutral",
                               doctorName = _context.Users.FirstOrDefault(x => (x.userID == ap.doctorID)).firstName + " " + _context.Users.FirstOrDefault(x => (x.userID == ap.doctorID)).lastName,
                               doctorRemarks = ap.doctorRemarks,
                               recommendation = ap.doctorRecommendation == 1 ? "Recommended" : ap.doctorRecommendation == 0 ? "Not Recommended" : "No comment"
                           }).OrderByDescending(x => x.actPreference.CentreActivity.activityTitle).ToList();

            foreach (var pref in actPref)
            {
                if (pref.activityExcluded != null)
                    pref.activityDesc = "Excluded from " + scheduler.getDate(pref.activityExcluded.dateTimeStart) + " to " + scheduler.getDate(pref.activityExcluded.dateTimeEnd) + " due to " + pref.activityExcluded.notes;
            }

            var problemLogList = (from pl in _context.ProblemLogs
                                  join lpl in _context.ListProblemLogs on pl.problemLogID equals lpl.list_problemLogID
                                  where pl.isApproved == 1
                                  where pl.isDeleted != 1
                                  where lpl.isDeleted != 1
                                  select new PatientProblemLogViewModel
                                  {
                                      problemLog = pl,
                                      problemName = lpl.value
                                  }).OrderByDescending(x => x.problemLog.createdDateTime).ToList();

            List<StaffAllocationViewModel> staffAllocation = patientMethod.getStaffAllocation(patientAllocation);

            var game = (from ag in _context.AssignedGames
                        join g in _context.Games on ag.gameID equals g.gameID
                        where g.isApproved == 1 && g.isDeleted != 1
                        where ag.patientAllocationID == patientAllocationID && ag.isApproved == 1 && ag.isDeleted != 1
                        select new PatientGameViewModel
                        {
                            game = g,
                            assignedGame = ag,
                        }).ToList();

            foreach (var games in game)
            {
                string categoryString = "";
                List<GameCategory> gameCategory = _context.GameCategories.Where(x => (x.gameID == games.game.gameID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                foreach (var cat in gameCategory)
                {
                    if (categoryString != "")
                        categoryString += ", ";
                    Category category = _context.Categories.SingleOrDefault(x => (x.categoryID == cat.categoryID && x.isApproved == 1 && x.isDeleted != 1));
                    categoryString += category.categoryName;
                }
                games.gameCategory = categoryString;
            }

            List<PatientGameRecordViewModel> gameRecordList = new List<PatientGameRecordViewModel>();
            foreach (var games in game)
            {
                int assignedGameID = games.assignedGame.assignedGameID;
                string gameName = games.game.gameName;

                List<GameRecord> gameRecord = _context.GameRecords.Where(x => (x.assignedGameID == assignedGameID && x.isDeleted != 1)).ToList();
                if (gameRecord.Count > 0)
                {
                    gameRecordList.Add(new PatientGameRecordViewModel
                    {
                        gameName = gameName,
                        playCount = gameRecord.Count,
                        lastPlayed = gameRecord[gameRecord.Count - 1].createDateTime
                    });
                }
            }

            var likes = (from l in _context.Likes
                         join sh in _context.SocialHistories on l.socialHistoryID equals sh.socialHistoryID
                         join ll in _context.ListLikes on l.likeItemID equals ll.list_likeID
                         where l.isApproved == 1 && l.isDeleted != 1
                         where ll.isDeleted != 1
                         where sh.patientAllocationID == patientAllocationID && sh.isApproved == 1 && sh.isDeleted != 1
                         select new PatientLikeViewModel
                         {
                             like = l,
                             likeItem = ll.value
                         }).ToList();

            var holidayExperience = (from he in _context.HolidayExperiences
                                     join sh in _context.SocialHistories on he.socialHistoryID equals sh.socialHistoryID
                                     join lc in _context.ListCountries on he.countryID equals lc.list_countryID
                                     where he.isApproved == 1 && he.isDeleted != 1
                                     where lc.isDeleted != 1
                                     where sh.patientAllocationID == patientAllocationID && sh.isApproved == 1 && sh.isDeleted != 1
                                     select new PatientHolidayViewModel
                                     {
                                         holidayExperience = he,
                                         country = lc.value,
                                     }).ToList();

            foreach (var holiday in holidayExperience)
            {
                if (holiday.holidayExperience.albumPatientID != null)
                    holiday.albumPath = _context.AlbumPatient.SingleOrDefault(x => (x.albumID == holiday.holidayExperience.albumPatientID && x.isApproved == 1 && x.isDeleted != 1)).albumPath;
            }

            var hobby = (from h in _context.Hobbieses
                         join sh in _context.SocialHistories on h.socialHistoryID equals sh.socialHistoryID
                         join lh in _context.ListHobbies on h.hobbyListID equals lh.list_hobbyID
                         where h.isApproved == 1 && h.isDeleted != 1
                         where lh.isDeleted != 1
                         where sh.patientAllocationID == patientAllocationID && sh.isApproved == 1 && sh.isDeleted != 1
                         select new PatientHobbyViewModel
                         {
                             hobby = h,
                             hobbyItem = lh.value
                         }).ToList();

            var habit = (from h in _context.Habits
                         join sh in _context.SocialHistories on h.socialHistoryID equals sh.socialHistoryID
                         join lh in _context.ListHabits on h.habitListID equals lh.list_habitID
                         where h.isApproved == 1 && h.isDeleted != 1
                         where lh.isDeleted != 1
                         where sh.patientAllocationID == patientAllocationID && sh.isApproved == 1 && sh.isDeleted != 1
                         select new PatientHabitViewModel
                         {
                             habit = h,
                             habitItem = lh.value
                         }).ToList();

            var dislikes = (from d in _context.Dislikes
                            join sh in _context.SocialHistories on d.socialHistoryID equals sh.socialHistoryID
                            join ld in _context.ListDislikes on d.dislikeItemID equals ld.list_dislikeID
                            where d.isApproved == 1 && d.isDeleted != 1
                            where ld.isDeleted != 1
                            where sh.patientAllocationID == patientAllocationID && sh.isApproved == 1 && sh.isDeleted != 1
                            select new PatientDislikeViewModel
                            {
                                dislike = d,
                                dislikeItem = ld.value
                            }).ToList();

            var allergy = (from a in _context.Allergies
                           join la in _context.ListAllergy on a.allergyListID equals la.list_allergyID
                           where la.isDeleted != 1
                           where a.patientAllocationID == patientAllocationID && a.isApproved == 1 && a.isDeleted != 1
                           select new PatientAllergyViewModel
                           {
                               allergy = a,
                               allergyType = la.value
                           }).ToList();

            List<SelectListItem> albumSelectListItem = list.getAlbumList(1, true, true);
            ViewBag.albumType = new SelectList(albumSelectListItem, "Value", "Text");

            List<SelectListItem> daySelectListItem = list.getDaySelectListItem(patientID);
            ViewBag.day = new SelectList(daySelectListItem, "Value", "Text");

            List<SelectListItem> allergySelectListItem = list.getAllergyList(1, false, true);
            ViewBag.allergy = new SelectList(allergySelectListItem, "Value", "Text");

            List<SelectListItem> mobilitySelectListItem = list.getMobilityList(1, false, true);
            ViewBag.mobility = new SelectList(mobilitySelectListItem, "Value", "Text");

            List_Diet dietList = _context.ListDiets.SingleOrDefault(x => (x.value == socialHistory.diet));
            List<SelectListItem> dietSelectListItem = list.getDietList(1, true, true);
            ViewBag.diet = new SelectList(dietSelectListItem, "Value", "Text", dietList.list_dietID);

            List_Education educationList = _context.ListEducations.SingleOrDefault(x => (x.value == socialHistory.education));
            List<SelectListItem> educationSelectListItem = list.getEducationList(1, true);
            ViewBag.education = new SelectList(educationSelectListItem, "Value", "Text", educationList.list_educationID);

            List_LiveWith liveWithList = _context.ListLiveWiths.SingleOrDefault(x => (x.value == socialHistory.liveWith));
            List<SelectListItem> liveWithSelectListItem = list.getLiveWithList(1, true);
            ViewBag.liveWith = new SelectList(liveWithSelectListItem, "Value", "Text", liveWithList.list_liveWithID);

            List_Occupation occupationList = _context.ListOccupations.SingleOrDefault(x => (x.value == socialHistory.occupation));
            List<SelectListItem> occupationSelectListItem = list.getOccupationList(1, true);
            ViewBag.occupation = new SelectList(occupationSelectListItem, "Value", "Text", occupationList.list_occupationID);

            List_Pet petList = _context.ListPets.SingleOrDefault(x => (x.value == socialHistory.pet));
            List<SelectListItem> petSelectListItem = list.getPetList(1, true, true);
            ViewBag.pet = new SelectList(petSelectListItem, "Value", "Text", petList.list_petID);

            List_Religion religionList = _context.ListReligions.SingleOrDefault(x => (x.value == socialHistory.religion));
            List<SelectListItem> religionSelectListItem = list.getReligionList(1, true);
            ViewBag.religion = new SelectList(religionSelectListItem, "Value", "Text", religionList.list_religionID);

            List<SelectListItem> dislikeSelectListItem = list.getDislikeList(1, false, true);
            ViewBag.dislike = new SelectList(dislikeSelectListItem, "Value", "Text");

            List<SelectListItem> habitSelectListItem = list.getHabitList(1, false, true);
            ViewBag.habit = new SelectList(habitSelectListItem, "Value", "Text");

            List<SelectListItem> hobbySelectListItem = list.getHobbyList(1, false, true);
            ViewBag.hobby = new SelectList(hobbySelectListItem, "Value", "Text");

            List<SelectListItem> countrySelectListItem = list.getCountryList(1);
            ViewBag.country = new SelectList(countrySelectListItem, "Value", "Text");

            List<SelectListItem> likeSelectListItem = list.getLikeList(1, false, true);
            ViewBag.like = new SelectList(likeSelectListItem, "Value", "Text");

            List<SelectListItem> preferenceSelectListItem = new List<SelectListItem>();
            preferenceSelectListItem.Add(new SelectListItem() { Value = "0", Text = "Dislike" });
            preferenceSelectListItem.Add(new SelectListItem() { Value = "1", Text = "Like" });
            preferenceSelectListItem.Add(new SelectListItem() { Value = "2", Text = "Neutral" });
            ViewBag.preference = new SelectList(preferenceSelectListItem, "Value", "Text");

            int isMainGuardian = 0;
            int userInitID = Convert.ToInt32(User.Identity.GetUserID2());
            PatientAllocation mainGuardianAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            ApplicationUser mainGuardian = _context.Users.SingleOrDefault(x => (x.userID == mainGuardianAllocation.guardianID && x.isApproved == 1 && x.isDeleted != 1));
            if (mainGuardianAllocation.guardianID == userInitID)
                isMainGuardian = 1;

            ViewBag.date = DateTime.Today;
            PatientOverviewViewModel model = new PatientOverviewViewModel
            {
                patientID = patientID,
                patient = patient,
                imageUrl = patientProfilePicture.albumPath,
                albumList = albumList,
                language = language,
                diagnosedDementia = diagnosedDementia,
                socialHistory = socialHistory,
                patientPrescriptions = patientPrescription,
                routine = routineList,
                listOfActivity = actPref,
                patientProblemLog = problemLogList,
                staffAllocation = staffAllocation,
                mobility = mobility,
                medicalHistory = medicationHistory,
                like = likes,
                holiday = holidayExperience,
                hobby = hobby,
                habit = habit,
                dislike = dislikes,
                game = game,
                doctorNote = doctorNote,
                dailyUpdates = dailyUpdates,
                allergy = allergy,
                gameRecordList = gameRecordList,
                privacySettingsLifestyle = patientMethod.getPrivacySettings(patientAllocationID, "Lifestyle"),
                privacySettingsPersonal = patientMethod.getPrivacySettings(patientAllocationID, "Personal"),
                mainGuardian = mainGuardian,
                isMainGuardian = isMainGuardian
            };

            model.vital = new Vital();
            if (vital.Count > 1)
                model.vital = vital[vital.Count - 1];

            return View(model);
        }

        // GET: /Guardian/UpdatePatient
        [Authorize(Roles = RoleName.isGuardian)]
        public ActionResult UpdatePatient(int patientID)
        {
            Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            int patientAllocationID = patientAllocation.patientAllocationID;

            AlbumCategory profilePicture = _context.AlbumCategories.SingleOrDefault(x => (x.albumCatName == "Profile Picture" && x.isApproved == 1 && x.isDeleted != 1));
            int albumID = profilePicture.albumCatID;

            PatientUpdateViewModel model = new PatientUpdateViewModel();
            model.patient = patient;
            model.patientID = patientID;
            model.handphoneNo = patient.handphoneNo;
            model.tempAddress = patient.tempAddress;
            model.preferredName = patient.preferredName;

            var preferredLanguage = _context.ListLanguages.ToList();
            ViewBag.preferredLanguage = new SelectList(preferredLanguage, "list_languageID", "value", patient.Language.languageListID);

            List<AlbumPatient> secAlbumPatient = _context.AlbumPatient.Where(x => (x.patientAllocationID == patientAllocationID && x.albumCatID == albumID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            if (secAlbumPatient.Count > 0)
            {
                string albumPath = secAlbumPatient[secAlbumPatient.Count - 1].albumPath;
                model.imageUrl = albumPath;
            }

            ViewBag.Modal = TempData["Modal"];

            ViewBag.Info = TempData["Info"];
            if (ViewBag.Info != "inactive")
                ViewBag.Info = "active";

            ViewBag.Img = TempData["Img"];
            if (ViewBag.Img != "active")
                ViewBag.Img = "inactive";

            return View(model);
        }

        // POST: /Guardian/UpdatePatientInformation
        [Authorize(Roles = RoleName.isGuardian)]
        public ActionResult UpdatePatientInformation(PatientUpdateViewModel model)
        {
            int preferredLanguageID = Convert.ToInt32(Request.Form["preferredLanguage"]);

            var preferredLanguage = _context.ListLanguages.ToList();
            ViewBag.preferredLanguage = new SelectList(preferredLanguage, "list_languageID", "value", preferredLanguageID);

            TempData["Info"] = "active";
            TempData["Img"] = "inactive";

            if (ModelState.IsValid)
            {
                int userInitID = Convert.ToInt32(User.Identity.GetUserID2());
                int patientID = model.patientID;
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));

                //account.updateAccount(int userInitID, int userID, int userTypeID, string preferredName, string firstName, string lastName, string email, string address, string handphoneNo, string? officeNo, int isLocked)
                string result = patientMethod.updatePatient(userInitID, patientID, model.preferredName, model.tempAddress, model.handphoneNo, preferredLanguageID);

                if (result == "Update Successfully.")
                {
                    TempData["Message"] = "Updated personal info for patient: " + patient.preferredName;
                    TempData["Modal"] = "true";
                }
                else
                {
                    TempData["Message"] = "No changes are made for patient: " + patient.preferredName;
                    TempData["Modal"] = "true";
                }

                return RedirectToAction("ViewPatient", "Guardian", new { patientID = patientID });
            }
            // If we got this far, something failed, redisplay form
            return View("UpdatePatient", model);
        }

        // POST: /Guardian/uploadProfileImage
        [HttpPost]
        [Authorize]
        public ActionResult uploadProfileImage(HttpPostedFileBase file, PatientUpdateViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["Img"] = "active";
            TempData["Modal"] = "true";

            try
            {
                if (file != null)
                {
                    int userID = Convert.ToInt32(User.Identity.GetUserID2());
                    int patientID = model.patientID;

                    Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                    string firstName = patient.firstName;
                    string lastName = patient.lastName;
                    string maskedNric = patient.maskedNric;

                    string result = patientMethod.uploadProfileImage(Server, file, patientID, userID, firstName, lastName, maskedNric);

                    if (result == null)
                    {
                        TempData["Message"] = "Error in uploading to cloudinary";
                        return RedirectToAction("UpdatePatient", "Guardian", new { patientID = model.patientID });
                    }

                    TempData["Info"] = "active";
                    TempData["Message"] = "Image Uploaded Successfully";
                    return RedirectToAction("ViewPatient", "Guardian", new { patientID = patientID });
                }
                TempData["Message"] = "No file chosen!";
                return RedirectToAction("UpdatePatient", "Guardian", new { patientID = model.patientID });
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("UpdatePatient", "Guardian", new { patientID = model.patientID });
            }
        }

        // GET: /Guardian/GetSchedule
        [Authorize(Roles = RoleName.isGuardian)]
        public ActionResult GetSchedule(int patientID)
        {
            Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            int patientAllocationID = patientAllocation.patientAllocationID;

            DateTime date = DateTime.Today;

            DateTime startDate = scheduler.getFirstDate(patientAllocationID);
            DateTime lastDate = scheduler.getLastDate(patientAllocationID);

            PatientScheduleGuardianViewModel model = new PatientScheduleGuardianViewModel();
            model.patientID = patientID;
            model.preferredName = patient.preferredName;
            model.schedule = patientMethod.getDaySchedule(patientAllocationID, startDate, lastDate);
            model.scheduleString = model.schedule.ToString();
            model.date = scheduler.getDateFormat(date);

            return View(model);
        }

        // GET: /Guardian/GetAttendance
        [Authorize(Roles = RoleName.isGuardian)]
        public ActionResult GetAttendance(int patientID)
        {
            Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            int patientAllocationID = patientAllocation.patientAllocationID;

            DateTime date = DateTime.Today;

            DateTime startDate = scheduler.getFirstDate(patientAllocationID);

            JArray attendance = patientMethod.getDayAttendance(patientAllocationID, startDate, date);
            PatientAttendanceViewModel model = new PatientAttendanceViewModel
            {
                patient = patient,
                attendanceString = attendance.ToString(),
                date = scheduler.getDateFormat(date)
            };

            return View(model);
        }

        // POST: /Guardian/uploadImage
        [HttpPost]
        [Authorize(Roles = RoleName.isGuardian)]
        public ActionResult uploadImage(HttpPostedFileBase file, PatientOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "inactive";
            TempData["Album"] = "active";
            TempData["Privacy"] = "inactive";

            int albumCategoryID = Convert.ToInt32(Request.Form["albumType"]);

            AlbumCategory others = _context.AlbumCategories.SingleOrDefault(x => (x.albumCatName == "Others" && x.isApproved == 1 && x.isDeleted != 1));

            if (albumCategoryID == 0)
            {
                TempData["Error1"] = "Please select an album name!";
            }
            else if (albumCategoryID == -1 && (model.albumName == "" || model.albumName == null))
            {
                TempData["Error1"] = "Please specify the album name!";
            }
            else
            {
                try
                {
                    if (file != null)
                    {
                        int userID = Convert.ToInt32(User.Identity.GetUserID2());
                        int patientID = model.patientID;

                        Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                        string firstName = patient.firstName;
                        string lastName = patient.lastName;
                        string maskedNric = patient.maskedNric;

                        string result = account.uploadPatientImage(Server, file, albumCategoryID, patientID, userID, firstName, lastName, maskedNric);

                        if (result == null)
                        {
                            TempData["Error1"] = "Error in uploading to cloudinary!";
                        }
                        TempData["Error1"] = "Image Uploaded Successfully!";
                    }
                    else
                        TempData["Error1"] = "No file chosen!";
                }
                catch (Exception ex)
                {
                    TempData["Error1"] = ex.Message;
                }
            }
            TempData["Modal3"] = "true";
            return RedirectToAction("ViewPatient", "Guardian", new { patientID = model.patientID });
        }

        // POST: /Guardian/AddMedication
        [Authorize]
        public ActionResult AddMedication(PatientOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "active";
            TempData["Album"] = "inactive";
            TempData["Privacy"] = "inactive";

            if (ModelState.IsValid)
            {
                int userID = Convert.ToInt32(User.Identity.GetUserID2());
                int patientID = model.patientID;
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

                if (model.medicalDetails != "" && model.informationSource != "")
                {
                    patientMethod.addMedicalHistory(userID, patientAllocation.patientAllocationID, model.informationSource, model.medicalDetails, model.medicalNotes, model.medicalEstDate, 1);
                    
                    TempData["Message"] = "Added Medical History!";
                }
                else
                    TempData["Message"] = "Please key in the fields!";

                TempData["Modal"] = "true";
            }
            return RedirectToAction("ViewPatient", "Guardian", new { patientID = model.patientID });
        }

        // POST: /Guardian/UpdateMedicalHistory
        [Authorize]
        public ActionResult UpdateMedicalHistory(PatientOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "active";
            TempData["Album"] = "inactive";
            TempData["Privacy"] = "inactive";

            if (ModelState.IsValid)
            {
                int userID = Convert.ToInt32(User.Identity.GetUserID2());
                int patientID = model.patientID;
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

                int medicalHistoryID = model.itemID;
                patientMethod.updateMedicalHistory(userID, patientAllocation.patientAllocationID, medicalHistoryID, null, null, model.medicalNotes, null, 1);

                TempData["Message"] = "Updated Medical History!";
                TempData["Modal"] = "true";
            }
            return RedirectToAction("ViewPatient", "Guardian", new { patientID = model.patientID });
        }

        // POST: /Guardian/UpdatePreference
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGuardian)]
        public ActionResult UpdatePreference(DoctorOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "iactive";

            int preference = Convert.ToInt32(Request.Form["preference"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.updatePreference(userID, patientAllocation.patientAllocationID, model.activityTitle, preference, 1);
            TempData["Message"] = "Updated Preference!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Guardian", new { patientID = model.patientID });
        }

        // POST: /Guardian/AddMedication
        [Authorize]
        public ActionResult AddRoutine(PatientOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "active";
            TempData["Album"] = "inactive";
            TempData["Privacy"] = "inactive";

            if (ModelState.IsValid)
            {
                int userID = Convert.ToInt32(User.Identity.GetUserID2());
                int patientID = model.patientID;
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

                if (model.activityTitle != null && model.routineStartDate != null && model.routineEndDate != null && model.routineStartTime != null && model.routineEndTime != null && DateTime.Compare(model.routineStartDate, model.routineEndDate) <= 0 && TimeSpan.Compare(model.routineStartTime, model.routineEndTime) < 0)
                {
                    if (patient.isRespiteCare == 1 && model.monday && model.tuesday && model.wednesday && model.thursday && model.friday && model.saturday && model.sunday)
                        patientMethod.addRoutine(userID, patientAllocation.patientAllocationID, null, model.activityTitle, model.routineStartDate, model.routineEndDate, model.routineStartTime, model.routineEndTime, "Everyday", model.routineNotes, null, null, 0, 2);

                    else if (patient.isRespiteCare == 0 && model.monday && model.tuesday && model.wednesday && model.thursday && model.friday)
                        patientMethod.addRoutine(userID, patientAllocation.patientAllocationID, null, model.activityTitle, model.routineStartDate, model.routineEndDate, model.routineStartTime, model.routineEndTime, "Everyday", model.routineNotes, null, null, 0, 2);

                    else
                    {
                        if (model.monday)
                            patientMethod.addRoutine(userID, patientAllocation.patientAllocationID, null, model.activityTitle, model.routineStartDate, model.routineEndDate, model.routineStartTime, model.routineEndTime, "Monday", model.routineNotes, null, null, 0, 2);
                        if (model.tuesday)
                            patientMethod.addRoutine(userID, patientAllocation.patientAllocationID, null, model.activityTitle, model.routineStartDate, model.routineEndDate, model.routineStartTime, model.routineEndTime, "Tuesday", model.routineNotes, null, null, 0, 2);
                        if (model.wednesday)
                            patientMethod.addRoutine(userID, patientAllocation.patientAllocationID, null, model.activityTitle, model.routineStartDate, model.routineEndDate, model.routineStartTime, model.routineEndTime, "Wednesday", model.routineNotes, null, null, 0, 2);
                        if (model.thursday)
                            patientMethod.addRoutine(userID, patientAllocation.patientAllocationID, null, model.activityTitle, model.routineStartDate, model.routineEndDate, model.routineStartTime, model.routineEndTime, "Thursday", model.routineNotes, null, null, 0, 2);
                        if (model.friday)
                            patientMethod.addRoutine(userID, patientAllocation.patientAllocationID, null, model.activityTitle, model.routineStartDate, model.routineEndDate, model.routineStartTime, model.routineEndTime, "Friday", model.routineNotes, null, null, 0, 2);
                        if (model.saturday && patient.isRespiteCare == 1)
                            patientMethod.addRoutine(userID, patientAllocation.patientAllocationID, null, model.activityTitle, model.routineStartDate, model.routineEndDate, model.routineStartTime, model.routineEndTime, "Saturday", model.routineNotes, null, null, 0, 2);
                        if (model.sunday && patient.isRespiteCare == 1)
                            patientMethod.addRoutine(userID, patientAllocation.patientAllocationID, null, model.activityTitle, model.routineStartDate, model.routineEndDate, model.routineStartTime, model.routineEndTime, "Sunday", model.routineNotes, null, null, 0, 2);
                    }

                    TempData["Message"] = "Added Routine!";
                }
                else
                    TempData["Message"] = "Error in adding Routine!";
                TempData["Modal"] = "true";
            }
            return RedirectToAction("ViewPatient", "Guardian", new { patientID = model.patientID });
        }

        // POST: /Guardian/AddAllergy
        [Authorize]
        public ActionResult AddAllergy(PatientOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "active";
            TempData["Album"] = "inactive";
            TempData["Privacy"] = "inactive";

            int allergyListID = Convert.ToInt32(Request.Form["allergy"]);
            
            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            if (model.allergyReaction != "")
            {
                patientMethod.addAllergy(userID, patientAllocation.patientAllocationID, allergyListID, model.allergyName, model.allergyReaction, model.allergyNotes, 1);
                TempData["Message"] = "Added Allergy!";
            }
            else
                TempData["Message"] = "Please key in the fields!";

            TempData["Modal"] = "true";
            
            return RedirectToAction("ViewPatient", "Guardian", new { patientID = model.patientID });
        }

        // POST: /Guardian/UpdateSocialHistory
        [Authorize]
        public ActionResult UpdateSocialHistory(PatientOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "active";
            TempData["Album"] = "inactive";
            TempData["Privacy"] = "inactive";

            int alcoholUse = Convert.ToInt32(model.alcoholUse);
            int caffeineUse = Convert.ToInt32(model.caffeineUse);
            int drugUse = Convert.ToInt32(model.drugUse);
            int exercise = Convert.ToInt32(model.exercise);
            int retired = Convert.ToInt32(model.retired);
            int tobaccoUse = Convert.ToInt32(model.tobaccoUse);
            int secondhandSmoker = Convert.ToInt32(model.secondhandSmoker);
            int sexuallyActive = Convert.ToInt32(model.sexuallyActive);

            int dietListID = Convert.ToInt32(Request.Form["diet"]);
            int educationListID = Convert.ToInt32(Request.Form["education"]);
            int liveWithListID = Convert.ToInt32(Request.Form["liveWith"]);
            int occupationListID = Convert.ToInt32(Request.Form["occupation"]);
            int petListID = Convert.ToInt32(Request.Form["pet"]);
            int religionListID = Convert.ToInt32(Request.Form["religion"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.updateSocialHistory(userID, patientAllocation.patientAllocationID, alcoholUse, caffeineUse, drugUse, exercise, retired, tobaccoUse, secondhandSmoker, sexuallyActive, dietListID, model.dietName, educationListID, model.educationName, liveWithListID, model.liveWithName, petListID, model.petName, religionListID, model.religionName, occupationListID, model.occupationName, 1);
            TempData["Message"] = "Updated Social History!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Guardian", new { patientID = model.patientID });
        }

        // POST: /Guardian/AddDislikes
        [Authorize]
        public ActionResult AddDislikes(PatientOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "active";
            TempData["Album"] = "inactive";
            TempData["Privacy"] = "inactive";

            int dislikeListID = Convert.ToInt32(Request.Form["dislike"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.addDislike(userID, patientAllocation.patientAllocationID, dislikeListID, model.dislikeName, 1);
            TempData["Message"] = "Added Dislike!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Guardian", new { patientID = model.patientID });
        }

        // POST: /Guardian/AddHabits
        [Authorize]
        public ActionResult AddHabits(PatientOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "active";
            TempData["Album"] = "inactive";
            TempData["Privacy"] = "inactive";

            int habitListID = Convert.ToInt32(Request.Form["habit"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.addHabit(userID, patientAllocation.patientAllocationID, habitListID, model.habitName, 1);
            TempData["Message"] = "Added Habit!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Guardian", new { patientID = model.patientID });
        }

        // POST: /Guardian/AddHobbies
        [Authorize]
        public ActionResult AddHobbies(PatientOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "active";
            TempData["Album"] = "inactive";
            TempData["Privacy"] = "inactive";

            int hobbyListID = Convert.ToInt32(Request.Form["hobby"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.addHobby(userID, patientAllocation.patientAllocationID, hobbyListID, model.hobbyName, 1);
            TempData["Message"] = "Added Hobby!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Guardian", new { patientID = model.patientID });
        }

        //patientMethod.addHolidayExperience(int userInitID, int patientAllocationID, int countryListID, HttpServerUtilityBase Server, HttpPostedFileBase file, string holidayExperience, DateTime holidayStartDate, DateTime holidayEndDate, int isApproved)
        // POST: /Guardian/AddHolidayExperiences
        [Authorize]
        public ActionResult AddHolidayExperiences(HttpPostedFileBase file, PatientOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "active";
            TempData["Album"] = "inactive";
            TempData["Privacy"] = "inactive";

            int countryListID = Convert.ToInt32(Request.Form["country"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            string result = patientMethod.addHolidayExperience(Server, file, userID, patientID, patientAllocation.patientAllocationID, countryListID, model.holidayExperience, model.holidayStartDate, model.holidayEndDate, 1);

            if (result == "Image Uploaded Successfully!")
                TempData["Message"] = "Added Holiday Experience!";
            else
                TempData["Message"] = result;

            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Guardian", new { patientID = model.patientID });
        }

        // POST: /Guardian/AddLikes
        [Authorize]
        public ActionResult AddLikes(PatientOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "active";
            TempData["Album"] = "inactive";
            TempData["Privacy"] = "inactive";

            int likeListID = Convert.ToInt32(Request.Form["like"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.addLike(userID, patientAllocation.patientAllocationID, likeListID, model.likeName, 1);
            TempData["Message"] = "Added Like!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Guardian", new { patientID = model.patientID });
        }

        // POST: /Guardian/Delete
        [Authorize]
        public ActionResult Delete(PatientOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "active";
            TempData["Album"] = "inactive";
            TempData["Privacy"] = "inactive";

            string tableName = model.tableName;
            int itemID = model.itemID;
            string deleteReason = model.deleteReason;

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.delete(userID, patientAllocation.patientAllocationID, tableName, itemID, deleteReason);
            TempData["Message"] = "Item Deleted!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Guardian", new { patientID = model.patientID });
        }

        // POST: /Guardian/UpdatePrivacySettings
        [Authorize]
        public ActionResult UpdatePrivacySettings(PatientOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "inactive";
            TempData["Album"] = "inactive";
            TempData["Privacy"] = "active";

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));
            List<PrivacySettingsViewModel> privacySettingsLifestyle = model.privacySettingsLifestyle;
            List<PrivacySettingsViewModel> privacySettingsPersonal = model.privacySettingsPersonal;

            foreach(var settings in privacySettingsPersonal)
                privacySettingsLifestyle.Add(settings);

            patientMethod.updatePrivacySettings(userID, patientAllocation.patientAllocationID, privacySettingsLifestyle);
            TempData["Message"] = "Updated Privacy Settings!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Guardian", new { patientID = model.patientID });
        }

        // POST: /Guardian/ClearPrivacySettings
        [Authorize]
        public ActionResult ClearPrivacySettings(int patientID)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "inactive";
            TempData["Album"] = "inactive";
            TempData["Privacy"] = "active";

            return RedirectToAction("ViewPatient", "Guardian", new { patientID = patientID });
        }

        // POST: /Guardian/ResetPrivacySettings
        [Authorize]
        public ActionResult ResetPrivacySettings(int patientID)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "inactive";
            TempData["Album"] = "inactive";
            TempData["Privacy"] = "active";

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));
            List<PrivacySettingsViewModel> privacySettings = patientMethod.getDefaultPrivacySettings();

            patientMethod.updatePrivacySettings(userID, patientAllocation.patientAllocationID, privacySettings);
            TempData["Message"] = "Reset to default settings!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Guardian", new { patientID = patientID });
        }
    }
}