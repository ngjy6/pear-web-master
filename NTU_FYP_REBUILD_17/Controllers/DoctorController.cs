using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NTU_FYP_REBUILD_17.Controllers
{
    public class DoctorController : Controller
    {
        private ApplicationDbContext _context;
        private App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        Controllers.Synchronization.AccountMethod account = new Controllers.Synchronization.AccountMethod();
        Controllers.Synchronization.ScheduleMethod scheduler = new Controllers.Synchronization.ScheduleMethod();
        Controllers.Synchronization.PatientMethod patientMethod = new Controllers.Synchronization.PatientMethod();
        Controllers.Synchronization.PrivacyMethod privacyMethod = new Controllers.Synchronization.PrivacyMethod();
        Controllers.Admin_page.ExportPatientScheduleController export = new Controllers.Admin_page.ExportPatientScheduleController();
        Controllers.Synchronization.ListMethod list = new Controllers.Synchronization.ListMethod();

        public DoctorController()
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

        // GET: /Doctor/Index
        [HttpGet]
        [Authorize(Roles = RoleName.isDoctor)]
        public ActionResult Index()
        {
            DoctorIndexViewModel model = new DoctorIndexViewModel();
            model.patient = new List<PatientViewModel>();
            List<int> patientListID = new List<int>();

            DateTime date = DateTime.Today;

            int userID = Convert.ToInt32(User.Identity.GetUserID2());

            var activePatientList = (from pa in _context.PatientAllocations
                               join p in _context.Patients on pa.patientID equals p.patientID
                               join u in _context.Users on pa.doctorID equals u.userID
                               where u.userID == userID && u.isDeleted != 1
                               where DateTime.Compare(p.startDate, date) <= 0 && (p.endDate == null || DateTime.Compare((DateTime)p.endDate, date) >= 0) && (p.inactiveDate == null || DateTime.Compare((DateTime)p.inactiveDate, date) > 0) && p.isApproved == 1 && p.isDeleted != 1
                               where pa.isApproved == 1 && pa.isDeleted != 1
                               select new PatientAllocationViewModel
                               {
                                   patientID = pa.patientID,
                                   date = p.startDate
                               }).OrderByDescending(x => x.date).ToList();

            foreach (var patient in activePatientList)
            {
                patientListID.Add(patient.patientID);
            }

            var notStartedPatient = (from pa in _context.PatientAllocations
                                     join p in _context.Patients on pa.patientID equals p.patientID
                                     join u in _context.Users on pa.doctorID equals u.userID
                                     where u.userID == userID && u.isDeleted != 1
                                     where DateTime.Compare(p.startDate, date) > 0 && p.isApproved == 1 && p.isDeleted != 1
                                     where pa.isApproved == 1 && pa.isDeleted != 1
                                     select new PatientAllocationViewModel
                                     {
                                         patientID = pa.patientID,
                                         date = p.startDate
                                     }).OrderBy(x => x.date).ToList();

            foreach (var patient in notStartedPatient)
            {
                patientListID.Add(patient.patientID);
            }

            var endedPatientList = (from pa in _context.PatientAllocations
                                     join p in _context.Patients on pa.patientID equals p.patientID
                                     join u in _context.Users on pa.doctorID equals u.userID
                                     where u.userID == userID && u.isDeleted != 1
                                     where DateTime.Compare(p.startDate, date) <= 0 && ((p.endDate != null && DateTime.Compare((DateTime)p.endDate, date) < 0) || (p.inactiveDate != null && DateTime.Compare((DateTime)p.inactiveDate, date) <= 0)) && p.isApproved == 1 && p.isDeleted != 1
                                     where pa.isApproved == 1 && pa.isDeleted != 1
                                     select new PatientAllocationViewModel
                                     {
                                         patientID = pa.patientID,
                                         date = p.inactiveDate != null ? (DateTime)p.inactiveDate : (DateTime)p.endDate
                                     }).OrderByDescending(x => x.date).ToList();

            foreach (var patient in endedPatientList)
            {
                patientListID.Add(patient.patientID);
            }

            AlbumCategory profilePicture = _context.AlbumCategories.SingleOrDefault(x => (x.albumCatName == "Profile Picture" && x.isApproved == 1 && x.isDeleted != 1));
            int albumID = profilePicture.albumCatID;

            for (int i = 0; i < patientListID.Count(); i++)
            {
                PatientViewModel patientViewModel = new PatientViewModel();

                int patientID = patientListID[i];
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                if (patient == null)
                    continue;

                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID));
                int patientAllocationID = patientAllocation.patientAllocationID;
                patientViewModel.patientID = patient.patientID;
                patientViewModel.name = patient.firstName + " " + patient.lastName;
                patientViewModel.preferredName = patient.preferredName;
                patientViewModel.nric = patient.maskedNric;
                patientViewModel.startDate = getDate(patient.startDate);
                patientViewModel.endDate = getDate(patient.endDate);
                patientViewModel.warningBit = 0;

                List<AlbumPatient> mainAlbumPatient = _context.AlbumPatient.Where(x => (x.patientAllocationID == patientAllocationID && x.albumCatID == albumID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                if (mainAlbumPatient.Count > 0)
                {
                    string albumPath = mainAlbumPatient[mainAlbumPatient.Count - 1].albumPath;
                    patientViewModel.imagePath = albumPath;
                }

                if (patient.inactiveDate != null)
                    patientViewModel.inactiveDate = getDate(patient.inactiveDate);

                patientViewModel.isActive = 1;
                patientViewModel.status = "Active";

                if (DateTime.Compare(patient.startDate, date) > 0)
                {
                    patientViewModel.isActive = 2;
                    patientViewModel.status = "Not yet admitted";
                }
                else if (patient.endDate != null && DateTime.Compare((DateTime)patient.endDate, date) < 0)
                {
                    patientViewModel.isActive = 0;
                    patientViewModel.status = "Terminated";
                }

                else if (patient.inactiveDate != null && DateTime.Compare((DateTime)patient.inactiveDate, date) <= 0)
                {
                    patientViewModel.isActive = 0;
                    patientViewModel.status = "Inactive";
                }

                model.patient.Add(patientViewModel);
            }

            return View(model);
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

        // GET: /Guardian/ViewPatient
        [HttpGet]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isDoctor)]
        public ActionResult ViewPatient(int patientID)
        {
            ViewBag.Modal = TempData["Modal"];

            ViewBag.Info = TempData["Info"];
            if (ViewBag.Info != "inactive")
                ViewBag.Info = "active";

            ViewBag.History = TempData["History"];
            if (ViewBag.History != "active")
                ViewBag.History = "inactive";

            DateTime date = DateTime.Today;

            Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            int patientAllocationID = patientAllocation.patientAllocationID;

            AlbumCategory profilePicture = _context.AlbumCategories.SingleOrDefault(x => (x.albumCatName == "Profile Picture" && x.isApproved == 1 && x.isDeleted != 1));
            int albumID = profilePicture.albumCatID;

            AlbumPatient patientProfilePicture = _context.AlbumPatient.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.albumCatID == albumID && x.isApproved == 1 && x.isDeleted != 1));

            ApplicationUser mainGuardian = _context.Users.SingleOrDefault(x => (x.userID == patientAllocation.guardianID && x.isApproved == 1 && x.isDeleted != 1));
            PatientGuardian patientGuardian = _context.PatientGuardian.SingleOrDefault(x => (x.patientGuardianID == patient.patientGuardianID && x.isInUse == 1 && x.isDeleted != 1));

            string mainGuardianRelationship = _context.ListRelationships.SingleOrDefault(x => (x.list_relationshipID == patientGuardian.guardianRelationshipID && x.isDeleted != 1)).value;

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

            List<MedicalHistory> medicationHistory = _context.MedicalHistory.Where(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            List<Vital> vital = _context.Vitals.Where(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).OrderByDescending(x => x.createDateTime).ToList();

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

            var allergy = (from a in _context.Allergies
                           join la in _context.ListAllergy on a.allergyListID equals la.list_allergyID
                           where la.isDeleted != 1
                           where a.patientAllocationID == patientAllocationID && a.isApproved == 1 && a.isDeleted != 1
                           select new PatientAllergyViewModel
                           {
                               allergy = a,
                               allergyType = la.value
                           }).ToList();

            var dailyUpdates = (from du in _context.DailyUpdates
                                join u in _context.Users on du.userID equals u.userID
                                where du.patientAllocationID == patientAllocationID && du.isApproved == 1 && du.isDeleted != 1
                                select new PatientDailyUpdatesNoteViewModel
                                {
                                    dailyUpdates = du,
                                    user = u
                                }).OrderByDescending(x => x.dailyUpdates.createDateTime).ToList();

            var mobility = (from m in _context.Mobility
                            join ml in _context.ListMobility on m.mobilityListID equals ml.list_mobilityID
                            where m.patientAllocationID == patientAllocationID && m.isApproved == 1 && m.isDeleted != 1
                            where ml.isDeleted != 1
                            select new MobilityViewModel
                            {
                                mobilityType = ml.value,
                                mobility = m
                            }).ToList();

            var doctorNote = (from dn in _context.DoctorNotes
                              join u in _context.Users on dn.doctorID equals u.userID
                              where dn.patientAllocationID == patientAllocationID && dn.isApproved == 1 && dn.isDeleted != 1
                              select new PatientDoctorNoteViewModel
                              {
                                  doctorNote = dn,
                                  doctor = u
                              }).OrderByDescending(x => x.doctorNote.createDateTime).ToList();

            var actPref = (from ca in _context.CentreActivities
                           join ap in _context.ActivityPreferences on ca.centreActivityID equals ap.centreActivityID
                           where ca.isApproved == 1 && ca.isDeleted != 1 && ca.activityTitle != "Lunch"
                           where ap.isApproved == 1 && ap.isDeleted != 1 && ap.patientAllocationID == patientAllocationID
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

            var game = (from ag in _context.AssignedGames
                        join g in _context.Games on ag.gameID equals g.gameID
                        where g.isApproved == 1 && g.isDeleted != 1
                        where ag.patientAllocationID == patientAllocationID && ag.isApproved == 1 && ag.isDeleted != 1
                        select new PatientGameViewModel
                        {
                            game = g,
                            assignedGame = ag,
                            gameTherapistName = ag.GameTherapist.AspNetUsers.firstName + " " + ag.GameTherapist.AspNetUsers.lastName
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

            var gamesTypeRecommendation = (from gtr in _context.GamesTypeRecommendations
                                            join c in _context.Categories on gtr.gameCategoryID equals c.categoryID
                                            where c.isApproved == 1 && c.isDeleted != 1
                                            where gtr.patientAllocationID == patientAllocationID && gtr.isDeleted != 1
                                            select new PatientGameCategoryRecommendationViewModel
                                            {
                                                gameCategory = c,
                                                gamesTypeRecommendation = gtr,
                                                doctorName = gtr.doctorID == null ? null : _context.Users.FirstOrDefault(x => (x.userID == gtr.doctorID)).firstName + " " + _context.Users.FirstOrDefault(x => (x.userID == gtr.doctorID)).lastName,
                                                gameTherapistName = gtr.gameTherapistID == null ? null : _context.Users.FirstOrDefault(x => (x.userID == gtr.gameTherapistID)).firstName + " " + _context.Users.FirstOrDefault(x => (x.userID == gtr.gameTherapistID)).lastName,
                                            }).OrderByDescending(x => x.gamesTypeRecommendation.isApproved).OrderByDescending(x => x.gamesTypeRecommendation.createDateTime).ToList();

            List<StaffAllocationViewModel> staffAllocation = patientMethod.getStaffAllocation(patientAllocation);

            List<SelectListItem> dementiaSelectListItem = list.getDementiaList(1);
            ViewBag.dementia = new SelectList(dementiaSelectListItem, "Value", "Text");

            List<SelectListItem> prescriptionSelectListItem = list.getPrescriptionList(1, false, true);
            ViewBag.prescription = new SelectList(prescriptionSelectListItem, "Value", "Text");

            List<SelectListItem> mobilitySelectListItem = list.getMobilityList(1, false, true);
            ViewBag.mobility = new SelectList(mobilitySelectListItem, "Value", "Text");

            List<SelectListItem> takenSelectListItem = new List<SelectListItem>();
            takenSelectListItem.Add(new SelectListItem() { Value = "0", Text = "Before Meal" });
            takenSelectListItem.Add(new SelectListItem() { Value = "1", Text = "After Meal" });
            ViewBag.taken = new SelectList(takenSelectListItem, "Value", "Text");

            List<SelectListItem> periodSelectListItem = new List<SelectListItem>();
            periodSelectListItem.Add(new SelectListItem() { Value = "0", Text = "Short Term" });
            periodSelectListItem.Add(new SelectListItem() { Value = "1", Text = "Long Term" });
            ViewBag.period = new SelectList(periodSelectListItem, "Value", "Text");

            List<SelectListItem> recommendSelectListItem = new List<SelectListItem>();
            recommendSelectListItem.Add(new SelectListItem() { Value = "0", Text = "Not Recommended" });
            recommendSelectListItem.Add(new SelectListItem() { Value = "1", Text = "Recommended" });
            recommendSelectListItem.Add(new SelectListItem() { Value = "2", Text = "No comment" });
            ViewBag.recommend = new SelectList(recommendSelectListItem, "Value", "Text");

            List<SelectListItem> gameCategorySelectListItem = list.getGameCategoryList(1, false);
            for (int i = (gameCategorySelectListItem.Count - 1); i >= 0; i--)
            {
                for (int j = 0; j < gamesTypeRecommendation.Count; j++)
                {
                    if ((gamesTypeRecommendation[j].gamesTypeRecommendation.isApproved != 0 || gamesTypeRecommendation[j].gamesTypeRecommendation.supervisorApproved != 0 || gamesTypeRecommendation[j].gamesTypeRecommendation.gameTherapistApproved != 0) && gamesTypeRecommendation[j].gameCategory.categoryName == gameCategorySelectListItem[i].Text)
                    {
                        gameCategorySelectListItem.RemoveAt(i);
                        break;
                    }
                }
            }
            ViewBag.gameCategory = new SelectList(gameCategorySelectListItem, "Value", "Text");

            bool allowGame = false;
            ActivityPreference androidGamePref = _context.ActivityPreferences.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.CentreActivity.activityTitle == "Android Game" && x.doctorRecommendation == 1 && x.isApproved == 1 && x.isDeleted != 1));
            if (androidGamePref != null)
                allowGame = true;

            ViewBag.date = DateTime.Today;
            DoctorOverviewViewModel model = new DoctorOverviewViewModel
            {
                patientID = patientID,
                patient = patient,
                imageUrl = patientProfilePicture.albumPath,
                mainGuardianRelationship = mainGuardianRelationship,
                diagnosedDementia = diagnosedDementia,
                vital = vital,
                patientPrescriptions = patientPrescription,
                allergy = allergy,
                listOfActivity = actPref,
                patientProblemLog = problemLogList,
                staffAllocation = staffAllocation,
                mobility = mobility,
                medicalHistory = medicationHistory,
                game = game,
                allowGame = allowGame,
                gameRecommended = gamesTypeRecommendation,
                doctorNote = doctorNote,
                dailyUpdates = dailyUpdates,
                gameRecordList = gameRecordList,
            };

            if (mainGuardian != null)
                model.mainGuardian = mainGuardian;

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            model.patientLanguage = patientMethod.getPatientLanguage(userID, patientAllocationID);

            var socialHistory = (from s in _context.SocialHistories
                                 join d in _context.ListDiets on s.dietID equals d.list_dietID
                                 join e in _context.ListEducations on s.educationID equals e.list_educationID
                                 join lw in _context.ListLiveWiths on s.liveWithID equals lw.list_liveWithID
                                 join o in _context.ListOccupations on s.occupationID equals o.list_occupationID
                                 join p in _context.ListPets on s.petID equals p.list_petID
                                 join r in _context.ListReligions on s.religionID equals r.list_religionID
                                 where s.patientAllocationID == patientAllocationID && s.isApproved == 1 && s.isDeleted != 1
                                 where d.isDeleted != 1 && e.isDeleted != 1 && lw.isDeleted != 1 && o.isDeleted != 1 && p.isDeleted != 1 && r.isDeleted != 1
                                 select new SocialHistoryViewModel
                                 {
                                     alcoholUse = s.alcoholUse == 1 ? "Yes" : s.alcoholUse == 0 ? "No" : "N/A",
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

            int? showAlcoholUse = privacyMethod.getPatientAlcoholPrivacy(userID, patientAllocation);
            if (showAlcoholUse == null || showAlcoholUse == 0)
                socialHistory.alcoholUse = null;

            int? showCaffeineUse = privacyMethod.getPatientCaffeinePrivacy(userID, patientAllocation);
            if (showCaffeineUse == null || showCaffeineUse == 0)
                socialHistory.caffeineUse = null;

            int? showDrugUse = privacyMethod.getPatientDrugUsePrivacy(userID, patientAllocation);
            if (showDrugUse == null || showDrugUse == 0)
                socialHistory.drugUse = null;

            int? showExercise = privacyMethod.getPatientExercisePrivacy(userID, patientAllocation);
            if (showExercise == null || showExercise == 0)
                socialHistory.exercise = null;

            int? showRetired = privacyMethod.getPatientRetiredPrivacy(userID, patientAllocation);
            if (showRetired == null || showRetired == 0)
                socialHistory.retired = null;

            int? showTobaccoUse = privacyMethod.getPatientTobaccoUsePrivacy(userID, patientAllocation);
            if (showTobaccoUse == null || showTobaccoUse == 0)
                socialHistory.tobaccoUse = null;

            int? showSecondhandSmoker = privacyMethod.getPatientSecondhandSmokerPrivacy(userID, patientAllocation);
            if (showSecondhandSmoker == null || showSecondhandSmoker == 0)
                socialHistory.secondhandSmoker = null;

            int? showSexuallyActive = privacyMethod.getPatientSexuallyActivePrivacy(userID, patientAllocation);
            if (showSexuallyActive == null || showSexuallyActive == 0)
                socialHistory.sexuallyActive = null;

            int? showDiet = privacyMethod.getPatientDietPrivacy(userID, patientAllocation);
            if (showDiet == null || showDiet == 0)
                socialHistory.diet = null;

            int? showEducation = privacyMethod.getPatientEducationPrivacy(userID, patientAllocation);
            if (showEducation == null || showEducation == 0)
                socialHistory.education = null;

            int? showLiveWith = privacyMethod.getPatientLiveWithPrivacy(userID, patientAllocation);
            if (showLiveWith == null || showLiveWith == 0)
                socialHistory.liveWith = null;

            int? showOccupation = privacyMethod.getPatientOccupationPrivacy(userID, patientAllocation);
            if (showOccupation == null || showOccupation == 0)
                socialHistory.occupation = null;

            int? showPet = privacyMethod.getPatientPetPrivacy(userID, patientAllocation);
            if (showPet == null || showPet == 0)
                socialHistory.pet = null;

            int? showReligion = privacyMethod.getPatientReligionPrivacy(userID, patientAllocation);
            if (showReligion == null || showReligion == 0)
                socialHistory.religion = null;

            model.socialHistory = socialHistory;

            int? showDislike = privacyMethod.getPatientDislikePrivacy(userID, patientAllocation);
            if (showDislike == 1)
            {
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

                model.dislike = dislikes;
            }

            int? showHabit = privacyMethod.getPatientHabitPrivacy(userID, patientAllocation);
            if (showHabit == 1)
            {
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

                model.habit = habit;
            }

            int? showHobby = privacyMethod.getPatientHobbyPrivacy(userID, patientAllocation);
            if (showHobby == 1)
            {
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

                model.hobby = hobby;
            }

            int? showHolidayExperience = privacyMethod.getPatientHolidaExperiencePrivacy(userID, patientAllocation);
            if (showHolidayExperience == 1)
            {
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

                model.holiday = holidayExperience;
            }

            int? showLike = privacyMethod.getPatientLikePrivacy(userID, patientAllocation);
            if (showLike == 1)
            {
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

                model.like = likes;
            }
            return View(model);
        }

        // POST: /Doctor/Delete
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isDoctor)]
        public ActionResult Delete(DoctorOverviewViewModel model)
        {
            TempData["Info"] = "active";
            TempData["History"] = "inactive";

            string tableName = model.tableName;
            int itemID = model.itemID;
            string deleteReason = model.deleteReason;

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.delete(userID, patientAllocation.patientAllocationID, tableName, itemID, deleteReason);
            TempData["Message"] = "Item Deleted!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Doctor", new { patientID = model.patientID });
        }

        // POST: /Doctor/AddPatientAssignedDementia
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isDoctor)]
        public ActionResult AddPatientAssignedDementia(DoctorOverviewViewModel model)
        {
            TempData["Info"] = "active";
            TempData["History"] = "inactive";

            int dementiaID = Convert.ToInt32(Request.Form["dementia"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.addPatientAssignedDementia(userID, patientAllocation.patientAllocationID, dementiaID, 1);
            TempData["Message"] = "Added Dementia!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Doctor", new { patientID = model.patientID });
        }

        // POST: /Doctor/AddPrescription
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isDoctor)]
        public ActionResult AddPrescription(DoctorOverviewViewModel model)
        {
            TempData["Info"] = "active";
            TempData["History"] = "inactive";

            int prescriptionID = Convert.ToInt32(Request.Form["prescription"]);
            int beforeMeal = Convert.ToInt32(Request.Form["taken"]);
            int isChronic = Convert.ToInt32(Request.Form["period"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.addPrescription(userID, patientAllocation.patientAllocationID, beforeMeal, model.dosage, prescriptionID, model.prescriptionName, model.prescriptionStartDate, model.prescriptionEndDate, model.frequencyPerDay, model.instruction, isChronic, model.prescriptionNotes, model.prescriptionStartTime, 1);
            TempData["Message"] = "Added Prescription!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Doctor", new { patientID = model.patientID });
        }

        // POST: /Doctor/AddMobility
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isDoctor)]
        public ActionResult AddMobility(DoctorOverviewViewModel model)
        {
            TempData["Info"] = "active";
            TempData["History"] = "inactive";

            int mobilityListID = Convert.ToInt32(Request.Form["mobility"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.addMobility(userID, patientAllocation.patientAllocationID, mobilityListID, model.mobilityName, 1);
            TempData["Message"] = "Added Mobility Aids!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Doctor", new { patientID = model.patientID });
        }

        // POST: /Doctor/AddDoctorNote
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isDoctor)]
        public ActionResult AddDoctorNote(DoctorOverviewViewModel model)
        {
            TempData["Info"] = "active";
            TempData["History"] = "inactive";

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.addDoctorNote(userID, patientAllocation.patientAllocationID, model.doctorNotes, 1);
            TempData["Message"] = "Added Doctor Notes!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Doctor", new { patientID = model.patientID });
        }

        // POST: /Doctor/UpdateRecommendation
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isDoctor)]
        public ActionResult UpdateRecommendation(DoctorOverviewViewModel model)
        {
            TempData["Info"] = "active";
            TempData["History"] = "inactive";

            int recommend = Convert.ToInt32(Request.Form["recommend"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.updateRecommendation(userID, patientAllocation.patientAllocationID, model.activityTitle, recommend, model.doctorRemarks2, 1);
            TempData["Message"] = "Updated Recommendation!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Doctor", new { patientID = model.patientID });
        }

        // POST: /Doctor/AddGameRecommended
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isDoctor)]
        public ActionResult AddGameRecommended(DoctorOverviewViewModel model)
        {
            TempData["Info"] = "active";
            TempData["History"] = "inactive";

            int gameCategoryID = Convert.ToInt32(Request.Form["gameCategory"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            GamesTypeRecommendation gamesTypeRecommendation = _context.GamesTypeRecommendations.SingleOrDefault(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.gameCategoryID == gameCategoryID && x.isApproved != 0 && x.isDeleted != 1));
            if (gamesTypeRecommendation == null)
            {
                patientMethod.addGameCategoryRecommended(userID, patientAllocation.patientAllocationID, gameCategoryID, model.gameCategoryStartDate, model.gameCategoryEndDate, model.recommendationReason, 2);
                TempData["Message"] = "Game category recommendation requested! Waiting for Supervisor approval.";
            }
            else
                TempData["Message"] = "Game category of " + gamesTypeRecommendation.Category.categoryName + " has already been approved/requested for patient. Please select another game category!";

            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "Doctor", new { patientID = model.patientID });
        }

        // GET: /Doctor/GetAttendance
        [HttpGet]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isDoctor)]
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

        // GET: /Doctor/ManageDementiaGame
        [HttpGet]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isDoctor)]
        public ActionResult ManageDementiaGame()
        {
            ViewBag.Modal = TempData["Modal"];

            ViewBag.gameForDementia = TempData["gameForDementia"];
            if (ViewBag.gameForDementia != "inactive")
                ViewBag.gameForDementia = "active";

            ViewBag.gameCategoryForDementia = TempData["gameCategoryForDementia"];
            if (ViewBag.gameCategoryForDementia != "active")
                ViewBag.gameCategoryForDementia = "inactive";

            DoctorDementiaGameViewModel model = new DoctorDementiaGameViewModel();
            List<DementiaGameViewModel> dementiaList = new List<DementiaGameViewModel>();
            List<DementiaGameCategoryViewModel> dementiaCategoryList = new List<DementiaGameCategoryViewModel>();

            List<DementiaType> dementiaTypes = _context.DementiaTypes.Where(x => (x.isApproved == 1 && x.isDeleted != 1)).OrderBy(x => x.dementiaType).ToList();

            int count = 1;
            foreach (var dementiaType in dementiaTypes)
            {
                if (count%3 != 1)
                {
                    count++;
                    continue;
                }

                string[] tokens = dementiaType.dementiaType.Split(' ');
                string dementiaName = "";

                for (int j = 0; j < tokens.Length - 2; j++)
                {
                    /*if (tokens[j] == "late" || tokens[j] == "stage" || tokens[j] == "early" || tokens[j] == "middle")
                        break;*/

                    dementiaName += tokens[j];

                    if (j + 1 < tokens.Length - 2)
                        dementiaName += " ";
                }

                dementiaList.Add(new DementiaGameViewModel
                {
                    dementiaType = dementiaType,
                    dementiaName = dementiaName
                });

                dementiaCategoryList.Add(new DementiaGameCategoryViewModel
                {
                    dementiaType = dementiaType,
                    dementiaName = dementiaName
                });

                count++;
            }
            model.dementiaGame = dementiaList;
            model.dementiaGameCategory = dementiaCategoryList;

            foreach (var dementia in model.dementiaGame)
            {
                dementia.gameAssigned = new List<GameAssignedViewModel>();

                List<GameAssignedDementia> gameAssignedList = _context.GameAssignedDementias.Where(x => (x.dementiaID == dementia.dementiaType.dementiaID && x.isDeleted != 1)).ToList();
                foreach (var gameAssigned in gameAssignedList)
                {
                    string categoryString = "";
                    List<GameCategory> gameCategory = _context.GameCategories.Where(x => (x.gameID == gameAssigned.gameID && x.isDeleted != 1)).ToList();
                    foreach (var cat in gameCategory)
                    {
                        if (categoryString != "")
                            categoryString += ", ";
                        Category category = _context.Categories.SingleOrDefault(x => (x.categoryID == cat.categoryID && x.isApproved == 1 && x.isDeleted != 1));
                        categoryString += category.categoryName;
                    }

                    dementia.gameAssigned.Add(new GameAssignedViewModel
                    {
                        gameAssigned = gameAssigned,
                        gameCategory = categoryString,
                        gameTherapistName = gameAssigned.gameTherapistID == null ? null : _context.Users.FirstOrDefault(x => (x.userID == gameAssigned.gameTherapistID)).firstName + " " + _context.Users.FirstOrDefault(x => (x.userID == gameAssigned.gameTherapistID)).lastName,
                    });
                }
            }

            foreach (var dementia in model.dementiaGameCategory)
            {
                dementia.gameAssigned = new List<GameCategoryAssignedViewModel>();

                List<GameCategoryAssignedDementia> gameAssignedList = _context.GameCategoryAssignedDementia.Where(x => (x.dementiaID == dementia.dementiaType.dementiaID && x.isDeleted != 1)).ToList();
                foreach (var gameAssigned in gameAssignedList)
                {
                    dementia.gameAssigned.Add(new GameCategoryAssignedViewModel
                    {
                        gameCategoryAssignedDementia = gameAssigned,
                        category = _context.Categories.SingleOrDefault(x => (x.categoryID == gameAssigned.categoryID && x.isDeleted != 1)),
                        doctorName = gameAssigned.doctorID == null ? null : _context.Users.FirstOrDefault(x => (x.userID == gameAssigned.doctorID)).firstName + " " + _context.Users.FirstOrDefault(x => (x.userID == gameAssigned.doctorID)).lastName,
                        gameTherapistName = gameAssigned.gameTherapistID == null ? null : _context.Users.FirstOrDefault(x => (x.userID == gameAssigned.gameTherapistID)).firstName + " " + _context.Users.FirstOrDefault(x => (x.userID == gameAssigned.gameTherapistID)).lastName,
                    });
                }
            }

            /*
            for (int i = dementiaList.Count - 1; i >= 0; i--)
            {
                string[] tokens = dementiaList[i].dementiaType.dementiaType.Split(' ');
                int count = 0;
                int tempCount = 0;
                int add = 0;

                while (true)
                {
                    add++;

                    string[] nextTokens = dementiaList[--i].dementiaType.dementiaType.Split(' ');
                    count = 0;
                    int remove = 0;
                    for (int j=0; j<tokens.Length; j++)
                    {
                        if (tokens[j] == "late" || tokens[j] == "stage" || tokens[j] == "early" || tokens[j] == "middle")
                        {
                            remove++;
                            continue;
                        }

                        if (tokens[j] == nextTokens[j])
                            count++;
                    }

                    if (count == tokens.Length - remove)
                    {
                        for (int k = 0; k < dementiaList[i].gameAssigned.Count; k++)
                        {
                            dementiaList[i + add].gameAssigned.Add(dementiaList[i].gameAssigned[k]);
                        }
                        tempCount = count;
                    }
                    else
                        break;

                    if (i-1 < 0)
                        break;
                }

                string dementiaName = "";
                for (int l = 0; l < tempCount; l++)
                {
                    dementiaName += tokens[l];
                    if (l + 1 < tempCount)
                        dementiaName += " ";
                }

                dementiaList[i + add].dementiaName = dementiaName;
            }*/

            List<SelectListItem> gameSelectListItem = list.getGameList(1, false, false);
            ViewBag.game = new SelectList(gameSelectListItem, "Value", "Text");

            List<SelectListItem> gameCategorySelectListItem = list.getGameCategoryList(1, false);
            ViewBag.gameCategory = new SelectList(gameCategorySelectListItem, "Value", "Text");

            return View(model);
        }

        // POST: /Doctor/AddDementiaGameCategory
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isDoctor)]
        public ActionResult AddDementiaGameCategory(DoctorDementiaGameViewModel model)
        {
            TempData["gameForDementia"] = "inactive";
            TempData["gameCategoryForDementia"] = "active";

            int categoryID = Convert.ToInt32(Request.Form["gameCategory"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int dementiaID = model.dementiaID;

            GameCategoryAssignedDementia gameCategoryAssignedDementia = _context.GameCategoryAssignedDementia.FirstOrDefault(x => (x.categoryID == categoryID && x.dementiaID == dementiaID && x.isApproved != 0 && x.isDeleted != 1));
            string[] tokens = _context.DementiaTypes.SingleOrDefault(x => (x.dementiaID == dementiaID && x.isApproved == 1 && x.isDeleted != 1)).dementiaType.Split(' ');
            string dementiaName = "";

            for (int j = 0; j < tokens.Length - 2; j++)
            {
                dementiaName += tokens[j];

                if (j + 1 < tokens.Length - 2)
                    dementiaName += " ";
            }

            if (gameCategoryAssignedDementia == null)
            {
                patientMethod.addDementiaGameCategory(userID, dementiaID, categoryID, model.recommendationReason, 2);
                TempData["Message"] = "Game category recommendation requested! Waiting for game therapist approval.";
            }
            else
                TempData["Message"] = "Game category of " + gameCategoryAssignedDementia.Category.categoryName + " has already been approved/requested for " + dementiaName + ". Please select another game category!";

            TempData["Modal"] = "true";
            return RedirectToAction("ManageDementiaGame", "Doctor");
        }

        // GET: /Doctor/Find
        [HttpGet]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isDoctor)]
        public ActionResult Find()
        {
            ViewBag.result = "none";

            List<SelectListItem> searchSelectListItem = list.getDoctorSearchList();
            ViewBag.search = new SelectList(searchSelectListItem, "Value", "Text");

            return View();
        }

        // POST: /Doctor/FindResult
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isDoctor)]
        public ActionResult FindResult(FindViewModel model)
        {
            ViewBag.result = "block";

            string searchType = Request.Form["search"];
            List<SelectListItem> searchSelectListItem = list.getDoctorSearchList();
            ViewBag.search = new SelectList(searchSelectListItem, "Value", "Text", searchType);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            List<SearchResultViewModel> result = patientMethod.search("Doctor", searchType, model.searchWords);

            return View("Find", new FindViewModel { searchWords = model.searchWords, result = result });
        }

        [HttpPost]
        public ActionResult AcknowledgePlayedLastWeek(DoctorAnnouncementViewModel davm)
        {
            var acknowlege = _context.AssignedGames.Single(s => s.assignedGameID == davm.assignGameID);
            acknowlege.isApproved = 2;
            _context.SaveChanges();
            DateTime todayDate = DateTime.Now;
            var insertIntoLogTable = new Log()
            {
                oldLogData = null,
                logDesc = "AssignGameID:" + davm.assignGameID + ";Action:Acknowledge List Of Patient Played Last Week;",
                tableAffected = "AssginGame Table",
                columnAffected = "isApproved",
                rowAffected = davm.assignGameID,
                patientAllocationID = davm.patientID,
                userIDInit = davm.userID, //For Now
                userIDApproved = 0, //For Now
                additionalInfo = null,
                remarks = "DoctorID: " + davm.userID + " has viewed this message under List Of Patient Played Last Week",
                logCategoryID = 4,
                createDateTime = todayDate,
                isDeleted = 0,
                approved = 1, //For Now
                reject = 0,
                supNotified = 0,
                userNotified = 1,
                rejectReason = null

            };
            _context.Logs.Add(insertIntoLogTable);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult AcknowledgeGameTypeRecommendApproval(DoctorAnnouncementViewModel davm)
        {
            var acknowlege = _context.GamesTypeRecommendations.Single(s => s.gamesTypeRecommendationID == davm.gamesTypeRecommendationID);
            if (davm.assignGameID == 1)
            {
                acknowlege.isApproved = 5;
            }
            else if (davm.assignGameID == -1)
            {
                acknowlege.isApproved = 6;
            }
            else if (davm.assignGameID == 4)
            {
                acknowlege.isApproved = 7;
            }

            _context.SaveChanges();
            DateTime todayDate = DateTime.Now;
            var insertIntoLogTable = new Log()
            {
                oldLogData = null,
                logDesc = "gamesTypeRecommendationID:" + davm.gamesTypeRecommendationID + ";Action:Acknowledge Game Type Recommend Approval;",
                tableAffected = "GamesTypeRecommendation Table",
                columnAffected = "isApproved",
                rowAffected = davm.gamesTypeRecommendationID,
                patientAllocationID = davm.patientID,
                userIDInit = davm.userID, //For Now
                userIDApproved = 0, //For Now
                additionalInfo = null,
                remarks = "DoctorID: " + davm.userID + " has viewed this message under Acknowledge Game Type Recommend Approval",
                logCategoryID = 4,
                createDateTime = todayDate,
                isDeleted = 0,
                approved = 1, //For Now
                reject = 0,
                supNotified = 0,
                userNotified = 1,
                rejectReason = null

            };
            _context.Logs.Add(insertIntoLogTable);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = RoleName.isDoctor)]
        public ActionResult Help()
        {
            //Only allow doctor to view this page
            if (User.IsInRole(RoleName.isDoctor))
            {
                return View("Help");
            }
            else
            {
                return View("_LoginPage");
            }
        }
        
    }
}

