using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;

namespace NTU_FYP_REBUILD_17.Controllers
{
	public class ManagePatientController : Controller
    {
        private ApplicationDbContext _context;

        public ManagePatientController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: ManagePatient
        [Authorize(Roles = RoleName.isDoctor)]
        public ActionResult Index()
        {
            //Only allow doctor to view this page
            if (User.IsInRole(RoleName.isDoctor))
            {
                var users = _context.Users.ToList();
                //var patients = _context.Patients.ToList();
                //var patientAllocations = _context.PatientAllocations.ToList();
                var patients = _context.Patients.Where(x => x.isApproved == 1 && x.isDeleted == 0).ToList();
                var patientAllocations = _context.PatientAllocations.Where(x => x.isApproved == 1 && x.isDeleted == 0).ToList();
                var albums = _context.AlbumPatient.ToList();
                var gameRecommend = _context.GamesTypeRecommendations.ToList();
                var allergies = _context.Allergies.ToList();
                var doctorNotes = _context.DoctorNotes.ToList();

                var viewModel = new ManagePatientsViewModel()
                {
                    Users = users,
                    Patients = patients,
                    PatientAllocations = patientAllocations,
                    Albums = albums,
                    GamesTypeRecommendations = gameRecommend,
                    Allergies = allergies,
                    DoctorNotes = doctorNotes
                };

                return View(viewModel);
            }
            else
            {
                return View("_LoginPage");
            }
        }

		[AllowAnonymous]
		[HttpGet]
		public PartialViewResult Notification()
		{
			System.Diagnostics.Debug.WriteLine("Notification GEt");
			notificationFB notiObj = new notificationFB();
			notiObj.notificationString = "GET";
			return PartialView("~/Views/Home/Notification.cshtml", notiObj);
		}

		[AllowAnonymous]
		[HttpPost]
		public PartialViewResult Notification(string msg)
		{
			System.Diagnostics.Debug.WriteLine("Notification GEt");
			notificationFB notiObj = new notificationFB();
			notiObj.notificationString = "GET";
			return PartialView("~/Views/Home/Notification.cshtml", notiObj);
		}

		// GET: ManagePatient/EditPatient
		public ActionResult EditPatient(int id)
        {
            var patient = _context.Patients.SingleOrDefault(p => p.patientID == id);
            var patientAllocation2 = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patient.patientID && x.isApproved == 1 && x.isDeleted != 1));
            var album = _context.AlbumPatient.SingleOrDefault(a => a.patientAllocationID == patientAllocation2.patientAllocationID);
            var patientAllocation = _context.PatientAllocations.ToList();
            var gameTypeRecommended = _context.GamesTypeRecommendations.ToList();
            var Allergies = _context.Allergies.ToList().Where(a => a.patientAllocationID == id);
            var doctorNotes = _context.DoctorNotes.ToList().Where(dn => dn.patientAllocationID == id);
            var category = _context.Categories.ToList();

