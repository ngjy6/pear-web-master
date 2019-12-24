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
using OfficeOpenXml;
using System.Web.Routing;

namespace NTU_FYP_REBUILD_17.Controllers
{
	public class GameTherapistController : Controller
    {
        private ApplicationDbContext _context;
        private App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        Controllers.Synchronization.PatientMethod patientMethod = new Controllers.Synchronization.PatientMethod();
        Controllers.Synchronization.PrivacyMethod privacyMethod = new Controllers.Synchronization.PrivacyMethod();
        Controllers.Synchronization.ListMethod list = new Controllers.Synchronization.ListMethod();
        Controllers.Synchronization.GameMethod gameMethod = new Controllers.Synchronization.GameMethod();
        Controllers.Synchronization.ScheduleMethod scheduler = new Controllers.Synchronization.ScheduleMethod();

        public GameTherapistController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
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

        // GET: /GameTherapist/Index
        [HttpGet]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult Index()
        {
            GameTherapistIndexViewModel model = new GameTherapistIndexViewModel();
            model.patient = new List<PatientViewModel>();
            List<int> patientListID = new List<int>();

            DateTime date = DateTime.Today;

            int userID = Convert.ToInt32(User.Identity.GetUserID2());

            var activePatientList = (from pa in _context.PatientAllocations
                                     join p in _context.Patients on pa.patientID equals p.patientID
                                     join u in _context.Users on pa.gametherapistID equals u.userID
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
                                     join u in _context.Users on pa.gametherapistID equals u.userID
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
                                    join u in _context.Users on pa.gametherapistID equals u.userID
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

        // GET: /GameTherapist/ViewPatient
        [HttpGet]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult ViewPatient(int patientID)
        {
            ViewBag.Modal = TempData["Modal"];

            ViewBag.Info = TempData["Info"];
            if (ViewBag.Info != "inactive")
                ViewBag.Info = "active";

            ViewBag.History = TempData["History"];
            if (ViewBag.History != "active")
                ViewBag.History = "inactive";

            ViewBag.GameList = TempData["GameList"];
            if (ViewBag.GameList != "active")
                ViewBag.GameList = "inactive";

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

            var diagnosedDementia = (from pad in _context.PatientAssignedDementias
                                     join d in _context.DementiaTypes on pad.dementiaID equals d.dementiaID
                                     where pad.patientAllocationID == patientAllocationID && pad.isApproved == 1 && pad.isDeleted != 1
                                     where d.isApproved == 1 && d.isDeleted != 1
                                     select new DementiaViewModel
                                     {
                                         pad = pad,
                                         dementia = d
                                     }).OrderBy(x => x.dementia.dementiaType).ToList();

            var mobility = (from m in _context.Mobility
                            join ml in _context.ListMobility on m.mobilityListID equals ml.list_mobilityID
                            where m.patientAllocationID == patientAllocationID && m.isApproved == 1 && m.isDeleted != 1
                            where ml.isDeleted != 1
                            select new MobilityViewModel
                            {
                                mobilityType = ml.value,
                                mobility = m
                            }).ToList();

            var game = (from ag in _context.AssignedGames
                        join g in _context.Games on ag.gameID equals g.gameID
                        where g.isApproved == 1 && g.isDeleted != 1
                        where ag.patientAllocationID == patientAllocationID && ag.isDeleted != 1
                        select new PatientGameViewModel
                        {
                            game = g,
                            assignedGame = ag,
                            gameTherapistName = ag.GameTherapist.AspNetUsers.firstName + " " + ag.GameTherapist.AspNetUsers.lastName,
                        }).OrderByDescending(x => x.assignedGame.createDateTime).ToList();

            foreach (var g in game)
                g.gameCategory = patientMethod.getGameCategoryList(g.game.gameName);

            List<SelectListItem> gameSelectListItem = list.getGameList(1, false, false);

            List<string> diagnosedDementiaType = new List<string>();
            foreach (var dementia in diagnosedDementia)
                diagnosedDementiaType.Add(patientMethod.getDementiaName(dementia.dementia.dementiaType));

            List<string> gameList = new List<string>();
            foreach (var gameItem in gameSelectListItem)
                gameList.Add(gameItem.Text);

            List<GameAssignedDementiaViewModel> gameAssignedToDementia = new List<GameAssignedDementiaViewModel>();
            for (int i=0; i< gameList.Count; i++)
            {
                GameAssignedDementiaViewModel gameAssignedViewModel = new GameAssignedDementiaViewModel();
                string gameName = gameList[i];
                gameAssignedViewModel.game = _context.Games.SingleOrDefault(x => (x.gameName == gameName && x.isApproved == 1 && x.isDeleted != 1));
                gameAssignedViewModel.categoryList = patientMethod.getGameCategoryList(gameName);
                gameAssignedViewModel.bitString = new List<string>();

                for (int j = 0; j < diagnosedDementiaType.Count; j++)
                {
                    string dementiaName = diagnosedDementiaType[j];
                    List<GameAssignedDementia> gad = _context.GameAssignedDementias.Where(x => (x.Game.gameName == gameName && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                
                    bool found = false;
                    foreach (var gameAss in gad)
                    {
                        string selectedDementiaName = patientMethod.getDementiaName(gameAss.DementiaType.dementiaType);
                        if (diagnosedDementiaType[j] == selectedDementiaName)
                        {
                            gameAssignedViewModel.bitString.Add("Yes");
                            gameAssignedViewModel.showGame = true;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        gameAssignedViewModel.bitString.Add("No");
                }
                gameAssignedToDementia.Add(gameAssignedViewModel);
            }

            for (int i=0; i<gameAssignedToDementia.Count; i++)
            {
                for (int j = 0; j < game.Count; j++)
                {
                    if ((game[j].assignedGame.isApproved != 0 && game[j].assignedGame.isDeleted != 1) && game[j].game.gameName == gameAssignedToDementia[i].game.gameName)
                    {
                        gameAssignedToDementia[i].gameAlreadyAssigned = true;
                        break;
                    }
                }
            }

            List<SelectListItem> gameCategorySelectListItem = list.getGameCategoryList(1, false);
            List<string> gameCategoryList = new List<string>();
            foreach (var gameCategoryItem in gameCategorySelectListItem)
                gameCategoryList.Add(gameCategoryItem.Text);

            List<GameCategoryAssignedDementiaViewModel> gameCategoryAssignedToDementia = new List<GameCategoryAssignedDementiaViewModel>();
            for (int i = 0; i < diagnosedDementiaType.Count; i++)
            {
                string dementiaName = diagnosedDementiaType[i];
                GameCategoryAssignedDementiaViewModel gameCategoryAssignedDementiaViewModel = new GameCategoryAssignedDementiaViewModel();
                gameCategoryAssignedDementiaViewModel.dementia = dementiaName;
                gameCategoryAssignedDementiaViewModel.recommendedCategory = new List<string>();

                for (int j = 0; j < gameCategoryList.Count; j++)
                {
                    string gameGategoryName = gameCategoryList[j];
                    List<GameCategoryAssignedDementia> gcad = _context.GameCategoryAssignedDementia.Where(x => (x.Category.categoryName == gameGategoryName && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                    bool added = false;
                    foreach (var gameCatAss in gcad)
                    {
                        string selectedDementiaName = patientMethod.getDementiaName(gameCatAss.DementiaType.dementiaType);
                        if (diagnosedDementiaType[i] == selectedDementiaName)
                        {
                            gameCategoryAssignedDementiaViewModel.recommendedCategory.Add("Yes");
                            added = true;
                            break;
                        }
                    }

                    if (!added)
                        gameCategoryAssignedDementiaViewModel.recommendedCategory.Add("No");
                }
                gameCategoryAssignedToDementia.Add(gameCategoryAssignedDementiaViewModel);
            }

            for (int i = 0; i < gameCategoryAssignedToDementia.Count; i++)
            {
                List<EachDementiaViewModel> viewModel = new List<EachDementiaViewModel>();
                for (int k = 0; k < gameList.Count; k++)
                {
                    EachDementiaViewModel eachDementiaViewModel = new EachDementiaViewModel();

                    string gameName = gameList[k];
                    eachDementiaViewModel.game = _context.Games.SingleOrDefault(x => (x.gameName == gameName && x.isApproved == 1 && x.isDeleted != 1));
                    eachDementiaViewModel.categoryList = patientMethod.getGameCategoryList(gameName);
                    eachDementiaViewModel.categoryBitString = new List<string>();

                    for (int l = 0; l < gameCategoryList.Count; l++)
                    {
                        string gameCategoryName = gameCategoryList[l];
                        GameCategory gameCategory = _context.GameCategories.SingleOrDefault(x => (x.Category.categoryName == gameCategoryName && x.Game.gameName == gameName && x.isApproved == 1 && x.isDeleted != 1));
                        if (gameCategory != null)
                            eachDementiaViewModel.categoryBitString.Add("Yes");
                        else
                            eachDementiaViewModel.categoryBitString.Add("No");

                        AssignedGame assigned = _context.AssignedGames.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.Game.gameName == gameName && x.isApproved != 0 && x.isDeleted != 1));
                        if (assigned != null)
                            eachDementiaViewModel.gameAlreadyAssigned = true;
                        else
                            eachDementiaViewModel.gameAlreadyAssigned = false;
                    }

                    for (int l = 0; l < eachDementiaViewModel.categoryBitString.Count; l++)
                    {
                        for (int m = 0; m < gameCategoryAssignedToDementia[i].recommendedCategory.Count; m++)
                        {
                            if (gameCategoryAssignedToDementia[i].recommendedCategory[m] == "Yes" && eachDementiaViewModel.categoryBitString[m] == "Yes")
                            {
                                eachDementiaViewModel.showGame = true;
                                break;
                            }
                        }
                        break;
                    }

                    viewModel.Add(eachDementiaViewModel);
                }
                gameCategoryAssignedToDementia[i].viewModel = viewModel;
            }

            List<string> gameCategoryRecommended = _context.GamesTypeRecommendations.Where(x => (x.patientAllocationID == patientAllocationID && (x.endDate == null || DateTime.Compare((DateTime)x.endDate, DateTime.Now) > 0) && x.isApproved == 1 && x.isDeleted != 1)).OrderBy(x => x.Category.categoryName).Select(x => x.Category.categoryName).ToList();

            GameAssignedPatientViewModel gameAssignedPatientViewModel = new GameAssignedPatientViewModel();
            gameAssignedPatientViewModel.recommendedCategory = new List<string>();
            for (int i=0; i< gameCategoryList.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < gameCategoryRecommended.Count; j++)
                {
                    if (gameCategoryList[i] == gameCategoryRecommended[j])
                    {
                        gameAssignedPatientViewModel.recommendedCategory.Add("Yes");
                        found = true;
                    }
                }
                if (!found)
                    gameAssignedPatientViewModel.recommendedCategory.Add("No");
            }

            gameAssignedPatientViewModel.viewModel = new List<EachDementiaViewModel>();
            for (int k = 0; k < gameList.Count; k++)
            {
                EachDementiaViewModel eachDementiaViewModel = new EachDementiaViewModel();

                string gameName = gameList[k];
                eachDementiaViewModel.game = _context.Games.SingleOrDefault(x => (x.gameName == gameName && x.isApproved == 1 && x.isDeleted != 1));
                eachDementiaViewModel.categoryList = patientMethod.getGameCategoryList(gameName);
                eachDementiaViewModel.categoryBitString = new List<string>();

                for (int l = 0; l < gameCategoryList.Count; l++)
                {
                    string gameCategoryName = gameCategoryList[l];
                    GameCategory gameCategory = _context.GameCategories.SingleOrDefault(x => (x.Category.categoryName == gameCategoryName && x.Game.gameName == gameName && x.isApproved == 1 && x.isDeleted != 1));
                    if (gameCategory != null)
                        eachDementiaViewModel.categoryBitString.Add("Yes");
                    else
                        eachDementiaViewModel.categoryBitString.Add("No");

                    AssignedGame assigned = _context.AssignedGames.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.Game.gameName == gameName && x.isApproved != 0 && x.isDeleted != 1));
                    if (assigned != null)
                        eachDementiaViewModel.gameAlreadyAssigned = true;
                    else
                        eachDementiaViewModel.gameAlreadyAssigned = false;
                }

                for (int l = 0; l < eachDementiaViewModel.categoryBitString.Count; l++)
                {
                    for (int m = 0; m < gameAssignedPatientViewModel.recommendedCategory.Count; m++)
                    {
                        if (gameAssignedPatientViewModel.recommendedCategory[m] == "Yes" && eachDementiaViewModel.categoryBitString[m] == "Yes")
                        {
                            eachDementiaViewModel.showGame = true;
                            break;
                        }
                    }
                    break;
                }
                gameAssignedPatientViewModel.viewModel.Add(eachDementiaViewModel);
            }

            for (int i = (gameSelectListItem.Count - 1); i >= 0; i--)
            {
                for (int j = 0; j < game.Count; j++)
                {
                    if (game[j].game.gameName == gameSelectListItem[i].Text)
                    {
                        gameSelectListItem.RemoveAt(i);
                        break;
                    }
                }
            }
            ViewBag.game = new SelectList(gameSelectListItem, "Value", "Text");

            List<PatientGameRecordListViewModel> gameRecordList = new List<PatientGameRecordListViewModel>();
            foreach (var games in game)
            {
                int assignedGameID = games.assignedGame.assignedGameID;
                string gameName = games.game.gameName;

                List<GameRecord> gameRecords = _context.GameRecords.Where(x => (x.assignedGameID == assignedGameID && x.isDeleted != 1)).ToList();
                foreach (var gameRecord in gameRecords)
                {
                    gameRecordList.Add(new PatientGameRecordListViewModel
                    {
                        gameName = gameName,
                        gameRecord = gameRecord
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
                                               allowRespond = (gtr.supervisorApproved == 1 && gtr.gameTherapistApproved == 2) ? 1 : 0
                                           }).OrderByDescending(x => x.gamesTypeRecommendation.isApproved).OrderByDescending(x => x.gamesTypeRecommendation.createDateTime).ToList();

            List<StaffAllocationViewModel> staffAllocation = patientMethod.getStaffAllocation(patientAllocation);

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
            GameTherapistOverviewViewModel model = new GameTherapistOverviewViewModel
            {
                patientID = patientID,
                patient = patient,
                imageUrl = patientProfilePicture.albumPath,
                mainGuardianRelationship = mainGuardianRelationship,
                diagnosedDementia = diagnosedDementia,
                staffAllocation = staffAllocation,
                mobility = mobility,
                game = game,
                allowGame = allowGame,
                gameRecommended = gamesTypeRecommendation,
                gameRecordList = gameRecordList,
                diagnosedDementiaType = diagnosedDementiaType,
                gameAssignedPatientViewModel = gameAssignedPatientViewModel,
                gameAssignedToDementia = gameAssignedToDementia,
                gameCategoryList = gameCategoryList,
                gameCategoryAssignedToDementia = gameCategoryAssignedToDementia
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


        // POST: /GameTherapist/AddGameRecommended
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult AddGameRecommended(GameTherapistOverviewViewModel model)
        {
            TempData["Info"] = "active";
            TempData["History"] = "inactive";
            TempData["GameList"] = "inactive";

            int gameID = Convert.ToInt32(Request.Form["game"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            DateTime? endDate = model.gameCategoryEndDate;
            string recommendationReason = model.recommendationReason;

            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            AssignedGame assignedGame = _context.AssignedGames.SingleOrDefault(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.gameID == gameID && x.gameTherapistID == null && x.isApproved != 0 && x.isDeleted != 1));
            if (assignedGame == null)
            {
                patientMethod.addGameRecommended(userID, patientAllocation.patientAllocationID, gameID, model.gameCategoryEndDate, model.recommendationReason, 2);
                TempData["Message"] = "Game recommendation requested! Waiting for supervisor approval.";
            }
            else
                TempData["Message"] = "Game " + assignedGame.Game.gameName + " has already been requested for patient. Please wait for supervisor to approve or reject the game!";

            TempData["Modal"] = "true";
            return RedirectToAction("ViewPatient", "GameTherapist", new { patientID = model.patientID });
        }

        // POST: /GameTherapist/addGameCategoryRecommended
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult addGameCategoryRecommended(GameTherapistOverviewViewModel model)
        {
            TempData["Info"] = "active";
            TempData["History"] = "inactive";
            TempData["GameList"] = "inactive";

            int gameCategoryID = Convert.ToInt32(Request.Form["gameCategory"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            GamesTypeRecommendation gamesTypeRecommendation = _context.GamesTypeRecommendations.SingleOrDefault(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.gameCategoryID == gameCategoryID && x.supervisorApproved == 1 && x.gameTherapistID == null && x.gameTherapistApproved == 2 && x.isApproved == 2 && x.isDeleted != 1));
            if (gamesTypeRecommendation == null)
            {
                patientMethod.addGameCategoryRecommended(userID, patientAllocation.patientAllocationID, gameCategoryID, model.gameCategoryStartDate, model.gameCategoryEndDate, model.recommendationReason, 1);
                TempData["Message"] = "Game category recommendation requested! Waiting for supervisor approval.";
            }
            else
                TempData["Message"] = "Game category of " + gamesTypeRecommendation.Category.categoryName + " has already been requested for patient. Please approve or reject the game category!";

            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "GameTherapist", new { patientID = model.patientID });
        }

        // POST: /GameTherapist/ManageGameTypeRecommendation
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult ManageGameTypeRecommendation(GameTherapistOverviewViewModel model)
        {
            TempData["Info"] = "active";
            TempData["History"] = "inactive";
            TempData["GameList"] = "inactive";

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            int approved = Convert.ToInt32(model.isApproved);
            int gamesTypeRecommendationID = model.itemID;
            string rejectionReason = model.rejectionReason;

            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.manageGameTypeRecommendation(userID, patientAllocation.patientAllocationID, gamesTypeRecommendationID, rejectionReason, approved);
            TempData["Message"] = "Responded to Recommended Game!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "GameTherapist", new { patientID = model.patientID });
        }

        // GET: /GameTherapist/ManageDementiaGame
        [HttpGet]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult ManageDementiaGame()
        {
            ViewBag.Modal = TempData["Modal"];

            ViewBag.gameForDementia = TempData["gameForDementia"];
            if (ViewBag.gameForDementia != "inactive")
                ViewBag.gameForDementia = "active";

            ViewBag.gameCategoryForDementia = TempData["gameCategoryForDementia"];
            if (ViewBag.gameCategoryForDementia != "active")
                ViewBag.gameCategoryForDementia = "inactive";

            GameTherapistDementiaGameViewModel model = new GameTherapistDementiaGameViewModel();
            List<DementiaGameViewModel> dementiaList = new List<DementiaGameViewModel>();
            List<DementiaGameCategoryViewModel> dementiaCategoryList = new List<DementiaGameCategoryViewModel>();

            List<DementiaType> dementiaTypes = _context.DementiaTypes.Where(x => (x.isApproved == 1 && x.isDeleted != 1)).OrderBy(x => x.dementiaType).ToList();

            int count = 1;
            foreach (var dementiaType in dementiaTypes)
            {
                if (count % 3 != 1)
                {
                    count++;
                    continue;
                }

                string dementiaName = patientMethod.getDementiaName(dementiaType.dementiaType);
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

            List<SelectListItem> gameSelectListItem = list.getGameList(1, false, false);
            ViewBag.game = new SelectList(gameSelectListItem, "Value", "Text");

            List<SelectListItem> gameCategorySelectListItem = list.getGameCategoryList(1, false);
            ViewBag.gameCategory = new SelectList(gameCategorySelectListItem, "Value", "Text");

            return View(model);
        }

        // POST: /GameTherapist/AddDementiaGame
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult AddDementiaGame(DoctorDementiaGameViewModel model)
        {
            TempData["gameForDementia"] = "active";
            TempData["gameCategoryForDementia"] = "inactive";

            int gameID = Convert.ToInt32(Request.Form["game"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int dementiaID = model.dementiaID;

            GameAssignedDementia gameAssignedDementia = _context.GameAssignedDementias.FirstOrDefault(x => (x.gameID == gameID && x.dementiaID == dementiaID && x.isApproved != 0 && x.isDeleted != 1));
            string dementiaString = _context.DementiaTypes.SingleOrDefault(x => (x.dementiaID == dementiaID && x.isApproved == 1 && x.isDeleted != 1)).dementiaType;
            string dementiaName = patientMethod.getDementiaName(dementiaString);

            if (gameAssignedDementia == null)
            {
                patientMethod.addDementiaGame(userID, dementiaID, gameID, model.recommendationReason, 1);
                TempData["Message"] = "Game recommendation added!";
            }
            else
                TempData["Message"] = "Game title of " + gameAssignedDementia.Game.gameName + " has already been approved for " + dementiaName + ". Please select another game!";

            TempData["Modal"] = "true";
            return RedirectToAction("ManageDementiaGame", "GameTherapist");
        }

        // POST: /GameTherapist/AddDementiaGameCategory
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult AddDementiaGameCategory(DoctorDementiaGameViewModel model)
        {
            TempData["gameForDementia"] = "inactive";
            TempData["gameCategoryForDementia"] = "active";

            int categoryID = Convert.ToInt32(Request.Form["gameCategory"]);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int dementiaID = model.dementiaID;

            GameCategoryAssignedDementia gameCategoryAssignedDementia = _context.GameCategoryAssignedDementia.FirstOrDefault(x => (x.categoryID == categoryID && x.dementiaID == dementiaID && x.isApproved != 0 && x.isDeleted != 1));
            string dementiaString = _context.DementiaTypes.SingleOrDefault(x => (x.dementiaID == dementiaID && x.isApproved == 1 && x.isDeleted != 1)).dementiaType;
            string dementiaName = patientMethod.getDementiaName(dementiaString);

            if (gameCategoryAssignedDementia == null)
            {
                patientMethod.addDementiaGameCategory(userID, dementiaID, categoryID, model.recommendationReason, 1);
                TempData["Message"] = "Game category recommendation added!";
            }
            else
                TempData["Message"] = "Game category of " + gameCategoryAssignedDementia.Category.categoryName + " has already been approved for " + dementiaName + ". Please select another game category!";

            TempData["Modal"] = "true";
            return RedirectToAction("ManageDementiaGame", "GameTherapist");
        }

        // POST: /GameTherapist/ManageDementiaGameCategory
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult ManageDementiaGameCategory(GameTherapistOverviewViewModel model)
        {
            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int gameCategoryAssignedDementiaID = model.itemID;
            int approved = Convert.ToInt32(model.isApproved);
            string rejectionReason = model.rejectionReason;

            patientMethod.manageDementiaGameCategory(userID, gameCategoryAssignedDementiaID, model.recommendationReason, rejectionReason, approved);
            TempData["Message"] = "Responded to recommended game category!";

            TempData["Modal"] = "true";
            return RedirectToAction("ManageDementiaGame", "GameTherapist");
        }

        // GET: /GameTherapist/ManageGame
        [HttpGet]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult ManageGame()
        {
            ManageGameViewModel model = new ManageGameViewModel();
            model.gameList = new List<GameViewModel>();

            List<Game> games = _context.Games.ToList();
            foreach (var game in games)
            {
                List<GameCategory> gameCategory = _context.GameCategories.Where(x => (x.gameID == game.gameID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                string gameCategoryList = "";
                foreach (var gameCat in gameCategory)
                    gameCategoryList += gameCat.Category.categoryName + ", ";

                if (gameCategoryList.Length > 2)
                    gameCategoryList = gameCategoryList.Substring(0, gameCategoryList.Length - 2);

                model.gameList.Add(new GameViewModel
                {
                    game = game,
                    gameCategory = gameCategory,
                    gameCategoryList = gameCategoryList
                });
            }

            return View(model);
        }

        // GET: /GameTherapist/AddGame
        [HttpGet]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult AddGame(AddGameViewModel model)
        {
            ViewBag.Modal = TempData["Modal"];

            List<SelectListItem> gameCategoryList = list.getGameCategoryList(1, false);
            model.category = new List<GameCategoryListViewModel>();

            for (int i=0; i<gameCategoryList.Count; i++)
            {
                model.category.Add(new GameCategoryListViewModel
                {
                    categoryID = Convert.ToInt32(gameCategoryList[i].Value),
                    categoryName = gameCategoryList[i].Text,
                });
            }

            return View(model);
        }

        // POST: /GameTherapist/AddGames
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult AddGames(AddGameViewModel model)
        {
            int userID = Convert.ToInt32(User.Identity.GetUserID2());

            Game gameExist = _context.Games.FirstOrDefault(x => (x.gameName == model.gameName && x.isApproved == 1 && x.isDeleted != 1));
            if (gameExist != null)
            {
                TempData["Message"] = "The same game title already exists";
                TempData["Modal"] = "true";
                return RedirectToAction("AddGame", "GameTherapist", new { gameName = model.gameName, gameDesc = model.gameDesc, duration = model.duration, rating = model.rating, difficulty = model.difficulty, gameCreatedBy = model.gameCreatedBy });
            }

            int gameID = gameMethod.addGame(userID, model.gameName, model.category, model.gameDesc, model.duration, model.rating, model.difficulty, model.gameCreatedBy, null, 1);
            TempData["Message"] = "Added game! Please add in the performance metric (if any).";
            TempData["Modal"] = "true";

            return RedirectToAction("GetUpdateGame", "GameTherapist", new { gameID = gameID });
        }

        // GET: /GameTherapist/GetUpdateGame
        [HttpGet]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult GetUpdateGame(int gameID)
        {
            ViewBag.Modal = TempData["Modal"];

            Game game = _context.Games.SingleOrDefault(x => (x.gameID == gameID && x.isApproved == 1 && x.isDeleted != 1));

            UpdateGameViewModel model = new UpdateGameViewModel
            {
                gameID = gameID,
                gameName = game.gameName,
                gameDesc = game.gameDesc,
                duration = game.duration,
                rating = game.rating,
                difficulty = game.difficulty,
                gameCreatedBy = game.gameCreatedBy,
                performanceMetric = _context.PerformanceMetricNames.Where(x => (x.gameID == gameID)).ToList()
            };

            List<SelectListItem> gameCategoryList = list.getGameCategoryList(1, false);
            model.gameCategory = new List<GameCategoryListViewModel>();

            for (int i = 0; i < gameCategoryList.Count; i++)
            {
                int categoryID = Convert.ToInt32(gameCategoryList[i].Value);
                bool value = true;
                if (_context.GameCategories.SingleOrDefault(x => (x.gameID == game.gameID && x.categoryID == categoryID && x.isApproved == 1 && x.isDeleted != 1)) == null)
                    value = false;

                model.gameCategory.Add(new GameCategoryListViewModel
                {
                    categoryID = Convert.ToInt32(gameCategoryList[i].Value),
                    categoryName = gameCategoryList[i].Text,
                    categoryChecked = value
                });
            }

            return View(model);
        }

        // POST: /GameTherapist/UpdateGame
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult UpdateGame(UpdateGameViewModel model)
        {
            int userID = Convert.ToInt32(User.Identity.GetUserID2());

            gameMethod.updateGame(userID, model.gameID, model.gameCategory, model.gameDesc, model.difficulty, model.duration, model.rating, model.gameCreatedBy, 1);
            TempData["Message"] = "Updated game!";

            TempData["Modal"] = "true";
            return RedirectToAction("GetUpdateGame", "GameTherapist", new { gameID = model.gameID });
        }

        // POST: /GameTherapist/AddPerformanceMetric
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult AddPerformanceMetric(UpdateGameViewModel model)
        {
            int userID = Convert.ToInt32(User.Identity.GetUserID2());

            PerformanceMetricName performanceMetricNameExist = _context.PerformanceMetricNames.FirstOrDefault(x => (x.performanceMetricName == model.performanceMetricName));
            if (performanceMetricNameExist != null)
            {
                TempData["Message"] = "Performance metric name already exists.";
            }

            else
            {
                gameMethod.addPerformanceMetric(userID, model.gameID, model.performanceMetricName, model.performanceMetricDetail, 1);
                TempData["Message"] = "Added performance metric!";
            }

            TempData["Modal"] = "true";
            return RedirectToAction("GetUpdateGame", "GameTherapist", new { gameID = model.gameID });
        }

        // POST: /GameTherapist/UpdatePerformanceMetric
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult UpdatePerformanceMetric(UpdateGameViewModel model)
        {
            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            gameMethod.updatePerformanceMetric(userID, model.gameID, model.performanceMetricName, model.performanceMetricDetail, 1);

            TempData["Message"] = "Updated performance metric!";
            TempData["Modal"] = "true";

            return RedirectToAction("GetUpdateGame", "GameTherapist", new { gameID = model.gameID });
        }

        // POST: /GameTherapist/AddGameCategoryFromGame
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult AddGameCategoryFromGame(AddGameViewModel model)
        {
            int userID = Convert.ToInt32(User.Identity.GetUserID2());

            Category categoryExist = _context.Categories.FirstOrDefault(x => (x.categoryName == model.categoryOthers && x.isApproved != 0 && x.isDeleted != 1));
            if (categoryExist != null)
            {
                if (categoryExist.isApproved != 1)
                    TempData["Message"] = "Game category of " + model.categoryOthers + " has already been requested. Please approve or reject the game category!";
                else
                    TempData["Message"] = "Game category of " + model.categoryOthers + " has already been approved.";
            }

            else
            {
                Category category = new Category
                {
                    categoryName = model.categoryOthers,
                    isApproved = 1,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.Categories.Add(category);
                _context.SaveChanges();

                string logData = new JavaScriptSerializer().Serialize(category);
                string logDesc = "New item";
                int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "category", "ALL", null, null, category.categoryID, 1, 1, null);

                TempData["Message"] = "Added game category!";
            }

            TempData["Modal"] = "true";
            return RedirectToAction("AddGame", "GameTherapist", new { gameName = model.gameName, gameDesc = model.gameDesc, duration = model.duration, rating = model.rating, difficulty = model.difficulty, gameCreatedBy = model.gameCreatedBy });
        }

        // GET: /GameTherapist/ViewGameRecord
        [HttpGet]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult ViewGameRecord()
        {
            ViewGameRecordViewModel model = new ViewGameRecordViewModel();
            model.gameRecord = new List<GameRecordViewModel>();

            List<GameRecord> gameRecords = _context.GameRecords.Where(x => (x.isDeleted != 1)).OrderByDescending(x => x.createDateTime).ToList();
            foreach(var gameRecord in gameRecords)
            {
                AssignedGame assignedGame = _context.AssignedGames.SingleOrDefault(x => (x.assignedGameID == gameRecord.assignedGameID && x.isApproved == 1 && x.isDeleted != 1));

                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == assignedGame.PatientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                GameRecordViewModel newGameRecord = new GameRecordViewModel
                {
                    gameName = _context.Games.SingleOrDefault(x => (x.gameID == gameRecord.AssignedGame.gameID && x.isApproved == 1 && x.isDeleted != 1)).gameName,
                    patientName = patient.firstName + " " + patient.lastName,
                    score = gameRecord.score,
                    timeTaken = gameRecord.timeTaken,
                    date = gameRecord.createDateTime
                };

                string[] tokens = gameRecord.performanceMetricsValues.Split(',');
                List<PerformanceMetricOrder> performanceMetricOrder = _context.PerformanceMetricOrders.Where(x => (x.gameID == assignedGame.gameID)).OrderBy(x => x.metricOrder).ToList();

                newGameRecord.performanceMetric = new List<PerformanceMetricViewModel>();

                for (int i=0; i<performanceMetricOrder.Count; i++)
                {
                    if (tokens[i] == null || tokens[i] == "0")
                        continue;

                    int performanceMetricID = performanceMetricOrder[i].pmnID;

                    newGameRecord.performanceMetric.Add(new PerformanceMetricViewModel
                    {
                        performanceMetricName = _context.PerformanceMetricNames.SingleOrDefault(x => (x.pmnID == performanceMetricID)).performanceMetricName,
                        performanceMetricDetail = tokens[i]
                    });
                }

                model.gameRecord.Add(newGameRecord);
            }

            return View(model);
        }

        // GET: /GameTherapist/ViewExportGameRecord
        [HttpGet]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult ViewExportGameRecord()
        {
            List<SelectListItem> gameSelectListItem = list.getGameList(1, true, false);
            ViewBag.game = new SelectList(gameSelectListItem, "Value", "Text");

            return View();
        }

        // POST: /GameTherapist/ExportGameRecord
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult ExportGameRecord()
        {
            int gameID = Convert.ToInt32(Request.Form["game"]);

            string userName = User.Identity.GetUserFirstName();
            int userID = Convert.ToInt32(User.Identity.GetUserID2());

            ExportToExcel(userID, userName, gameID);

            return RedirectToAction("ViewExportGameRecord", "GameTherapist");
        }

        public ExcelWorksheet addToWorkSheet(ExcelWorksheet ws, int gameID)
        {
            List<PerformanceMetricOrder> performanceMetricOrder = _context.PerformanceMetricOrders.Where(x => (x.gameID == gameID)).OrderBy(x => x.metricOrder).ToList();
            // Add heading title
            ws.Cells["A1"].Value = "Patient Name";
            ws.Cells["B1"].Value = "Score";
            ws.Cells["C1"].Value = "Time Taken";
            ws.Cells["D1"].Value = "Date";
            ws.Cells["E1"].Value = "Time";

            char letter = 'F';
            for (int i=0; i<performanceMetricOrder.Count; i++)
            {
                letter = (char)(((int)letter) + 1);

                ws.Cells[letter.ToString() + "1"].Value = performanceMetricOrder[i].PerformanceMetricName.performanceMetricName;
            }

            List<GameRecord> gameRecords = _context.GameRecords.Where(x => (x.isDeleted != 1)).OrderByDescending(x => x.createDateTime).ToList();
            for (int j=0; j<gameRecords.Count; j++)
            {
                int assignedGameID = gameRecords[j].assignedGameID;
                AssignedGame assignedGame = _context.AssignedGames.SingleOrDefault(x => (x.assignedGameID == assignedGameID && x.isApproved == 1 && x.isDeleted != 1));
                if (assignedGame.gameID != gameID)
                    continue;

                ws.Cells["A" + (j + 2).ToString()].Value = gameRecords[j].AssignedGame.PatientAllocation.Patient.firstName + " " + gameRecords[j].AssignedGame.PatientAllocation.Patient.lastName;
                ws.Cells["B" + (j + 2).ToString()].Value = gameRecords[j].score;
                ws.Cells["C" + (j + 2).ToString()].Value = gameRecords[j].timeTaken;
                ws.Cells["D" + (j + 2).ToString()].Value = gameRecords[j].createDateTime.ToString("dd/MM/yyyy");
                ws.Cells["E" + (j + 2).ToString()].Value = gameRecords[j].createDateTime.ToString("HH:mm");

                letter = 'F';
                string[] tokens = gameRecords[j].performanceMetricsValues.Split(',');
                for (int k = 0; k < performanceMetricOrder.Count; k++)
                {
                    letter = (char)(((int)letter) + 1);

                    ws.Cells[letter.ToString() + (j + 2).ToString()].Value = tokens[k];
                }
            }
            return ws;
        }

        public void ExportToExcel(int userID, string userName, int gameID)
        {
            ExcelPackage pck = new ExcelPackage();
            string gameName = "All Game";

            if (gameID == 0)
            {
                List<Game> games = _context.Games.Where(x => (x.isApproved == 1 && x.isDeleted != 1)).ToList();
                foreach(var game in games)
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(game.gameName);
                    ws = addToWorkSheet(ws, game.gameID);
                    ws.Cells["A:AZ"].AutoFitColumns();
                    ws.Protection.IsProtected = true;
                }
            }
            else
            {
                Game game = _context.Games.SingleOrDefault(x => (x.gameID == gameID && x.isApproved == 1 && x.isDeleted != 1));
                gameName = game.gameName;

                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(gameName);
                ws = addToWorkSheet(ws, game.gameID);
                ws.Cells["A:AZ"].AutoFitColumns();
                ws.Protection.IsProtected = true;
            }
            
            string password = userName;
            string logDesc = "Export Game";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, null, logDesc, logCategoryID, null, userID, userID, null, null, null, "gameRecord", null, null, null, null, 1, 1, null);

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=Game Record for " + gameName + " on " + scheduler.getDateNumberOnly(DateTime.Today) + ".xlsx");
            Response.BinaryWrite(pck.GetAsByteArray(password));
            Response.End();
        }

        // GET: /GameTherapist/Find
        [HttpGet]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult Find()
        {
            ViewBag.result = "none";

            List<SelectListItem> searchSelectListItem = list.getGameTherapistSearchList();
            ViewBag.search = new SelectList(searchSelectListItem, "Value", "Text");

            return View();
        }

        // POST: /GameTherapist/FindResult
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult FindResult(FindViewModel model)
        {
            ViewBag.result = "block";

            string searchType = Request.Form["search"];
            List<SelectListItem> searchSelectListItem = list.getGameTherapistSearchList();
            ViewBag.search = new SelectList(searchSelectListItem, "Value", "Text", searchType);

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            List<SearchResultViewModel> result = patientMethod.search("GameTherapist", searchType, model.searchWords);

            return View("Find", new FindViewModel { searchWords = model.searchWords, result = result });
        }

        // POST: /GameTherapist/AddGameForPatient
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult AddGameForPatient(GameTherapistOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "inactive";
            TempData["GameList"] = "active";

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            List<AssignNewGameViewModel> assignPatientViewModel = model.assignPatientViewModel;
            DateTime? endDate = model.gameCategoryEndDate;
            string recommendationReason = model.recommendationReason;

            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.addRecommendGameForPatient(userID, patientAllocation.patientAllocationID, assignPatientViewModel, endDate, recommendationReason, 2);
            TempData["Message"] = "Recommended Game for patient!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "GameTherapist", new { patientID = model.patientID });
        }

        // POST: /GameTherapist/AddRecommendGameForPatient
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult AddRecommendGameForPatient(GameTherapistOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "inactive";
            TempData["GameList"] = "active";

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            List<AssignNewGameViewModel> assignNewGameViewModel = model.assignNewGameViewModel;
            DateTime? endDate = model.gameCategoryEndDate;
            string recommendationReason = model.recommendationReason;

            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.addRecommendGameForPatient(userID, patientAllocation.patientAllocationID, assignNewGameViewModel, endDate, recommendationReason, 2);
            TempData["Message"] = "Recommended Game for patient!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "GameTherapist", new { patientID = model.patientID });
        }

        // POST: /GameTherapist/AddRecommendGameCategoryForPatient
        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult AddRecommendGameCategoryForPatient(GameTherapistOverviewViewModel model)
        {
            TempData["Info"] = "inactive";
            TempData["History"] = "inactive";
            TempData["GameList"] = "active";

            int userID = Convert.ToInt32(User.Identity.GetUserID2());
            int patientID = model.patientID;
            List<AssignNewGameViewModel> assignNewGameViewModel = model.assignNewGameCategoryViewModel;
            DateTime? endDate = model.gameCategoryEndDate;
            string recommendationReason = model.recommendationReason;

            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1));

            patientMethod.addRecommendGameForPatient(userID, patientAllocation.patientAllocationID, assignNewGameViewModel, endDate, recommendationReason, 2);
            TempData["Message"] = "Recommended Game for patient!";
            TempData["Modal"] = "true";

            return RedirectToAction("ViewPatient", "GameTherapist", new { patientID = model.patientID });
        }

        [Authorize(Roles = RoleName.isGameTherapist)]
        public ActionResult ManageGameTherapist()
        {
            //Only allow GameTherapist to view this page
            if (User.IsInRole(RoleName.isGameTherapist))
            {
                var users = _context.Users.ToList();
                var patients = _context.Patients.ToList();
                var patientAllocations = _context.PatientAllocations.ToList();
                var albums = _context.AlbumPatient.ToList();
                var gameRecommend = _context.GamesTypeRecommendations.ToList();
                var allergies = _context.Allergies.ToList();;

                var viewModel = new ManagePatientsViewModel()
                {
                    Users = users,
                    Patients = patients,
                    PatientAllocations = patientAllocations,
                    Albums = albums,
                    GamesTypeRecommendations = gameRecommend,
                    Allergies = allergies,
                };

                return View(viewModel);
            }
            else
            {
                return View("_LoginPage");
            }
        }

        public ActionResult Help()
        {
            if (User.IsInRole(RoleName.isGameTherapist))
            {
                return View("Help");
            }

            else
            {
                return View("_LoginPage");
            }
        }

        ///  Get Performance Matric Name by GameID
        private List<PerformanceMatricOrderList> GetPerformanceMatricByGameID(int gameID)
        {
            return (from g in _context.Games
                         join pmn in _context.PerformanceMetricNames on g.gameID equals pmn.gameID
                         join pmo in _context.PerformanceMetricOrders on pmn.pmnID equals pmo.pmnID
                    where g.gameID == gameID
                         orderby pmo.metricOrder
                         select new PerformanceMatricOrderList { PerformanceMetricName = pmn.performanceMetricName, PerformanceMetricNameID = pmn.pmnID, MetricOrder = pmo.metricOrder }).ToList();
        }

        public ActionResult ExportAllPerformanceMetrics(int? selectedGame)
        {
            if (User.IsInRole(RoleName.isGameTherapist))
            {
                /// Set export error to viewbag
                ViewBag.Error = TempData["ErrorMessage"];

                var viewModel = new ExportAllPerformanceMetricsViewModel();
                /// Get Game list for dropdown
                var game = _context.Games.ToList();
                viewModel.Games = game;
                /// Get game attribute if game is selected
                if (selectedGame.HasValue)
                {
                    viewModel.PerformanceMatricOrderList = GetPerformanceMatricByGameID((int)selectedGame);
                }
                /// Set GameID in viewbag for export
                ViewBag.GameID = selectedGame;
                return View(viewModel);
            }
            else
            {
                return View("_LoginPage");
            }
        }
        
        public ActionResult ExportToCSV(int? GameID, DateTime? startDate, DateTime? endDate, string GameAttributes)
        {
            /// Get game record query by gameid
            var grQurery = _context.GameRecords.Where(x => x.AssignedGame.gameID == GameID);

            /// Filter by startDate if  startDate have balue
            if (startDate.HasValue)
                grQurery = grQurery.Where(x => DbFunctions.TruncateTime(x.createDateTime) >= startDate);

            /// Filter by endDate if endDate have value
            if (endDate.HasValue)
                grQurery = grQurery.Where(x => DbFunctions.TruncateTime(x.createDateTime) <= endDate);

            /// Get required data by linq operation
            var result = grQurery.Select(x => new {
                x.performanceMetricsValues,
                x.AssignedGame.Game.gameName,
                x.score,
                x.timeTaken,
                x.createDateTime,
                x.AssignedGame.PatientAllocation.Patient.gender,
                x.AssignedGame.PatientAllocation.Patient.firstName,
                x.AssignedGame.PatientAllocation.Patient.lastName
            }).ToList();
           
            /// If no data found then return with error message
            if (result.Count == 0)
            {
                TempData["ErrorMessage"] = "No data found";
                return RedirectToAction("ExportAllPerformanceMetrics", "GameTherapist", new { selectedGame = GameID });
            }

            /// Get selected game attributes
            List<int> matricOrders = GameAttributes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

            /// Generate Header for CSV export
            List<string> header = new List<string>();
            header.Add("gender");
            header.Add("score");
            header.Add("timeTaken");
            header.Add("createDateTime");
            header.Add("fullName");
            header.Add("day");
            header.Add("month");
            header.Add("year");

            /// Get performance matric name by game id
            var preGetPerformanceMatrics = GetPerformanceMatricByGameID((int)GameID);
            /// Add selected header in header list
            foreach (var matricOrder in matricOrders)
            {
                header.Add(preGetPerformanceMatrics.FirstOrDefault(x => x.MetricOrder == matricOrder)?.PerformanceMetricName);
            }

            /// generate list of game record for export result
            List<List<string>> finalResult = new List<List<string>>();
            foreach (var item in result)
            {
                List<string> newRecord = new List<string>();
                newRecord.Add(item.gender.ToString());
                newRecord.Add(item.score.ToString());
                newRecord.Add(item.timeTaken.ToString());
                newRecord.Add(item.createDateTime.ToShortDateString());
                newRecord.Add(item.firstName + " " + item.lastName);
                newRecord.Add(item.createDateTime.Day.ToString());
                newRecord.Add(item.createDateTime.Month.ToString());
                newRecord.Add(item.createDateTime.Year.ToString());

                /// Add selected game attribute value
                var performanceMetricsValues = item.performanceMetricsValues.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var matricOrder in matricOrders)
                {
                    newRecord.Add(performanceMetricsValues[matricOrder - 1]);
                }
                finalResult.Add(newRecord);
            }
            
            /// Generate csv record
            StringWriter sw = new StringWriter();
            
            /// Write header row
            sw.WriteLine(string.Join(",", header));

            /// Write game record
            foreach (var item in finalResult)
            {
                sw.WriteLine(string.Join(",", item));
            }
            /// generate file name
            var fileName = "AllDataExported" + DateTime.Now.ToString() + ".csv";

            /// return file object
            return File(new System.Text.UTF8Encoding().GetBytes(sw.ToString()), "text/csv", fileName);
        }
    }
}