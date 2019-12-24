using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.Dtos;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;

namespace NTU_FYP_REBUILD_17.Controllers.Api
{
	public class PatientListPageController : ApiController
	{
		private ApplicationDbContext _context;
		App_Code.SOLID shortcutMethod = new App_Code.SOLID();
		public PatientListPageController()
		{
			_context = new ApplicationDbContext();

		}

		[HttpGet]
		[Route("api/PatientListPage/GetAlbumDoctorNoteFromPatient_XML")]
		public IHttpActionResult GetAlbumDoctorNoteFromPatient_XML(int patientID, string token)
		{
			shortcutMethod.printf(shortcutMethod.getUserType(token, null));
			if ((shortcutMethod.getUserType(token, null).Equals("NONE")) || (shortcutMethod.getUserType(token, null).Equals("Guardian")))
			{
				shortcutMethod.printf("Incorrect token/Not authorised");
				return BadRequest("Invalid Token/Not authorised");
			}
			shortcutMethod.printf("In GetAlbumDoctorNoteFromPatient_XML");
			var patient = _context.Patients.Where(x => (x.isDeleted == 0 && x.isApproved == 1 && x.patientID == patientID)).ToList();
            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            var album = _context.AlbumPatient.Where(z => z.patientAllocationID == patientAllocation.patientAllocationID).Where(x => (x.isDeleted == 0 && x.isApproved == 1)).ToList();
			var DoctorNote = _context.DoctorNotes.Where(z => z.patientAllocationID == patientID).Where(x => (x.isDeleted == 0 && x.isApproved == 1)).ToList();
			List<PatientAlbumDoctorNoteViewModel> q = new List<PatientAlbumDoctorNoteViewModel>();

			if (patient != null && album != null && DoctorNote != null)
			{
				shortcutMethod.printf("In if");
				q = (from p in patient
					 join a in album on p.patientID equals a.albumID
                     join d in DoctorNote on p.patientID equals d.patientAllocationID
                     select new PatientAlbumDoctorNoteViewModel
					 {
						 patient_address = p.address,
						 patient_autoGame = p.autoGame,
						 patient_createDateTime = p.createDateTime,
						 patient_DOB = p.DOB,
						 patient_firstName = p.firstName,
						 patient_gender = p.gender,
						 //patient_guardianContactNo = p.guardianContactNo,
						 //patient_guardianName = p.guardianName,
						 //patient_guardianEmail = p.guardianEmail,
						 patient_handphoneNo = p.handphoneNo,
						 patient_isApproved = p.isApproved,
						 patient_isDeleted = p.isDeleted,
						 patient_lastName = p.lastName,
						 patient_nric = p.nric,
						 //patient_preferredLanguage = p.preferredLanguage,
						 patient_preferredName = p.preferredName,
						 patient_updateBit = p.updateBit,
						 patient_patientID = p.patientID,
						 album_albumID = a.albumID,
						 album_albumCatID = a.albumCatID,
						 album_albumPath = a.albumPath,
						 album_createDateTime = a.createDateTime,
						 album_isApproved = a.isApproved,
						 album_isDeleted = a.isDeleted,
						 album_patientID = p.patientID,
						 doctorNote_createDateTime = d.createDateTime,
						 doctorNote_isApproved = d.isApproved,
						 doctorNote_isDeleted = d.isDeleted,
						 doctorNote_note = d.note,
						 doctorNote_patientID = d.patientAllocationID,
						 doctorNote_doctorNoteID = d.doctorNoteID
					 }).ToList();
			}

			for (int x = 0; x < patient.Count(); x++)
			{
				shortcutMethod.printf(patient[x].address);
				shortcutMethod.printf("" + patient[x].autoGame);
				shortcutMethod.printf("" + patient[x].createDateTime);
				shortcutMethod.printf("" + patient[x].autoGame);
			}
			return Ok(q.Select(Mapper.Map<PatientAlbumDoctorNoteViewModel, PatientAlbumDoctorNoteDTOViewModel>));
		}