            var viewModel = new EditPatientViewModel()
            {
                Patient = patient,
                Album = album,
                PatientAllocations = patientAllocation,
                GamesTypeRecommendations = gameTypeRecommended,
                Allergies = Allergies,
                DoctorNotes = doctorNotes,
                Categories = category
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateDoctorNote(DoctorNote dn, EditPatientViewModel epvm)
        {
            dn.createDateTime = DateTime.Now;
            _context.DoctorNotes.Add(dn);
            _context.SaveChanges();

            var tableAffected = _context.DoctorNotes.SingleOrDefault(p => p.patientAllocationID == dn.patientAllocationID && p.note.Equals(dn.note));

            var insertIntoLogTable = new Log()
            {
                oldLogData = null,
                logDesc = "PatientID:" + dn.patientAllocationID + ";Action:ADD DOCOTOR NOTE;Notes:" + dn.note + ";",
                tableAffected = "DoctorNote Table",
                columnAffected = "ALL",
                rowAffected = tableAffected.doctorNoteID,
                patientAllocationID = dn.patientAllocationID,
                userIDInit = epvm.userID,   //For Now
                userIDApproved = epvm.userID,   //For Now
                additionalInfo = null,
                remarks = dn.note,
                logCategoryID = 2,
                createDateTime = dn.createDateTime,
                isDeleted = dn.isDeleted,
                approved = 1,               //For Now
                reject = 0,
                supNotified = 0,
                userNotified = 0,
                rejectReason = null

            };

            _context.Logs.Add(insertIntoLogTable);
            _context.SaveChanges();

            return RedirectToAction("EditPatient", "ManagePatient", new { id = dn.patientAllocationID });
        }

        [HttpPost]
        public ActionResult CreateRecommendedGame(EditPatientViewModel e)
        {
            e.createDateTime = DateTime.Today;
            for (var i = 0; i < e.reason.Count(); i++)
            {
                if (!(e.reason.ToList()[i].Equals("")))
                {
                    var insertGameRecommended = new GamesTypeRecommendation()
                    {
                        rejectionReason = e.reason.ToList()[i].ToString(),
                        patientAllocationID = e.patientAllocationID,
                        isDeleted = e.isDeleted,
                        createDateTime = e.createDateTime,
                    };
                    _context.GamesTypeRecommendations.Add(insertGameRecommended);
                    _context.SaveChanges();

                    var catID = e.categoryID.ToList()[i];
                    var reason = e.reason.ToList()[i].ToString();
                    var tableAffected = _context.GamesTypeRecommendations.SingleOrDefault(p => p.createDateTime == e.createDateTime
                    && p.rejectionReason.Equals(reason)
                    && p.patientAllocationID == e.patientAllocationID);

                    var insertIntoLogTable = new Log()
                    {
                        oldLogData = null,
                        logDesc = "PatientAllocationID:" + e.patientAllocationID + ";Action:ADD Recommened Game(CategoryID):" + e.categoryID.ToList()[i] + ";Reason:" + e.reason.ToList()[i].ToString() + ";",
                        tableAffected = "GamesTypeRecommendation Table",
                        columnAffected = "ALL",
                        rowAffected = tableAffected.gamesTypeRecommendationID,
                        patientAllocationID = e.patientID,
                        userIDInit = e.userID,   //For Now
                        userIDApproved = 0,   //For Now
                        additionalInfo = null,
                        remarks = e.reason.ToList()[i].ToString(),
                        logCategoryID = 2,
                        createDateTime = e.createDateTime,
                        isDeleted = 0,
                        approved = 1,               //For Now
                        reject = 0,
                        supNotified = 0,
                        userNotified = 0,
                        rejectReason = null

                    };
                    _context.Logs.Add(insertIntoLogTable);
                    _context.SaveChanges();
                }

            }

            return RedirectToAction("EditPatient", "ManagePatient", new { id = e.patientID });
        }

        public ActionResult ViewReport(int id, int catID, int startDate, int endDate)
        {
            ViewBag.isData = null;
            ViewBag.isData2 = null;

            ViewBag.patientAllocationID = id;
            ViewBag.CatID = catID;

            List<ViewReportPoints> dataPoints = new List<ViewReportPoints>();
            var abc = _context.GameCategories
                .Include(a => a.Game)
                .Where(a => a.categoryID == catID)
                .Where(a => a.isApproved == 1)
                .Where(a => a.isDeleted == 0)
                .ToList();

            var avgDailyScore = _context.GameRecords.Include(a => a.AssignedGame)
                .Include(a => a.AssignedGame.Game)
                .Where(a => a.AssignedGame.patientAllocationID == id)
                .Where(a => a.AssignedGame.isApproved == 1)
                .Where(a => a.AssignedGame.isDeleted == 0)
                .ToList();


            double? total = 0;
            int count = 0;
            ViewBag.isData = false;
            foreach (var i in avgDailyScore)
            {
                foreach (var ii in abc)
                {
                    if (i.AssignedGame.gameID == ii.gameID && Convert.ToDateTime(ii.createDateTime.ToString()).Year == DateTime.Now.Year && Convert.ToDateTime(ii.createDateTime.ToString()).Month == DateTime.Now.Month && Convert.ToDateTime(i.createDateTime.ToString()).Year == DateTime.Now.Year && Convert.ToDateTime(i.createDateTime.ToString()).Month == DateTime.Now.Month)
                    {
                        var month = Convert.ToDateTime(i.createDateTime.ToString()).Month - 1;
                        dataPoints.Add(new ViewReportPoints(Convert.ToDateTime(i.createDateTime.ToString()).Year.ToString(), month.ToString(), Convert.ToDateTime(i.createDateTime.ToString()).Day.ToString(), i.score));
                        total += i.score;
                        count++;
                        ViewBag.isData = true;
                        break;
                    }
                }
            }

            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            ViewBag.CatName = _context.Categories.SingleOrDefault(a => a.categoryID == catID).categoryName;
            if (count > 0 && total != 0)
            {
                ViewBag.MonthlyAvgScore = Math.Round((double)total / count, 2);
            }
            else
            {
                ViewBag.MonthlyAvgScore = "nil";
            }
            return View();
        }

        [HttpPost]
        public ActionResult ViewReport2(ViewReportViewModel e)
        {
            ViewBag.patientAllocationID = e.patientAllocationID;
            ViewBag.CatID = e.catID;

            if (e.hasData)
            {
                List<ViewReportPoints> dataPoints = new List<ViewReportPoints>();
                var abc = _context.GameCategories
                    .Include(a => a.Game)
                    .Where(a => a.categoryID == e.catID)
                    .Where(a => a.isApproved == 1)
                    .Where(a => a.isDeleted == 0)
                    .ToList();

                var avgDailyScore = _context.GameRecords.Include(a => a.AssignedGame)
                    .Include(a => a.AssignedGame.Game)
                    .Where(a => a.AssignedGame.patientAllocationID == e.patientAllocationID)
                    .Where(a => a.AssignedGame.isApproved == 1)
                    .Where(a => a.AssignedGame.isDeleted == 0)
                    .ToList();


                double? total = 0;
                int count = 0;
                ViewBag.isData = false;
                foreach (var i in avgDailyScore)
                {
                    foreach (var ii in abc)
                    {
                        if (i.AssignedGame.gameID == ii.gameID && Convert.ToDateTime(ii.createDateTime.ToString()).Year == DateTime.Now.Year && Convert.ToDateTime(ii.createDateTime.ToString()).Month == DateTime.Now.Month && Convert.ToDateTime(i.createDateTime.ToString()).Year == DateTime.Now.Year && Convert.ToDateTime(i.createDateTime.ToString()).Month == DateTime.Now.Month)
                        {
                            var month = Convert.ToDateTime(i.createDateTime.ToString()).Month - 1;
                            dataPoints.Add(new ViewReportPoints(Convert.ToDateTime(i.createDateTime.ToString()).Year.ToString(), month.ToString(), Convert.ToDateTime(i.createDateTime.ToString()).Day.ToString(), i.score));
                            total += i.score;
                            count++;
                            ViewBag.isData = true;
                            break;
                        }
                    }
                }

                ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
                ViewBag.CatName = _context.Categories.SingleOrDefault(a => a.categoryID == e.catID).categoryName;
                if (count > 0 && total != 0)
                {
                    ViewBag.MonthlyAvgScore = Math.Round((double)total / count, 2);
                }
                else
                {
                    ViewBag.MonthlyAvgScore = "nil";

                }
            }




            List<ViewReportPoints> dataPoints2 = new List<ViewReportPoints>();
            var abc2 = _context.GameCategories
                .Include(a => a.Game)
                .Where(a => a.categoryID == e.catID)
                .Where(a => a.isApproved == 1)
                .Where(a => a.isDeleted == 0)
                .ToList();

            var avgDailyScore2 = _context.GameRecords.Include(a => a.AssignedGame)
                .Include(a => a.AssignedGame.Game)  //Game ID
                .Where(a => a.AssignedGame.patientAllocationID == e.patientAllocationID)
                .Where(a => a.AssignedGame.isApproved == 1)
                .Where(a => a.AssignedGame.isDeleted == 0)
                .ToList().OrderBy(a => a.createDateTime);

            var avgDailyScore245 = _context.GameRecords.Include(a => a.AssignedGame)
                .Include(a => a.AssignedGame.Game)  //Game ID
                .Where(a => a.AssignedGame.patientAllocationID == e.patientAllocationID)
                .Where(a => a.AssignedGame.isApproved == 1)
                .Where(a => a.AssignedGame.isDeleted == 0)
                .ToList().OrderBy(a => a.createDateTime).ToList();


            //Below this part will be for the past record codes.
            if (e.startDate != "" && e.startDate != "0" && e.startDate != null && e.endDate != "" && e.endDate != "0" && e.endDate != null)
            {
                dataPoints2 = new List<ViewReportPoints>();
                //HERE
                double? total2 = 0;
                int count2 = 0;
                foreach (var i in avgDailyScore2)
                {
                    foreach (var ii in abc2)
                    {
                        DateTime start = DateTime.ParseExact(e.startDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        DateTime end = DateTime.ParseExact(e.endDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        int result = DateTime.Compare(ii.createDateTime, start);
                        int result2 = DateTime.Compare(end, ii.createDateTime);

                        int result3 = DateTime.Compare(i.createDateTime, start);
                        int result4 = DateTime.Compare(end, i.createDateTime);


                        if (i.AssignedGame.gameID == ii.gameID
                            && result == 1 && result2 == 1 && result3 == 1 && result4 == 1
                            && ii.categoryID == e.catID)
                        {
                            var month = Convert.ToDateTime(i.createDateTime.ToString()).Month - 1;
                            dataPoints2.Add(new ViewReportPoints(Convert.ToDateTime(i.createDateTime.ToString()).Year.ToString(), month.ToString(), Convert.ToDateTime(i.createDateTime.ToString()).Day.ToString(), i.score));
                            total2 += i.score;
                            count2++;
                            ViewBag.isData2 = true;
                            break;
                        }
                    }
                }

                ViewBag.DataPoints2 = JsonConvert.SerializeObject(dataPoints2);
                ViewBag.CatName = _context.Categories.SingleOrDefault(a => a.categoryID == e.catID).categoryName;
                ViewBag.StartDate = e.startDate;
                ViewBag.EndDate = e.endDate;
                if (count2 > 0 && total2 != 0)
                {
                    //                    ViewBag.MonthlyAvgScore2 = total2 / count2;
                    ViewBag.MonthlyAvgScore2 = Math.Round((double)total2 / count2, 2);
                }
                else
                {
                    ViewBag.MonthlyAvgScore2 = "nil";
                }
            }
            return View("ViewReport");
        }


        // GET: AdvanceSearch
        [Authorize(Roles = RoleName.isDoctor)]
        public ActionResult AdvanceSearch()
        {
            //Only allow doctor to view this page
            if (User.IsInRole(RoleName.isDoctor))
            {
                var users = _context.Users.ToList();
                var patients = _context.Patients.ToList();
                var patientAllocations = _context.PatientAllocations.ToList();
                var albums = _context.AlbumPatient.ToList();
                var gameRecommend = _context.GamesTypeRecommendations.ToList();
                var allergies = _context.Allergies.ToList();
                var doctorNotes = _context.DoctorNotes.ToList();

                var viewModel = new ManagePatientsViewModel()
                {
                    Users = users,
                    Patients = patients,
                    PatientAllocations = patientAllocations,
                    Albums = albums,
                    GamesTypeRecommendations = gameRecommend,
                    Allergies = allergies,
                    DoctorNotes = doctorNotes
                };

                return View(viewModel);
            }
            else
            {
                return View("_LoginPage");
            }
        }

        public ActionResult CentreActivity()
        {
            return View();
        }

        /// Get Create Activity for selected patients
        public ActionResult GetCreateActivityResult(List<int> Patients)
        {
            DateTime Date = DateTime.Now;
            CenterActivityViewModel viewModel = new CenterActivityViewModel();
            // Get selected patients
            viewModel.Patients = _context.Patients.Where(x => Patients.Contains(x.patientID)).ToList();
            // Get activities
            viewModel.CentreActivities = _context.CentreActivities.OrderBy(x => x.centreActivityID).ToList();
            // Get existing activity preferences
            viewModel.ActivityPreferences = (from p in _context.Patients
                                             join ap in _context.ActivityPreferences on p.patientID equals ap.patientAllocationID
                                             join allo in _context.PatientAllocations on p.patientID equals allo.patientID
                                             where ap.isApproved == 1 && ap.isDeleted == 0
                                             select new ActivityPreferenceViewModel
                                             {
                                                 PatientID = ap.patientAllocationID,
                                                 CentreActivityId = ap.centreActivityID,
                                                 IsLike = ap.isLike,
                                                 IsDislike = ap.isDislike,
                                                 CentreactivityExclusionID = null,//(from ae in _context.ActivityExclusions where ae.patientID == ap.patientID && ae.centreActivityID == ap.CentreActivityId && ae.isDeleted == 0 && ae.isApproved == 1 && ae.dateTimeStart < Date && ae.dateTimeEnd > Date select ae.centreActivityID).FirstOrDefault()
                                                 IsActivity = true,
                                                 DoctorRecomendation = ap.doctorRecommendation
                                             }).ToList();

            // Get activity exclusions
            var activityExclusions = _context.ActivityExclusions.Where(x => x.isDeleted == 0 && x.isApproved == 1 && x.dateTimeStart < Date && x.dateTimeEnd > Date && x.centreActivityID != null).ToList();
            foreach (var item in activityExclusions)
            {
                // Check activity preference exist or not
                var activityPreference = viewModel.ActivityPreferences.FirstOrDefault(x => x.PatientID == item.patientAllocationID && x.CentreActivityId == item.centreActivityID);
                if (activityPreference != null)
                {
                    activityPreference.CentreactivityExclusionID = item.centreActivityID;
                }
                else
                {
                    // Create activity preference for non existing activity preference of activity exclusion
                    viewModel.ActivityPreferences.Add(new ActivityPreferenceViewModel
                    {
                        PatientID = item.patientAllocationID,
                        CentreActivityId = (int)item.centreActivityID,
                        IsLike = 0,
                        IsDislike = 0,
                        CentreactivityExclusionID = item.centreActivityID,
                        IsActivity = false,
                        DoctorRecomendation = 0
                    });
                }
            }

            return Json(viewModel);
        }

        // Save tick and cross event
        public ActionResult SetActivityPreference(int patientId, int centreActivityID, int status, string remarks)
        {
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
                activityPreference.doctorRecommendation = status;
                //Add new record
                if (isNew)
                    _context.ActivityPreferences.Add(activityPreference);
                _context.SaveChanges();
                //Update remark and log for unrecommend
                if (isNew && status == 2 || !isNew && status == 2)
                {
                    activityPreference.doctorRemarks = remarks;
                    InsertLog(patientId, centreActivityID, status, remarks, isNew);
                }
                //Update remark and log for recommend
                if (isNew && status == 1 || !isNew && status == 1)
                {
                    activityPreference.doctorRemarks = remarks;
                    InsertLog(patientId, centreActivityID, status, remarks, isNew);
                }
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        // Insert log for tick and cross update and tick insert
        public bool InsertLog(int patientId, int centreActivityID, int status, string remarks, bool IsNew)
        {
            // Generate logDate for insert log
            string userID = User.Identity.GetUserTypeID();
            string logData = "";
            if (IsNew && status == 1)
            {
                logData = $"PatientID = {patientId};centreActivityID = {centreActivityID}; isLike = 0; isDislike=0; isNeutral=0; isApproved=1; doctorRecommendation=1; isDeleted=0; doctorRemark = {remarks};";
            }
            else if (status == 1)
            {
                logData = $"PatientID= {patientId}; centreActivityID= {centreActivityID}; doctorRecommendation=1; doctorRemark = {remarks};";
            }
            if (IsNew && status == 2)
            {
                logData = $"PatientID = {patientId};centreActivityID = {centreActivityID}; isLike = 0; isDislike=0; isNeutral=0; isApproved=1; doctorRecommendation=2; isDeleted=0; doctorRemark = {remarks};";
            }
            else if (status == 2)
            {
                logData = $"PatientID= {patientId}; centreActivityID= {centreActivityID}; doctorRecommendation=2; doctorRemark = {remarks};";
            }
            string logDesc = $"Update doctorRecommendation – centreActivityID {centreActivityID}";
            Log log = new Log
            {
                logData = logData,
                logDesc = logDesc,
                logCategoryID = 2,
                patientAllocationID = patientId,
                userIDInit = int.Parse(userID),
                userIDApproved = int.Parse(userID),
                tableAffected = "activityPreferences",
                columnAffected = "doctorRecommendation",
                rowAffected = patientId,
                isDeleted = 0,
                createDateTime = DateTime.Now,
                supNotified = 0,
                approved = 1,
                reject = 0,
                userNotified = 0
            };
            _context.Logs.Add(log);
            _context.SaveChanges();
            return true;
        }
    }
}