/*

                DateTime today = DateTime.Parse(DateTime.Today.AddDays(-7).ToString());
                if (User.IsInRole(RoleName.isDoctor))
                {
                    var id = Convert.ToInt32(User.Identity.GetUserID2());
                    var patients = _context.GameRecords
                        .Include(a => a.AssignedGame)
                        .Include(a => a.AssignedGame.Game)
                        .Include(a => a.AssignedGame.PatientAllocation)
                        .Include(a => a.AssignedGame.PatientAllocation.Patient)
                        .Where(a => a.createDateTime > today)
                        .Where(a => a.isDeleted == 0)
                        .Where(a => a.AssignedGame.isDeleted == 0)
                        .Where(a => a.AssignedGame.isApproved == 1)
                        .Where(a => a.AssignedGame.PatientAllocation.doctorID == id)
                        .Where(a => a.AssignedGame.PatientAllocation.isDeleted == 0)
                        .Where(a => a.AssignedGame.PatientAllocation.isApproved == 1);


                    var patients2 = _context.GameRecords
                        .Include(a => a.AssignedGame)
                        .Include(a => a.AssignedGame.Game)
                        .Include(a => a.AssignedGame.PatientAllocation)
                        .Include(a => a.AssignedGame.PatientAllocation.Patient)
                        .Where(a => a.isDeleted == 0)
                        .Where(a => a.AssignedGame.isDeleted == 0)
                        .Where(a => a.AssignedGame.isApproved == 2)
                        .Where(a => a.AssignedGame.PatientAllocation.doctorID == id)
                        .Where(a => a.AssignedGame.PatientAllocation.isDeleted == 0)
                        .Where(a => a.AssignedGame.PatientAllocation.isApproved == 1);


                    var albums = _context.AlbumPatient.ToList();
                    var users = _context.Users.ToList();

                    var gameTypeRecommendationApproved = _context.GamesTypeRecommendations
                        .Include(b => b.PatientAllocation)
                        .Include(b => b.PatientAllocation.Patient)
                        .Include(b => b.PatientAllocation.gametherapistID)
                        .Where(a => a.PatientAllocation.doctorID == id)
                        .Where(b => b.isApproved == 4 || b.isApproved == -1 || b.isApproved == 1)
                        .Where(b => b.isDeleted == 0);


                    var gameTypeRecommendationApproved2 = _context.GamesTypeRecommendations
                        .Include(b => b.PatientAllocation)
                        .Include(b => b.PatientAllocation.Patient)
                        .Include(b => b.PatientAllocation.gametherapistID)
                        .Where(b => b.isApproved == 5 || b.isApproved == 6 || b.isApproved == 7)
                        .Where(b => b.isDeleted == 0);

                    var viewModel = new DoctorAnnouncementViewModel()
                    {
                        Patients = patients,
                        Patients2 = patients2,
                        Albums = albums,
                        Users = users,
                        GamesTypeRecommendations = gameTypeRecommendationApproved,
                        GamesTypeRecommendations2 = gameTypeRecommendationApproved2,
                        userID = Convert.ToInt32(User.Identity.GetUserID2())
                    };
                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
    */