		[HttpGet] // Working get 
		[Route("api/PatientListPage/GetHolidayExperienceEntry_XML")]
		public IHttpActionResult GetHolidayExperienceEntry_XML(int patientID, string token)
		{

			int testing = String.Compare(token, "1234", true);
			var dateTime = System.DateTime.Now.AddHours(-1);
			if (testing == 0 || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0)))
			{
				int socialhistoryID = 0;
				try
				{
					socialhistoryID = _context.SocialHistories.Where(a => a.isApproved == 1).Where(b => b.isDeleted == 0).FirstOrDefault(x => x.patientAllocationID == patientID).socialHistoryID;
				}
				catch
				{
					socialhistoryID = 0;
				}
				var HolidayExpEntry = _context.HolidayExperiences.Where(b => (b.isApproved == 1 && b.isDeleted == 0) || (b.isApproved == 0 && b.isDeleted == 1)).Where(p => p.socialHistoryID == socialhistoryID).ToList();

				if (HolidayExpEntry == null || socialhistoryID.ToString() == "")
				{
					return NotFound();
				}
				return Ok(HolidayExpEntry.Select(Mapper.Map<HolidayExperience, HolidayExperienceDto>));
			}
			else
			{
				return BadRequest("Invalid Token");
			}
		}
		[HttpGet] // Working uncomment and use this
		[Route("api/PatientListPage/test_XML")]
		public IHttpActionResult test_XML(int patientID, string notes)
		{
			var test = _context.Vitals.Where(x => (x.patientAllocationID == patientID && x.notes == notes));
			return Ok(test.Select(Mapper.Map<Vital, VitalDto>));
		}


		[HttpGet] // Working uncomment and use this
		[Route("api/PatientListPage/likeitem_XML")]
		public IHttpActionResult likeitem_XML(int patientID, string token)
		{
			int testing = String.Compare(token, "1234", true);
			var dateTime = System.DateTime.Now.AddHours(-1);
			if (testing == 0 || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0)))
			{
				int socialhistoryID = 0;
				try
				{
					socialhistoryID = _context.SocialHistories.Where(a => a.isApproved == 1).Where(b => b.isDeleted == 0).FirstOrDefault(x => x.patientAllocationID == patientID).socialHistoryID;
				}
				catch
				{
					socialhistoryID = 0;
				}
				return Ok(_context.Likes.Where(x => x.socialHistoryID == socialhistoryID).Where(b => (b.isApproved == 1 && b.isDeleted == 0) || (b.isApproved == 0 && b.isDeleted == 1)).ToList().Select(Mapper.Map<Like, LikesDto>));
			}
			else
			{
				return BadRequest("Invalid Token");
			}
		}


		[HttpGet] // Working uncomment and use this
		[Route("api/PatientListPage/dislikeitem_XML")]
		public IHttpActionResult dislikeitem_XML(int patientID, string token)
		{
			int testing = String.Compare(token, "1234", true);
			var dateTime = System.DateTime.Now.AddHours(-1);
			if (testing == 0 || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0)))
			{
				int socialhistoryID = 0;
				try
				{
					socialhistoryID = _context.SocialHistories.Where(a => a.isApproved == 1).Where(b => b.isDeleted == 0).FirstOrDefault(x => x.patientAllocationID == patientID).socialHistoryID;
				}
				catch
				{
					socialhistoryID = 0;
				}
				return Ok(_context.Dislikes.Where(x => x.socialHistoryID == socialhistoryID).Include(y => y.SocialHistory).Where(b => (b.isApproved == 1 && b.isDeleted == 0) || (b.isApproved == 0 && b.isDeleted == 1)).ToList().Select(Mapper.Map<Dislike, DislikesDto>));
			}
			else
			{
				return BadRequest("Invalid Token");
			}
		}



		[HttpGet] // Working uncomment and use this no9
		[Route("api/PatientListPage/problemlog_XML")]
		public IHttpActionResult problemlog_XML(int patientID, string token)
		{
			int testing = String.Compare(token, "1234", true); // 0 = true
			var dateTime = System.DateTime.Now.AddHours(-1);
			if (testing == 0 || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0)))
			{
				var problemlogResult = Ok(_context.ProblemLogs.Where(a => a.patientAllocationID == patientID).Where(b => b.isApproved == 1).Where(c => c.isDeleted == 0).OrderByDescending(d => d.createdDateTime).ToList().Select(Mapper.Map<ProblemLog, problemlogDto>));
				return problemlogResult;

			}
			else
			{
				return BadRequest("Invalid Token");
			}
		}

		[HttpGet] // Working uncomment and use this 5 need to have patientId parameter and order by createDateTime Desc
		[Route("api/PatientListPage/vital_XML")]
		public IHttpActionResult vital_XML(int patientID, string token)
		{
			int testing = String.Compare(token, "1234", true); // 0 = true
			var dateTime = System.DateTime.Now.AddHours(-1);
			if (testing == 0 || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0)))
			{
				var vitalResult = Ok(_context.Vitals.Where(a=>a.patientAllocationID == patientID).Where(b => b.isApproved == 1).Where(c => c.isDeleted == 0).OrderByDescending(d=>d.createDateTime).ToList().Select(Mapper.Map<Vital, VitalDto>));
				return vitalResult;

			}
			else
			{
				return BadRequest("Invalid Token");
			}
		}

		[HttpGet] // Working uncomment and use this no6
		[Route("api/PatientListPage/allergy_XML")]
		public IHttpActionResult allergy_XML(int patientID,string token)
		{
			int testing = String.Compare(token, "1234", true); // 0 = true
			var dateTime = System.DateTime.Now.AddHours(-1);
			if (testing == 0 || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0)))
			{
				var allergyResult = Ok(_context.Allergies.Where(a=>a.patientAllocationID == patientID).Where(b => b.isApproved == 1).Where(c => c.isDeleted == 0).OrderByDescending(d=>d.createDateTime).ToList().Select(Mapper.Map<Allergy, AllergyDto>));
				return allergyResult;

			}
			else
			{
				return BadRequest("Invalid Token");
			}
		}

		[HttpGet] // Working uncomment and use this
		[Route("api/PatientListPage/prescription_XML")]
		public IHttpActionResult prescription_XML(int patientID, string token)
		{
			int testing = String.Compare(token, "1234", true); // 0 = true
			var dateTime = System.DateTime.Now.AddHours(-1);
			if (testing == 0 || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0)))
			{
				var prescriptionResult = Ok(_context.Prescriptions.Where(a => a.patientAllocationID == patientID).Where(b => b.isApproved == 1).Where(c => c.isDeleted == 0).OrderByDescending(d=>d.prescriptionID).ToList().Select(Mapper.Map<Prescription, PrescriptionDto>));
				return prescriptionResult;

			}
			else
			{
				return BadRequest("Invalid Token");
			}
		}

		[HttpGet] // Working uncomment and use this
		[Route("api/PatientListPage/routine_XML")]
		public IHttpActionResult routine_XML(int patientID, string token)
		{
			int testing = String.Compare(token, "1234", true); // 0 = true
			var dateTime = System.DateTime.Now.AddHours(-1);
			if (testing == 0 || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0)))
			{
				var routineResult = Ok(_context.Routines.Where(a => a.patientAllocationID == patientID).Where(b => b.isApproved == 1).Where(c => c.isDeleted == 0).OrderByDescending(d => d.createDateTime).ToList().Select(Mapper.Map<Routine, RoutineDto>));
				return routineResult;

			}
			else
			{
				return BadRequest("Invalid Token");
			}
		}

		[HttpGet] // Working uncomment and use this
		[Route("api/PatientListPage/activityPreferenceandcentreActivity_XML")]
		public IHttpActionResult activityPreferenceandcentreActivity_XML(int patientID, string token)
		{

			int testing = String.Compare(token, "1234", true); // 0 = true
			var dateTime = System.DateTime.Now.AddHours(-1);
			if (testing == 0 || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0)))
			{
				var activityPreferenceandcentreActivityResult = Ok(_context.ActivityPreferences.Where(a => a.patientAllocationID == patientID).Include(x=>x.CentreActivity).Where(b=>b.isApproved==1).Where(c=>c.isDeleted==0).ToList().Select(Mapper.Map<ActivityPreference, ActivityPreferenceDto>));
				return activityPreferenceandcentreActivityResult;

			}
			else
			{
				return BadRequest("Invalid Token");
			}

			

		}

		[HttpGet] // Working uncomment and use this
		[Route("api/PatientListPage/scheduleandpatientAllocationandcentreActivity_XML")]
		public IHttpActionResult scheduleandpatientAllocationandcentreActivity_XML(int patientID, DateTime todaydate,string token)
		{
			int testing = String.Compare(token, "1234", true); // 0 = true
			var dateTime = System.DateTime.Now.AddHours(-1);

			if (testing == 0 || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0)))
			{
				int patientAllocationID = _context.PatientAllocations.FirstOrDefault(x => x.patientID == patientID).patientAllocationID;
				var scheduleandpatientAllocationandcentreActivityResult = Ok(_context.Schedules.Where(a=>a.patientAllocationID == patientAllocationID).Where(b => b.isApproved == 1).Where(c => c.isDeleted == 0).Where(d=>d.dateStart==todaydate).Include(x => x.PatientAllocation).Include(w=>w.CentreActivity).Include(y=>y.Routine).OrderBy(z=>z.timeStart).ToList().Select(Mapper.Map<Schedule, zhSchedulesDto>));
				return scheduleandpatientAllocationandcentreActivityResult;

			}
			else
			{
				return BadRequest("Invalid Token");
			}
		}


		[HttpGet] // Working uncomment and use this no1
		[Route("api/PatientListPage/album_XML")]
		public IHttpActionResult album_XML(int patientID, string token)
		{
			int testing = String.Compare(token, "1234", true); // 0 = true
			var dateTime = System.DateTime.Now.AddHours(-1);
			if (testing == 0 || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0)))
			{
                var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                var albumresult = Ok(_context.AlbumPatient.Where(a => a.patientAllocationID == patientAllocation.patientAllocationID).Where(b => b.isApproved == 1).Where(c => c.isDeleted == 0).ToList().Select(Mapper.Map<AlbumPatient, AlbumDto>));
				return albumresult;

			}
			else
			{
				return BadRequest("Invalid Token");
			}
		}

		[HttpGet] // Working uncomment and use this no1
		[Route("api/PatientListPage/patient_XML")]
		public IHttpActionResult patient_XML(int patientID, string token)
		{
			int testing = String.Compare(token, "1234", true); // 0 = true
			var dateTime = System.DateTime.Now.AddHours(-1);
			if (testing == 0 || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0)))
			{
				var albumresult = Ok(_context.Patients.Where(a => a.patientID == patientID).Where(b => b.isApproved == 1).Where(c => c.isDeleted == 0).ToList().Select(Mapper.Map<Patient, PatientDto>));
				return albumresult;

			}
			else
			{
				return BadRequest("Invalid Token");
			}
		}

		[HttpGet] // Working uncomment and use this no1
		[Route("api/PatientListPage/DoctorNote_XML")]
		public IHttpActionResult DoctorNote_XML(int patientID, string token)
		{
			int testing = String.Compare(token, "1234", true); // 0 = true
			var dateTime = System.DateTime.Now.AddHours(-1);
			if (testing == 0 || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0)))
			{
				var albumresult = Ok(_context.DoctorNotes.Where(a => a.patientAllocationID == patientID).Where(b => b.isApproved == 1).Where(c => c.isDeleted == 0).ToList().Select(Mapper.Map<DoctorNote, DoctorNoteDto>));
				return albumresult;

			}
			else
			{
				return BadRequest("Invalid Token");
			}
		}

		[HttpGet] // Working uncomment and use this no2
		[Route("api/PatientListPage/socialHistories_XML")]
		public IHttpActionResult socialHistories_XML(int patientID, string token)
		{
			int testing = String.Compare(token, "1234", true); // 0 = true
			var dateTime = System.DateTime.Now.AddHours(-1);
			if (testing == 0 || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0)))
			{
				int socialhistoryID = 0;
				try
				{
					socialhistoryID = _context.SocialHistories.Where(a => a.isApproved == 1).Where(b => b.isDeleted == 0).FirstOrDefault(x => x.patientAllocationID == patientID).socialHistoryID;
				}
				catch
				{
					socialhistoryID = 0;
				}
				var socialhistoriesResult = Ok(_context.SocialHistories.Where(a => a.patientAllocationID == patientID).Where(b => b.isApproved == 1).Where(c => c.isDeleted == 0).ToList().Select(Mapper.Map<SocialHistory, SocialHistoryDto>));
				return socialhistoriesResult;

			}
			else
			{
				return BadRequest("Invalid Token");
			}
		}

		[HttpGet] // Working uncomment and use this no2
		[Route("api/PatientListPage/Habits_XML")]
		public IHttpActionResult Habits_XML(int patientID, string token)
		{
			int testing = String.Compare(token, "1234", true); // 0 = true
			var dateTime = System.DateTime.Now.AddHours(-1);
			if (testing == 0 || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0)))
			{
				int socialhistoryID = 0;
				try
				{
					socialhistoryID = _context.SocialHistories.Where(a => a.isApproved == 1).Where(b => b.isDeleted == 0).FirstOrDefault(x => x.patientAllocationID == patientID).socialHistoryID;
				}
				catch
				{
					socialhistoryID = 0;
				}
				var habitsResult = Ok(_context.Habits.Where(a => a.socialHistoryID == socialhistoryID).Where(b => b.isApproved == 1).Where(c => c.isDeleted == 0).ToList().Select(Mapper.Map<Habit, HabitDto>));
				return habitsResult;

			}
			else
			{
				return BadRequest("Invalid Token");
			}
		}

		[HttpGet] // Working uncomment and use this no2
		[Route("api/PatientListPage/Hobbies_XML")]
		public IHttpActionResult Hobbies_XML(int patientID, string token)
		{
			int testing = String.Compare(token, "1234", true); // 0 = true
			var dateTime = System.DateTime.Now.AddHours(-1);
			if (testing == 0 || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0)))
			{
				int socialhistoryID = 0;
				try
				{
					socialhistoryID = _context.SocialHistories.Where(a => a.isApproved == 1).Where(b => b.isDeleted == 0).FirstOrDefault(x => x.patientAllocationID == patientID).socialHistoryID;
				}
				catch
				{
					socialhistoryID = 0;
				}
				var habitsResult = Ok(_context.Hobbieses.Where(a => a.socialHistoryID == socialhistoryID).Where(b => b.isApproved == 1).Where(c => c.isDeleted == 0).ToList().Select(Mapper.Map<Hobbies, HobbiesDto>));
				return habitsResult;

			}
			else
			{
				return BadRequest("Invalid Token");
			}
		}

		//http://mvc.fyp2017.com/api/PatientListPage/activityPrefcentreActivity_XML?token=1234&patientID=1
		[HttpGet] // Working uncomment and use this no2
		[Route("api/PatientListPage/activityPrefcentreActivity_XML")]
		public IHttpActionResult activityPrefcentreActivity_XML(int patientID, string token)
		{
			string userType = shortcutMethod.getUserType(token, null);
			if (userType.Equals("Guardian") || userType.Equals("NONE"))
				return BadRequest("Invalid Token");
			var activityPreference = _context.ActivityPreferences.Where(x => (x.isApproved == 1 && x.isDeleted == 0));
			var centreActivity = _context.CentreActivities.Where(x => (x.isApproved == 1 && x.isDeleted == 0));
			List<activityPref_centreActivityViewModel> q = new List<activityPref_centreActivityViewModel>();
			if (activityPreference != null && centreActivity != null)
			{
				q = (from a in activityPreference
					 join c in centreActivity on a.centreActivityID equals c.centreActivityID
					 where a.patientAllocationID == patientID
					 select new activityPref_centreActivityViewModel
					 {
						 activityPref_patientID = a.patientAllocationID,
						 activityPref_centreActivityID = a.centreActivityID,
						 activityPref_isLike = a.isLike,
						 activityPref_isDislike = a.isDislike,
						 activityPref_isNeutral = a.isNeutral,
						 activityPref_isApproved = a.isApproved,
						 activityPref_doctorRecommendation = a.doctorRecommendation,
						 activityPref_isDeleted = a.isDeleted,
						 activityPref_doctorRemarks = a.doctorRemarks,

						 centreActivity_centreActivityID = c.centreActivityID,
						 centreActivity_activityTitle = c.activityTitle,
						 centreActivity_activityDesc = c.activityDesc,
						 centreActivity_isCompulsory = c.isCompulsory,
						 centreActivity_isFixed = c.isFixed,
						 centreActivity_isGroup = c.isGroup,
						 centreActivity_minDuration = c.minDuration,
						 centreActivity_maxDuration = c.maxDuration,
						 centreActivity_isDeleted = c.isDeleted,
						 centreActivity_isApproved = c.isApproved,
						 centreActivity_minPeopleReq = c.minPeopleReq,
						 centreActivity_createDateTime = c.createDateTime,
					 }).ToList();
				return Ok(q.Select(Mapper.Map<activityPref_centreActivityViewModel, activityPref_centreActivityViewModel>));
			}
			return BadRequest("Invalid Token");
		}

        //https://localhost:44300/api/PatientListPage/displayViewedPatient_JSONString?token=1234&patientID=63
        [HttpGet]
        [Route("api/PatientListPage/displayViewedPatient_JSONString")]
        public HttpResponseMessage displayViewedPatient_JSONString(string token, int patientID)
        {
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;
            var viewPatient = _context.Patients.SingleOrDefault(x => x.patientID == patientID);
            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted != 1));
            var socialHistory = _context.SocialHistories.Where(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            List<int> socialHistoryIDs = new List<int>();
            JArray jarrayLikes = new JArray();
            JArray jarrayDislike = new JArray();
            JArray jarrayAllergy = new JArray();
            JArray jarraySchedule = new JArray();
            JArray jarrayPatient = new JArray();
            JArray jarrayAlbum = new JArray();
            JArray jarrayVital = new JArray();
            JArray jarrayDocNote = new JArray();
            JObject o = new JObject();
            JObject a = new JObject();
            //Added Missing Data
            JObject c = new JObject();

            for (int i = 0; i < socialHistory.Count(); i++)
            {
                socialHistoryIDs.Add(socialHistory[i].socialHistoryID);
            }

            var patient = _context.Patients.SingleOrDefault((x => (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted != 1)));
            if (patient == null)
                return null;

            var language = _context.Languages.SingleOrDefault(x => x.languageID == patient.preferredLanguageID && x.isDeleted != 1 && x.isApproved == 1).languageListID;
            var listLanguage = _context.ListLanguages.SingleOrDefault(x => x.list_languageID == language && x.isDeleted != 1).value;
            a["preferredName"] = patient.preferredName;
            a["preferredLanguage"] = listLanguage;
            a["handphoneNo"] = patient.handphoneNo;
            a["DOB"] = patient.DOB;
            a["NRIC"] = patient.nric.Remove(1, 4).Insert(1, "xxxx");
            a["firstName"] = patient.firstName;
            a["lastName"] = patient.lastName;
            //Added Missing Data
            a["gender"] = patient.gender;
            jarrayPatient.Add(a);
            o["Patients"] = a;

            //Added Missing Data
            //var vital = _context.Vitals.SingleOrDefault((x => (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted == 0)));
            var vital = _context.Vitals.Where((x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1))).OrderByDescending(x => x.createDateTime).Take(1).ToList().FirstOrDefault();

            if (vital == null)
            {
                c["bloodPressure"] = null;
                c["temperature"] = null;
            }
            else
            {
                c["bloodPressure"] = vital.bloodPressure;
                c["temperature"] = vital.temperature;
            }

            o["Vitals"] = c;

            //Added Missing Data
            var DoctorNote = _context.DoctorNotes.Where(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();

            List<int> doctorNoteID = new List<int>();
            JArray jarraynote = new JArray();
            for (int i = 0; i < DoctorNote.Count(); i++)
            {
                doctorNoteID.Add(DoctorNote[i].doctorNoteID);
            }
            if (doctorNoteID.Count() == 0)
            {
                o["Notes"] = jarraynote;
            }


      
            for (int i = 0; i < doctorNoteID.Count(); i++)
            {
                int docnotID = doctorNoteID[i];
                var note = _context.DoctorNotes.Where(x => (x.doctorNoteID == docnotID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                for (int j = 0; j < note.Count(); j++)
                {
                    jarraynote.Add(note[j].note);
                }
                o["Notes"] = jarraynote;
            }

            if (socialHistoryIDs.Count() == 0)
            {
                o["Likes"] = jarrayLikes;
                o["Dislike"] = jarrayDislike;
                o["Allergy"] = jarrayAllergy;
            }

            for (int i = 0; i < socialHistoryIDs.Count(); i++)
            {
                int socialhisID = socialHistoryIDs[i];
                var likes = _context.Likes.Where(x => (x.socialHistoryID == socialhisID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                var dislikes = _context.Dislikes.Where((x => x.socialHistoryID == socialhisID && x.isApproved == 1 && x.isDeleted != 1)).ToList();


                for (int j = 0; j < likes.Count(); j++)
                {
                    int likeItemID = Convert.ToInt32(likes[j].likeItemID);
                    string likeItem = _context.ListLikes.SingleOrDefault(x => x.list_likeID == likeItemID && x.isDeleted != 1).value;
                    jarrayLikes.Add(likeItem);
                }
                for (int k = 0; k < dislikes.Count(); k++)
                {
                    int dislikeItemID = Convert.ToInt32(dislikes[k].dislikeItemID);

                    string dislikeItem = _context.ListDislikes.SingleOrDefault(x => x.list_dislikeID == dislikeItemID && x.isDeleted != 1).value;
                    jarrayDislike.Add(dislikeItem);
                }

                o["Likes"] = jarrayLikes;
                o["Dislike"] = jarrayDislike;

            }

            var allergy = _context.Allergies.Where((x => x.patientAllocationID == patientAllocation.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            for (int l = 0; l < allergy.Count(); l++)
            {
                int allergyListID = Convert.ToInt32(allergy[l].allergyListID);
                string allergyName = _context.ListAllergy.SingleOrDefault(x => x.list_allergyID == allergyListID && x.isDeleted != 1).value;
                jarrayAllergy.Add(allergyName);
            }
            o["Allergy"] = jarrayAllergy;

            var dateAndTime = DateTime.Now;
            var date = dateAndTime.Date;
            var schedule = _context.Schedules.Where(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1 && x.dateStart == date)).ToList();
            for (int m = 0; m < schedule.Count(); m++)
            {
                JObject b = new JObject();
                int? scheduleCentreActivityID = schedule[m].centreActivityID;
                //shortcutMethod.printf("" + scheduleCentreActivityID);
                var centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == scheduleCentreActivityID && x.isApproved == 1 && x.isDeleted != 1));
                //shortcutMethod.printf("Deciding");
                if (centreActivity == null)
                {
                    //shortcutMethod.printf("centreActivity is null");
                    int? scheduleRoutineID = schedule[m].routineID;
                    var routine = _context.Routines.SingleOrDefault(x => (x.routineID == scheduleRoutineID && x.isApproved == 1 && x.isDeleted != 1));
                    JArray tempSchedule = new JArray();
                    b["eventName"] = routine.eventName;
                    b["startTime"] = routine.startTime;
                    b["endTime"] = routine.endTime;
                }
                else
                {
                    //shortcutMethod.printf("centreActivity is not null");
                    shortcutMethod.printf(schedule[m].scheduleID + "timeStart" + schedule[m].timeStart + "timeEnd:" + schedule[m].timeEnd);
                    b["eventName"] = centreActivity.activityTitle;
                    b["startTime"] = schedule[m].timeStart;
                    b["endTime"] = schedule[m].timeEnd;
                    //shortcutMethod.printf(jarraySchedule.ToString());
                }
                jarraySchedule.Add(b);
            }
            o["Schedule"] = jarraySchedule;

            var albumPath = _context.AlbumPatient.SingleOrDefault(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1 && x.albumCatID == 1));
            if (albumPath != null)
                o["albumPath"] = albumPath.albumPath;
            else
                o["albumPath"] = jarrayAlbum;

            string output = JsonConvert.SerializeObject(o);
            string json = o.ToString(Newtonsoft.Json.Formatting.None);
            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(o);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

    }
}
