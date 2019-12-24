using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.App_Code;
using NTU_FYP_REBUILD_17.Dtos;
using NTU_FYP_REBUILD_17.Models;


namespace NTU_FYP_REBUILD_17.Controllers.Api
{
	public class PatientController : ApiController
	{
		private ApplicationDbContext _context;
		App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        private Controllers.Synchronization.PatientMethod patientMethod = new Controllers.Synchronization.PatientMethod();
        private Controllers.Synchronization.ScheduleMethod scheduler = new Controllers.Synchronization.ScheduleMethod();

        public PatientController()
		{
			_context = new ApplicationDbContext();
		}

		//GET /api/patient
		[HttpGet]
		[Route("api/Patient/GetPatients_XML")]
		public IHttpActionResult GetPatients_XML(string token)
		{

			var dateTime = System.DateTime.Now.AddHours(-1);

			if (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime && (_context.UserTables.Where(c => c.token == token).Count() > 0))
			{
				return Ok(_context.PatientAllocations.Where(x => (x.isDeleted == 0 && x.isApproved == 1)).Include(b => b.Patient).Where(b => (b.Patient.isApproved == 1 && b.isDeleted == 0)).ToList().Select(Mapper.Map<PatientAllocation, PatientAllocationDto>));
			}
			else
			{
				return BadRequest("Invalid Token");
			}

		}

        // Note:    This function will be replaced by the function called "GetPatients(string token)"
        //          It will be temporary placed here while front-end transit to the new API
        [HttpGet]
        [Route("api/Patient/GetPatients_JSON")]
        public JsonResult<IEnumerable<PatientAllocationDto>> GetPatients_JSON(string token)
        {
            var dateTime = System.DateTime.Now.AddHours(-1);

            if (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime &&
                (_context.UserTables.Where(c => c.token == token).Count() > 0))
            {
                return Json(_context.PatientAllocations.Where(x => (x.isDeleted == 0 && x.isApproved == 1)).Include(b => b.Patient).ToList().Select(Mapper.Map<PatientAllocation, PatientAllocationDto>));
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        [Route("api/Patient/GetPatients")]
        public HttpResponseMessage GetPatients(string token)
        {

            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            JArray overallJArray = new JArray();

            //var patientAllocationList = _context.PatientAllocations.Where(x => (x.caregiverID == userID && x.isApproved == 1 && x.isDeleted == 0)).ToList();

            var patientAllocationList = (from pa in _context.PatientAllocations
                                         join p in _context.Patients on pa.patientID equals p.patientID
                                         where pa.isApproved == 1 && pa.isDeleted != 1
                                         where p.isApproved == 1 && p.isDeleted != 1
                                         where p.isActive == 1
                                         where p.endDate > DateTime.Today || p.endDate == null
                                         orderby p.firstName ascending
                                         select pa).ToList();

            foreach (var curPatient in patientAllocationList)
            {
                JArray jarrayAlbum = new JArray();
                JArray jarraySchedule = new JArray();

                JObject patientJObj = new JObject();

                var viewPatient = _context.Patients.SingleOrDefault(x => x.patientID == curPatient.patientID);
                var patient = _context.Patients.SingleOrDefault((x => (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted == 0)));
                if (patient == null)
                    return null;

                patientJObj["patientID"] = patient.patientID;
                patientJObj["firstName"] = patient.firstName;
                patientJObj["lastName"] = patient.lastName;
                patientJObj["preferredName"] = patient.preferredName;
                patientJObj["NRIC"] = patient.nric.Remove(1, 4).Insert(1, "xxxx");
                
                //var patientAllocation = _context.PatientAllocations.SingleOrDefault( x => x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted == 0)
                var albumPath = (from pa in _context.PatientAllocations
                                 join p in _context.Patients on pa.patientID equals p.patientID
                                 join a in _context.AlbumPatient on pa.patientAllocationID equals a.patientAllocationID
                                 where a.albumCatID == 1
                                 where pa.isApproved == 1 && pa.isDeleted == 0
                                 where p.isApproved == 1 && p.isDeleted == 0
                                 where a.isApproved == 1 && a.isDeleted == 0
                                 where pa.patientID == viewPatient.patientID
                                 select a).SingleOrDefault();
                //var albumPath = _context.Albums.SingleOrDefault(x => (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted == 0));
                if (albumPath != null)
                    patientJObj["albumPath"] = albumPath.albumPath;
                else
                    patientJObj["albumPath"] = jarrayAlbum;

                overallJArray.Add(patientJObj);
            }

            string output = JsonConvert.SerializeObject(overallJArray);
            string json = overallJArray.ToString(Newtonsoft.Json.Formatting.None);
            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(overallJArray);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }




        //GET /Api/patient/1
        public IHttpActionResult GetPatient(int id)
		{
			var patient = _context.Patients.Where(x => (x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault(p => p.patientID == id);

			if (patient == null)
			{
				return NotFound();
			}

			return Ok(Mapper.Map<Patient, PatientDto>(patient));
        }

        //POST /Api/addPatient
        [HttpPost]
        [Route("api/PatientController/addMedHistory")]
        public string addMedHist(HttpRequestMessage bodyResult)
        {


            string jsonString = bodyResult.Content.ReadAsStringAsync().Result;
            JObject logjObject = JObject.Parse(jsonString);

            string token = (string)logjObject.SelectToken("token");
            ApplicationUser user = shortcutMethod.getUserDetails(token, null);

            JArray jArray = (JArray)logjObject.SelectToken("medHist");

            foreach (var item in jArray)
            {

                string infoSource = (string)item["informationSource"];
                string medDetails = (string)item["medicalDetails"];
                string medNotes = (string)item["medNotes"];
                string medDate = (string)item["medEstimatedDate"];
                DateTime medEstimatedDate = DateTime.ParseExact(medDate, "dd/mm/yyyy", null);
            }


            return "0";
        }



    //POST /Api/addPatient
    [HttpPost]
        [Route("api/Patient/addPatient")]
        public string addPatient(HttpRequestMessage bodyResult)
        {
            string jsonString = bodyResult.Content.ReadAsStringAsync().Result;
            JObject logjObject = JObject.Parse(jsonString);
            
            string token = (string)logjObject.SelectToken("token");

            ApplicationUser user = shortcutMethod.getUserDetails(token, null);

            if (user.Equals("Guardian") || user.Equals("NONE"))
                return null;

            //Guardian Information
            string guardianName = logjObject.SelectToken("guardianName").ToString();
            string guardianNRIC = logjObject.SelectToken("guardianNRIC").ToString();
            int guardianRelationshipID = (int)logjObject.SelectToken("guardianRelationshipID");
            //int guardianRelationshipID = Convert.ToInt32(guardianRSID);
            string guardianRelationName = logjObject.SelectToken("guardianRelationName").ToString();
            string guardianContactNo = logjObject.SelectToken("guardianContactNo").ToString();
            string guardianEmail= logjObject.SelectToken("guardianEmail").ToString();


            //secGuardian
            string guardianName2 = logjObject.SelectToken("guardianName2").ToString();
            string guardianNRIC2 = logjObject.SelectToken("guardianNRIC2").ToString();
            int guardian2RelationshipID = (int)logjObject.SelectToken("guardian2RelationshipID");
            //int guardian2RelationshipID = Convert.ToInt32(guardian2RSID);
            string guardianRelationName2 = logjObject.SelectToken("guardianRelationName2").ToString();
            string guardianContactNo2 = logjObject.SelectToken("guardianContactNo2").ToString();
            string guardianEmail2 = logjObject.SelectToken("guardianEmail2").ToString();


            int patientGuardianID = patientMethod.addPatientGuardian(user.userID, guardianName, guardianNRIC, guardianRelationshipID, guardianRelationName,
                                  guardianContactNo, guardianEmail, guardianName2, guardianNRIC2, guardian2RelationshipID,
                                  guardianRelationName2, guardianContactNo2, guardianEmail2, 1);


            if (patientGuardianID != -1)
            {

                //Patient Info
                string nric = logjObject.SelectToken("nric").ToString();
                string firstName = logjObject.SelectToken("firstName").ToString();
                string lastName = logjObject.SelectToken("lastName").ToString();
                string preferredName = logjObject.SelectToken("preferredName").ToString();
                string handphoneNo = logjObject.SelectToken("handphoneNo").ToString();
                string homeNo = logjObject.SelectToken("homeNo").ToString();

                string preferredLanguageIDstring = logjObject.SelectToken("preferredLanguageID").ToString();
                int preferredLanguageID = Convert.ToInt32(preferredLanguageIDstring);
                string languageName = logjObject.SelectToken("languageName").ToString();
                string address = logjObject.SelectToken("address").ToString();
                string tempAddress = logjObject.SelectToken("tempAddress").ToString();
                string gender = logjObject.SelectToken("gender").ToString();
                string DOBstring = logjObject.SelectToken("DOB").ToString();
                DateTime DOB = DateTime.ParseExact(DOBstring, "dd/MM/yyyy", null);

                string startDatestring = logjObject.SelectToken("startDate").ToString();
                DateTime startDate = DateTime.ParseExact(startDatestring, "dd/MM/yyyy", null);

                string endDatestring = logjObject.SelectToken("endDate").ToString();
                DateTime endDate = DateTime.ParseExact(endDatestring, "dd/MM/yyyy", null);

                int isRespiteCare = (int)logjObject.SelectToken("isRespiteCare");
                //int isRespiteCare = Convert.ToInt32(isRespiteCarestring);

                int patientID = patientMethod.addPatient(user.userID, patientGuardianID, nric, firstName, lastName, preferredName,handphoneNo,homeNo, preferredLanguageID, languageName,
                    address, tempAddress, gender, DOB, startDate, endDate, isRespiteCare, 1);

                //Patient Allocation Info
                if (patientID != -1)
                {
                    int assignedDoctor = (int)logjObject.SelectToken("assignedDoctor");
                    //int assignedDoctor = Convert.ToInt32(assignedDoctorstring);
                    int assignedCaregiver = (int)logjObject.SelectToken("assignedCaregiver");
                    //int assignedCaregiver = Convert.ToInt32(assignedCaregiverstring);
                    int assignedGametherapist = (int)logjObject.SelectToken("assignedGametherapist");
                    //int assignedGametherapist = Convert.ToInt32(assignedGametherapiststring);
                    int supervisorID= (int)logjObject.SelectToken("supervisorID");
                    //int supervisorID = Convert.ToInt32(supervisorIDstring);

                    int patientAllocationID = patientMethod.addPatientAllocation(user.userID, patientID, assignedDoctor, assignedCaregiver, assignedGametherapist, supervisorID, 1);

                    //Album Patient
                    if (patientAllocationID != -1)
                    {
                        patientMethod.addDefaultProfileImage(supervisorID, patientAllocationID, 1);

                        //Dementia Condition
                        int dementiaID = (int)logjObject.SelectToken("dementiaID");
                        //int dementiaID = Convert.ToInt32(dementiaIDstring);
                        patientMethod.addPatientAssignedDementia(supervisorID, patientAllocationID, dementiaID, 1);


                        //Medical History  //Multiple

                        JArray jArray = (JArray)logjObject.SelectToken("medHistory");

                        foreach (var item in jArray)
                        {

                            string infoSource = (string)item["informationSource"];
                            string medDetails = (string)item["medicalDetails"];
                            string medNotes = (string)item["medNotes"];
                            string medDate = (string)item["medEstimatedDate"];
                            DateTime medicalEstimatedDate = DateTime.ParseExact(medDate, "dd/MM/yyyy", null);
                        
                        patientMethod.addMedicalHistory(supervisorID, patientAllocationID, infoSource, medDetails, medNotes, medicalEstimatedDate, 1);
                        }


                        //Mobility
                        string mobilityListstring = logjObject.SelectToken("mobilityListID").ToString();
                        int mobilityListID = Convert.ToInt32(mobilityListstring);
                        string mobilityName = logjObject.SelectToken("mobilityName").ToString();

                        patientMethod.addMobility(supervisorID, patientAllocationID, mobilityListID, mobilityName, 1);


                        //Social History 
                        int alcoholUse = (int)logjObject.SelectToken("alcoholUse");
                        int caffeineUse = (int)logjObject.SelectToken("caffeineUse");
                        int drugUse = (int)logjObject.SelectToken("drugUse");
                        int exercise = (int)logjObject.SelectToken("exercise");
                        int retired = (int)logjObject.SelectToken("retired");
                        int tobaccoUse = (int)logjObject.SelectToken("tobaccoUse");
                        int secondhandSmoker = (int)logjObject.SelectToken("secondhandSmoker");
                        int sexuallyActive = (int)logjObject.SelectToken("sexuallyActive");
                        int occupationID = (int)logjObject.SelectToken("occupationID");
                        string occupationName = logjObject.SelectToken("occupationName").ToString();

                        int dietListID = (int)logjObject.SelectToken("dietListID");
                        string dietName = logjObject.SelectToken("dietName").ToString();

                        int educationListID = (int)logjObject.SelectToken("educationListID");
                        string educationName = logjObject.SelectToken("educationName").ToString();

                        int liveWithListID = (int)logjObject.SelectToken("liveWithListID");
                        string liveWithName = logjObject.SelectToken("liveWithName").ToString();

                        int petListID = (int)logjObject.SelectToken("petListID");
                        string petName = logjObject.SelectToken("petName").ToString();

                        int religionListID = (int)logjObject.SelectToken("religionListID");
                        string religionName = logjObject.SelectToken("religionName").ToString();


                        int socialHistoryID = patientMethod.addSocialHistory(supervisorID, patientAllocationID, alcoholUse, caffeineUse, drugUse, exercise, retired, tobaccoUse, secondhandSmoker, sexuallyActive, dietListID, dietName, educationListID, educationName, liveWithListID, liveWithName, petListID, petName, religionListID, religionName, occupationID, occupationName, 1);

                        //Privacy Settings
                        patientMethod.addDefaultPrivacySettings(socialHistoryID, patientAllocationID, supervisorID, 1);

                        //Allergy 
                        JArray allergyjArray = (JArray)logjObject.SelectToken("allergy");

                        foreach (var item in allergyjArray)
                        {

                            int allergyListID = (int)item["allergyListID"];
                            string allergyName = (string)item["allergyName"];
                            string allergyReaction = (string)item["allergyReaction"];
                            string allergyNotes = (string)item["allergyNotes"];

                            patientMethod.addAllergy(supervisorID, patientAllocationID, allergyListID, allergyName, allergyReaction, allergyNotes, 1);
                        }
                        
                        /*
                        int allergyListID = (int)logjObject.SelectToken("allergyListID");
                        string allergyName = logjObject.SelectToken("allergyName").ToString();
                        string allergyReaction = logjObject.SelectToken("allergyReaction").ToString();
                        string allergyNotes = logjObject.SelectToken("allergyNotes").ToString();

                        patientMethod.addAllergy(supervisorID, patientAllocationID, allergyListID, allergyName, allergyReaction, allergyNotes, 1);
                        */

                        //CentreActivity
                        patientMethod.addDefaultActivityPreferences(supervisorID, patientAllocationID, 1);

                        scheduler.generateWeeklySchedule(false, false);
                        return "1";

                    }
                }


                }


                return "0";
        }



        //POST /Api/nricValidation
        [HttpPost]
        [Route("api/Patient/nricValidation")]
        public string nricValidation(HttpRequestMessage bodyResult)
        {
            string jsonString = bodyResult.Content.ReadAsStringAsync().Result;
            JObject logjObject = JObject.Parse(jsonString);
            string nric = logjObject.SelectToken("nric").ToString();


            if (shortcutMethod.checkNric(nric))
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }


        //PUT /Api/Patient/1
        [HttpPut]
		public IHttpActionResult UpdatePatient(int id, PatientDto patientDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var patientInDb = _context.Patients.SingleOrDefault(p => p.patientID == id);

			if (patientInDb == null)
			{
				return NotFound();
			}

			Mapper.Map(patientDto, patientInDb);

			return Ok(_context.SaveChanges());
		}

		//DELETE /Api/Patients/1
		[HttpDelete]
		public IHttpActionResult DeleteCustomer(int id)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var patientInDb = _context.Patients.SingleOrDefault(p => p.patientID == id);

			if (patientInDb == null)
			{
				return NotFound();
			}

			_context.Patients.Remove(patientInDb);
			return Ok(_context.SaveChanges());
		}


        [HttpGet]
        [Route("api/Patient/patientPersonalInfo")]
        public HttpResponseMessage patientPersonalInfo(string token, int patientID, int masked)
        {
            // addLogToDB();

            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;
            ApplicationUser user = shortcutMethod.getUserDetails(token, null);

            Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            PatientAllocation pa = _context.PatientAllocations.SingleOrDefault(x => x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1);
            if (patient == null)
                return null;
            JObject jobjSend = new JObject();
            JObject jobjPatientInfo = new JObject();
            jobjPatientInfo["FirstName"] = patient.firstName;
            jobjPatientInfo["LastName"] = patient.lastName;
            if (masked == 1)
                jobjPatientInfo["NRIC"] = patient.nric.Remove(1, 4).Insert(1, "xxxx");
            else
            {
                jobjPatientInfo["NRIC"] = patient.nric;

            }

            jobjPatientInfo["Gender"] = patient.gender;
            jobjPatientInfo["DOB"] = patient.DOB;
            var today = DateTime.Today;
            var age = today.Year - Int32.Parse(DateTime.Now.Date.ToString("yyyy"));
            age = today.Year - patient.DOB.Year;
            if (patient.DOB > today.AddYears(-age)) age--;
            shortcutMethod.printf("patient.DOB=" + patient.DOB + "age=" + age + " today.Year=" + today.Year + " DateTime.Now.Date(YYYY)=" + Int32.Parse(DateTime.Now.Date.ToString("yyyy")));
            jobjPatientInfo["Age"] = age;
            jobjPatientInfo["HomeNO"] = patient.homeNo;
            jobjPatientInfo["HandphoneNo"] = patient.handphoneNo;
            jobjPatientInfo["Address"] = patient.address;
            jobjPatientInfo["TempAddress"] = patient.tempAddress;
            jobjPatientInfo["PreferredName"] = patient.preferredName;
            jobjPatientInfo["PreferredLanguage"] = _context.ListLanguages.Where(x => x.list_languageID == patient.preferredLanguageID && x.isDeleted != 1).SingleOrDefault().value;
            jobjPatientInfo["MainGuardianName"] = patient.PatientGuardian.guardianName;
            jobjPatientInfo["MainGuardianNo"] = patient.PatientGuardian.guardianContactNo;
            jobjPatientInfo["MainGuardianEmail"] = patient.PatientGuardian.guardianEmail;
            jobjPatientInfo["MainGuardianRelationship"] = _context.ListRelationships.SingleOrDefault(x => x.list_relationshipID == patient.PatientGuardian.guardianRelationshipID).value;
            jobjPatientInfo["SecGuardianName"] = patient.PatientGuardian.guardianName2;
            jobjPatientInfo["SecGuardianNo"] = patient.PatientGuardian.guardianContactNo2;
            jobjPatientInfo["SecGuardianEmail"] = patient.PatientGuardian.guardianEmail2;
            jobjPatientInfo["SecGuardianRelationship"] = _context.ListRelationships.SingleOrDefault(x => x.list_relationshipID == patient.PatientGuardian.guardian2RelationshipID).value;
                //jobjPatientInfo["OtherLanguage"] = "TBC";

            //var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            var album = _context.AlbumPatient.SingleOrDefault(x => (x.patientAllocationID == pa.patientAllocationID && x.albumCatID == 1 && x.isApproved == 1 && x.isDeleted != 1));
            if (album != null)
                jobjPatientInfo["albumPath"] = album.albumPath;
            else if (patient.gender.Equals("M"))
            {
                jobjPatientInfo["albumPath"] = "https://pear.fyp2017.com/Image/UsersAvatar/boy.png";
            }
            else if (patient.gender.Equals("F"))
            {
                jobjPatientInfo["albumPath"] = "https://pear.fyp2017.com/Image/UsersAvatar/girl.png";
            }
            else
                jobjPatientInfo["albumPath"] = "";

            //Patient DoctorNote
            JArray jobjPatientDoctorNoteInfo = new JArray();
            var doctorNote = _context.DoctorNotes.Where(x => (x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            for (int i = 0; i < doctorNote.Count(); i++)
            {
                JObject jobjDoctorNote = new JObject();
                jobjDoctorNote["doctorNote"] = doctorNote[i].note;
                jobjPatientDoctorNoteInfo.Add(jobjDoctorNote);
            }

            JArray jobjPatientSocialHistoryInfo = new JArray();
            JObject jobjSocialHistory = new JObject();
            var socialhistory = _context.SocialHistories.Where(x => (x.patientAllocationID == pa.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).SingleOrDefault();

            int dietID = Convert.ToInt32(socialhistory.dietID);
            int religionID = Convert.ToInt32(socialhistory.religionID);


            jobjSocialHistory["socialHistoryID"] = socialhistory.socialHistoryID;
            jobjSocialHistory["SexuallyActive"] = socialhistory.sexuallyActive;
            jobjSocialHistory["SecondhandSmoker"] = socialhistory.secondhandSmoker;
            jobjSocialHistory["AlcoholUse"] = socialhistory.alcoholUse;
            jobjSocialHistory["CaffeineUse"] = socialhistory.caffeineUse;
            jobjSocialHistory["TobaccoUse"] = socialhistory.tobaccoUse;
            jobjSocialHistory["DrugUse"] = socialhistory.drugUse;
            jobjSocialHistory["Exercise"] = socialhistory.exercise;
            jobjSocialHistory["Retired"] = socialhistory.retired;
            jobjSocialHistory["Pet"] = socialhistory.petID;
            jobjSocialHistory["Occupation"] = socialhistory.occupationID;
            jobjSocialHistory["Education"] = socialhistory.educationID;
            jobjSocialHistory["LiveWith"] = socialhistory.liveWithID;
            jobjSocialHistory["Diet"] = socialhistory.dietID;
            jobjSocialHistory["Religion"] = socialhistory.religionID;

            jobjPatientSocialHistoryInfo.Add(jobjSocialHistory);

            jobjSend["Patient"] = jobjPatientInfo;
            jobjSend["DoctorNote"] = jobjPatientDoctorNoteInfo;
            jobjSend["SocialHistory"] = jobjPatientSocialHistoryInfo;
            string output = JsonConvert.SerializeObject(jobjSend);
            string json = jobjSend.ToString(Newtonsoft.Json.Formatting.None);
            shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(jobjSend);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");







            return response;
        }
        [HttpGet]
        [Route("api/Patient/patientPreferenceInfo")]
        public HttpResponseMessage patientPreferenceInfo(string token, int patientID, int masked)
        {
            // addLogToDB();

            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;
            Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted == 0));
            if (patient == null)
                return null;
            JObject jobjSend = new JObject();
            JObject jobjPatientInfo = new JObject();
            jobjPatientInfo["FirstName"] = patient.firstName;
            jobjPatientInfo["LastName"] = patient.lastName;
            if (masked == 1)
                jobjPatientInfo["NRIC"] = patient.nric.Remove(1, 4).Insert(1, "xxxx");
            else
            {
                jobjPatientInfo["NRIC"] = patient.nric;
                ApplicationUser user = shortcutMethod.getUserDetails(token, null);
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, "", "Requested for Full NRIC", 18, patientID, user.userID, user.userID, null, null,
                    null, "", "", "", "", 0, 0, 1, "");
            }
            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            var album = _context.AlbumPatient.SingleOrDefault(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.albumCatID == 1 && x.isApproved == 1 && x.isDeleted == 0));
            if (album != null)
                jobjPatientInfo["albumPath"] = album.albumPath;
            else if (patient.gender.Equals("M"))
            {
                jobjPatientInfo["albumPath"] = "https://pear.fyp2017.com/Image/UsersAvatar/boy.png";
            }
            else if (patient.gender.Equals("F"))
            {
                jobjPatientInfo["albumPath"] = "https://pear.fyp2017.com/Image/UsersAvatar/girl.png";
            }
            else
                jobjPatientInfo["albumPath"] = "";
            JArray jobjPatientSocialHistoryInfo = new JArray();
            var socialhistory = _context.SocialHistories.Where(x => (x.patientAllocationID == patientID && x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault();

            JObject jobjSocialHistory = new JObject();
            JArray jArrLikes = new JArray();
            JArray jArrDislikes = new JArray();
            JArray jArrHobby = new JArray();
            JArray jArrHabits = new JArray();
            JArray jArrLikesList = new JArray();
            JArray jArrDislikesList = new JArray();
            JArray jArrHobbyList = new JArray();
            JArray jArrHabitsList = new JArray();
            // jobjSocialHistory["SocialHistoryID"] = socialhistory.socialHistoryID;
            int socialId = socialhistory.socialHistoryID;
            var likes = _context.Likes.Where(x => (x.socialHistoryID == socialId && x.isApproved == 1 && x.isDeleted == 0)).ToList();
            if (likes == null)
                jobjSend["Likes"] = "";
            else
            {
                for (int i = 0; i < likes.Count(); i++)
                {
                    JObject jobjLikes = new JObject();
                    jobjLikes["LikeID"] = likes[i].likeID;
                    jobjLikes["LikeDesc"] = _context.ListLikes.SingleOrDefault(x => (x.list_likeID == likes[i].likeItemID)).value;
                    jArrLikes.Add(jobjLikes);
                }
            }
            var dislikes = _context.Dislikes.Where(x => (x.socialHistoryID == socialId && x.isApproved == 1 && x.isDeleted == 0)).ToList();
            if (dislikes == null)
                jobjSend["Dislikes"] = "";
            else
            {
                for (int i = 0; i < dislikes.Count(); i++)
                {
                    JObject jobjDisLikes = new JObject();
                    jobjDisLikes["DislikeID"] = dislikes[i].dislikeID;
                    jobjDisLikes["DislikeDesc"] = _context.ListDislikes.SingleOrDefault(x => (x.list_dislikeID == dislikes[i].dislikeItemID)).value;
                    jArrDislikes.Add(jobjDisLikes);
                }
            }
            var hobby = _context.Hobbieses.Where(x => (x.socialHistoryID == socialId && x.isApproved == 1 && x.isDeleted == 0)).ToList();
            if (hobby == null)
                jobjSend["Hobby"] = "";
            else
            {
                for (int i = 0; i < hobby.Count(); i++)
                {
                    JObject jobjHobby = new JObject();
                    jobjHobby["HobbyID"] = hobby[i].hobbyID;
                    jobjHobby["HobbyDesc"] = _context.ListHobbies.SingleOrDefault(x => (x.list_hobbyID == hobby[i].hobbyListID)).value;
                    jArrHobby.Add(jobjHobby);
                }
            }
            var habit = _context.Habits.Where(x => (x.socialHistoryID == socialId && x.isApproved == 1 && x.isDeleted == 0)).ToList();
            if (habit == null)
                jobjSend["Habit"] = "";
            else
            {
                for (int i = 0; i < habit.Count(); i++)
                {
                    JObject jobjhabit = new JObject();
                    jobjhabit["HabitID"] = habit[i].habitID;
                    jobjhabit["Habit"] = _context.ListHabits.SingleOrDefault(x => (x.list_habitID == habit[i].habitListID)).value;
                    jArrHabits.Add(jobjhabit);
                }
            }

            //Retrive the list
            var likesList = _context.ListLikes.ToList();
            if (likesList == null)
                jobjSend["LikesList"] = "";
            else
            {
                for (int i = 0; i < likesList.Count(); i++)
                {
                    JObject jobjLikes = new JObject();
                    jobjLikes["LikeID"] = likesList[i].list_likeID;
                    jobjLikes["LikeDesc"] = likesList[i].value;
                    jArrLikesList.Add(jobjLikes);
                }
            }
            var dislikesList = _context.ListDislikes.ToList();
            if (dislikesList == null)
                jobjSend["DislikesList"] = "";
            else
            {
                for (int i = 0; i < dislikesList.Count(); i++)
                {
                    JObject jobjDisLikes = new JObject();
                    jobjDisLikes["DislikeID"] = dislikesList[i].list_dislikeID;
                    jobjDisLikes["DislikeDesc"] = dislikesList[i].value;
                    jArrDislikesList.Add(jobjDisLikes);
                }
            }
            var hobbyList = _context.ListHobbies.ToList();
            if (hobbyList == null)
                jobjSend["HobbyList"] = "";
            else
            {
                for (int i = 0; i < hobbyList.Count(); i++)
                {
                    JObject jobjHobby = new JObject();
                    jobjHobby["HobbyID"] = hobbyList[i].list_hobbyID;
                    jobjHobby["HobbyDesc"] = hobbyList[i].value;
                    jArrHobbyList.Add(jobjHobby);
                }
            }
            var habitList = _context.ListHabits.ToList();
            if (habit == null)
                jobjSend["HabitList"] = "";
            else
            {
                for (int i = 0; i < habitList.Count(); i++)
                {
                    JObject jobjhabit = new JObject();
                    jobjhabit["HabitID"] = habitList[i].list_habitID;
                    jobjhabit["Habit"] = habitList[i].value;
                    jArrHabitsList.Add(jobjhabit);
                }
            }


            jobjSend["patients"] = jobjPatientInfo;
            jobjSend["Likes"] = jArrLikes;
            jobjSend["Dislikes"] = jArrDislikes;
            jobjSend["Hobby"] = jArrHobby;
            jobjSend["Habit"] = jArrHabits;
            jobjSend["LikesList"] = jArrLikesList;
            jobjSend["DislikesList"] = jArrDislikesList;
            jobjSend["HobbyList"] = jArrHobbyList;
            jobjSend["HabitList"] = jArrHabitsList;

            string output = JsonConvert.SerializeObject(jobjSend);
            string json = jobjSend.ToString(Newtonsoft.Json.Formatting.None);
            shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(jobjSend);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");



            return response;
        }

        [HttpPut]
        [Route("api/Patient/updateGeneralInfo")]
        public string updateGeneralInfo(HttpRequestMessage bodyResult)
        {
            string generalInfo = bodyResult.Content.ReadAsStringAsync().Result;
            string oldLogData = "";
            string logData = ""; // Longer details
            string columns = " ";

            JObject jsonGeneralInfo = JObject.Parse(generalInfo);
            string token = (string)jsonGeneralInfo.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);
            //   shortcutMethod.printf("updateVital_String");
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;
            // shortcutMethod.printf("UserType not guardian/Invalid user");
            int SocialHistoryID = (int)jsonGeneralInfo.SelectToken("SocialHistoryID");
            var socialhistory = _context.SocialHistories.Where(x => (x.socialHistoryID == SocialHistoryID && x.isApproved == 1 && x.isDeleted == 0)).FirstOrDefault();
            SocialHistory oldSocialHistory = new SocialHistory();
            oldSocialHistory = socialhistory;
            if (socialhistory == null)
                return null;
            string s1 = new JavaScriptSerializer().Serialize(socialhistory);



            //Update new row
            string diet = (String)jsonGeneralInfo.SelectToken("Diet");
            int dietID = _context.ListDiets.SingleOrDefault(x => (x.value == diet)).list_dietID;
            if (!dietID.Equals(oldSocialHistory.dietID))
            {

                oldLogData = oldLogData + " Diet:" + oldSocialHistory.dietID;
                logData = logData + " Diet:" + diet;
                columns = columns + " Diet";
                socialhistory.dietID = dietID;
            }
            //  jsonnewlog.Add("Diet", diet);


            string religion = (String)jsonGeneralInfo.SelectToken("Religion");
            int religionID = _context.ListReligions.SingleOrDefault(x => (x.value == religion)).list_religionID;
            if (!religionID.Equals(oldSocialHistory.religionID))
            {
                oldLogData = oldLogData + " Religion:" + oldSocialHistory.religionID;
                logData = logData + " Religion:" + religion;
                columns = columns + " Religion";
                socialhistory.religionID = religionID;
            }
            int sexuallyActive = (int)jsonGeneralInfo.SelectToken("SexuallyActive");

            if (sexuallyActive != oldSocialHistory.sexuallyActive)
            {
                oldLogData = oldLogData + " SexuallyActive:" + oldSocialHistory.sexuallyActive;
                logData = logData + " SexuallyActive:" + sexuallyActive;
                columns = columns + " SexuallyActive";
                socialhistory.sexuallyActive = sexuallyActive;
            }
            int alcoholUse = (int)jsonGeneralInfo.SelectToken("AlcoholUse");
            if (alcoholUse != oldSocialHistory.alcoholUse)
            {
                oldLogData = oldLogData + " AlcoholUse:" + oldSocialHistory.alcoholUse;
                logData = logData + " AlcoholUse:" + alcoholUse;
                columns = columns + " AlcoholUse";
                socialhistory.alcoholUse = alcoholUse;
            }


            int caffeineUse = (int)jsonGeneralInfo.SelectToken("CaffeineUse");
            if (caffeineUse != oldSocialHistory.caffeineUse)
            {
                oldLogData = oldLogData + " CaffeineUse:" + oldSocialHistory.caffeineUse;
                logData = logData + " CaffeineUse:" + caffeineUse;
                columns = columns + " CaffeineUse";
                socialhistory.caffeineUse = caffeineUse;
            }
            int tobaccoUse = (int)jsonGeneralInfo.SelectToken("TobaccoUse");
            if (tobaccoUse != oldSocialHistory.tobaccoUse)
            {
                oldLogData = oldLogData + " TobaccoUse:" + oldSocialHistory.tobaccoUse;
                logData = logData + " TobaccoUse:" + tobaccoUse;
                columns = columns + " TobaccoUse";
                socialhistory.tobaccoUse = tobaccoUse;
            }
            int drugUse = (int)jsonGeneralInfo.SelectToken("DrugUse");
            if (drugUse != oldSocialHistory.drugUse)
            {
                oldLogData = oldLogData + " DrugUse:" + oldSocialHistory.drugUse;
                logData = logData + " DrugUse:" + drugUse;
                columns = columns + " DrugUse";
                socialhistory.drugUse = drugUse;
            }

            string pet = (String)jsonGeneralInfo.SelectToken("Pet");
            int petID = _context.ListPets.SingleOrDefault(x => (x.value == pet)).list_petID;
            if (!petID.Equals(oldSocialHistory.petID))
            {
                oldLogData = oldLogData + " Pet:" + oldSocialHistory.petID;
                logData = logData + " Pet:" + pet;
                columns = columns + " Pet";
                socialhistory.petID = petID;
            }

            string occupation = (String)jsonGeneralInfo.SelectToken("Occupation");
            int occupationID = _context.ListOccupations.SingleOrDefault(x => (x.value == occupation)).list_occupationID;
            if (!occupationID.Equals(oldSocialHistory.occupationID))
            {
                oldLogData = oldLogData + " Occupation:" + oldSocialHistory.occupationID;
                logData = logData + " Occupation:" + occupation;
                columns = columns + " Occupation";
                socialhistory.occupationID = occupationID;
            }
            string education = (String)jsonGeneralInfo.SelectToken("Education");
            int educationID = _context.ListEducations.SingleOrDefault(x => (x.value == education)).list_educationID;
            if (!educationID.Equals(oldSocialHistory.educationID))
            {
                oldLogData = oldLogData + " Education:" + oldSocialHistory.educationID;
                logData = logData + " Education:" + education;
                columns = columns + " Education";
                socialhistory.educationID = educationID;
            }
            int exercise = (int)jsonGeneralInfo.SelectToken("Exercise");
            if (exercise != oldSocialHistory.exercise)
            {
                oldLogData = oldLogData + " Exercise:" + oldSocialHistory.exercise;
                logData = logData + " Exercise:" + exercise;
                columns = columns + " Exercise";
                socialhistory.exercise = exercise;
            }

            string liveWith = (String)jsonGeneralInfo.SelectToken("LiveWith");
            int liveWithID = _context.ListLiveWiths.SingleOrDefault(x => (x.value == liveWith)).list_liveWithID;
            if (!liveWithID.Equals(oldSocialHistory.liveWithID))
            {
                oldLogData = oldLogData + " LiveWith:" + oldSocialHistory.liveWithID;
                logData = logData + " LiveWith:" + liveWith;
                columns = columns + " LiveWith";
                socialhistory.liveWithID = liveWithID;
            }

            string s2 = new JavaScriptSerializer().Serialize(socialhistory);
            shortcutMethod.printf(s1 + "\n" + s2);

            var differences = oldSocialHistory.CompareObj(socialhistory);

            JObject jsonnewlog = new JObject();
            JObject jsonoldlog = new JObject();

            for (int i = 0; i < differences.Count(); i++)
            {
                string typeA = differences[i].valA.GetType().ToString();
                string typeB = differences[i].valB.GetType().ToString();
                if (typeA.Contains("Int") || typeB.Contains("Int"))
                {
                    jsonoldlog.Add(differences[i].PropertyName, Int32.Parse(differences[i].valA.ToString()));
                    jsonnewlog.Add(differences[i].PropertyName, Int32.Parse(differences[i].valB.ToString()));
                }
                else
                {
                    jsonoldlog.Add(differences[i].PropertyName, differences[i].valA.ToString());
                    jsonnewlog.Add(differences[i].PropertyName, differences[i].valB.ToString());
                }
            }




            int approved = 0;
            int supervisornotified = 1;
            int logCategoryID_BasedOnTypeDeleted = 5;
            String logDesc_BasedOnTypeDeleted = "Update" + columns + " info for patient";
            string logDesc = logDesc_BasedOnTypeDeleted; // Short details
            int logCategoryID = logCategoryID_BasedOnTypeDeleted; // choose categoryID
                                                                  // int patientID = Vital.patientID;
                                                                  //////
            int userIDInit = shortcutMethod.getUserDetails(token, null).userID;
            // int userIDApproved = shortcutMethod.getUserDetails(token, null).userID;
            int userIDApproved = 3;
            string additionalInfo = null; // None just null
            string remarks = null; // none just null
            string tableAffected = "SocialHistory"; //Which database table that got affected
            string columnAffected = columns; // Which database table column is affected
            int rowAffected = 1; // Not needed just put 0
            int supNotified = 0;  //Not needed just put 0
            int userNotified = 0;// Not needed just put 0

            int success = _context.SaveChanges();



            if (success > 0)
            {
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, SocialHistoryID, userIDInit, userIDApproved, null, additionalInfo,
    remarks, tableAffected, columnAffected, "", "", supNotified, userNotified, approved, "");
                return "Update Successfully. Row updated:" + success;
            }
            else
            {
                return "Update failed" + oldLogData + "///////////////" + logData;
            }



        }

        [HttpGet]
        [Route("api/Patient/GetLog")]
        public HttpResponseMessage GetLog()
        {

            ApplicationDbContext _context = new ApplicationDbContext(); ;

            JArray jarrayAlbum = new JArray();
            JObject o = new JObject();
            JObject albumArrayOj = new JObject();
            var log = _context.Logs.ToList();


            for (int i = 0; i < log.Count(); i++)
            {


                // o["catId"] = albumCat.albumCatID;
                // o["catName"] = albumPath[i].albumCatName;
                o["LogData"] = log[i].logData;
                o["LogDesc"] = log[i].logDesc;
                o["OldLogDesc"] = log[i].oldLogData;
                o["UserIDApproved"] = log[i].userIDApproved;
                // var albumCat = _context.AlbumCategories.SingleOrDefault(x => x.albumCatID == albumPath[i].albumCatID);


                jarrayAlbum.Add(o);
            }

            albumArrayOj["Album"] = jarrayAlbum;

            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(albumArrayOj);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
        //https://localhost:44300/api/Patient/PostProfileImage?patientid=4
        [Route("api/Patient/PostProfileImage")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> PostProfileImage()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                int patientId = 0;
                string oldLogData = "";
                string logData = ""; // Longer details
                string columns = " ";

                var httpRequest = HttpContext.Current.Request;
                var queryVals = Request.RequestUri.ParseQueryString();
                patientId = int.Parse(queryVals["patientid"]);


                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];


                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {

                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("Please Upload a file upto 1 mb.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {



                            var filePath = HttpContext.Current.Server.MapPath("https://pear.fyp2017.com/Image/UsersAvatar/" + postedFile.FileName);

                            postedFile.SaveAs(filePath);

                        }
                    }

                    var message1 = string.Format("Image Updated Successfully." + patientId);
                    var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientId && x.isApproved == 1 && x.isDeleted != 1));
                    var album = _context.AlbumPatient.SingleOrDefault(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.albumCatID == 1 && x.isApproved == 1 && x.isDeleted == 0));
                    if (album != null)
                    {
                        album.albumPath = "https://pear.fyp2017.com/Image/UsersAvatar/" + postedFile.FileName;
                    }
                    else
                    {
                        album.albumPath = "https://pear.fyp2017.com/Image/UsersAvatar/boy.png";
                    }
                    int success = _context.SaveChanges();
                    if (success > 0)
                    {
                        int approved = 0;
                        int supervisornotified = 1;
                        int logCategoryID_BasedOnTypeDeleted = 5;
                        String logDesc_BasedOnTypeDeleted = "upload photo Abum path" + "https://pear.fyp2017.com/Image/UsersAvatar/" + postedFile.FileName;
                        string logDesc = logDesc_BasedOnTypeDeleted; // Short details
                        int logCategoryID = logCategoryID_BasedOnTypeDeleted; // choose categoryID
                                                                              // int patientID = Vital.patientID;
                                                                              //////
                                                                              //  int userIDInit = shortcutMethod.getUserDetails(token, null).userID;
                        int userIDInit = 0;
                        // int userIDApproved = shortcutMethod.getUserDetails(token, null).userID;
                        int userIDApproved = 3;
                        string additionalInfo = null; // None just null
                        string remarks = null; // none just null
                        string tableAffected = "Album"; //Which database table that got affected
                        string columnAffected = columns; // Which database table column is affected
                        int rowAffected = 1; // Not needed just put 0
                        int supNotified = 0;  //Not needed just put 0
                        int userNotified = 0;// Not needed just put 0
                                             // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                        shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientId, userIDInit, userIDApproved, null, additionalInfo,
            remarks, tableAffected, columnAffected, "", "", supNotified, userNotified, approved, "");
                    }
                    return Request.CreateErrorResponse(HttpStatusCode.Created, message1); ;
                }
                var res = string.Format("Please Upload a image.");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception ex)
            {
                var res = string.Format(ex.Message);
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }
        //testing
        public void addLogToDB()
        {
            var now = DateTime.Now;
            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == 3 && x.isApproved == 1 && x.isDeleted != 1));
            var album = _context.AlbumPatient.SingleOrDefault(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.albumCatID == 7 && x.isApproved == 1 && x.isDeleted == 0));
            album.albumCatID = 1;
            _context.SaveChanges();
        }



        //Required token,patientID,note
        [HttpPut]
        [Route("api/Patient/updateDoctorNote")]
        public string updateDoctorNote(HttpRequestMessage bodyResult)
        {
            string doctorNoteJson = bodyResult.Content.ReadAsStringAsync().Result;
            string columns = "Notes ";
            string oldLogData = "";
            string logData = ""; // Longer details

            JObject jsonGeneralInfo = JObject.Parse(doctorNoteJson);
            string token = (string)jsonGeneralInfo.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);
            //   shortcutMethod.printf("updateVital_String");
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;
            // shortcutMethod.printf("UserType not guardian/Invalid user");
            int patientID = (int)jsonGeneralInfo.SelectToken("patientID");
            var doctorNote = _context.DoctorNotes.Where(x => (x.patientAllocationID == patientID && x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault();
            DoctorNote oldHistory = new DoctorNote();
            oldHistory = doctorNote;
            if (doctorNote == null)
                return null;
            string s1 = new JavaScriptSerializer().Serialize(doctorNote);


            //Update new row
            string note = (string)jsonGeneralInfo.SelectToken("note");
            if (!note.Equals(oldHistory.note))
            {
                oldLogData = oldLogData + " note:" + oldHistory.note;
                logData = logData + " note:" + note;
                columns = columns + " note";
                doctorNote.note = note;
            }
            string s2 = new JavaScriptSerializer().Serialize(doctorNote);
            shortcutMethod.printf(s1 + "\n" + s2);

            var differences = oldHistory.CompareObj(doctorNote);

            JObject jsonnewlog = new JObject();
            JObject jsonoldlog = new JObject();

            for (int i = 0; i < differences.Count(); i++)
            {
                string typeA = differences[i].valA.GetType().ToString();
                string typeB = differences[i].valB.GetType().ToString();
                if (typeA.Contains("Int") || typeB.Contains("Int"))
                {
                    jsonoldlog.Add(differences[i].PropertyName, Int32.Parse(differences[i].valA.ToString()));
                    jsonnewlog.Add(differences[i].PropertyName, Int32.Parse(differences[i].valB.ToString()));
                }
                else
                {
                    jsonoldlog.Add(differences[i].PropertyName, differences[i].valA.ToString());
                    jsonnewlog.Add(differences[i].PropertyName, differences[i].valB.ToString());
                }
            }




            int approved = 0;
            int supervisornotified = 1;
            int logCategoryID_BasedOnTypeDeleted = 5;
            String logDesc_BasedOnTypeDeleted = "Update doctor note info for patient";



            string logDesc = logDesc_BasedOnTypeDeleted; // Short details
            int logCategoryID = logCategoryID_BasedOnTypeDeleted; // choose categoryID
                                                                  // int patientID = Vital.patientID;
                                                                  //////
                                                                  //  int userIDInit = shortcutMethod.getUserDetails(token, null).userID;
            int userIDInit = shortcutMethod.getUserDetails(token, null).userID;
            int userIDApproved = shortcutMethod.getUserDetails(token, null).userID;
            string additionalInfo = null; // None just null
            string remarks = null; // none just null
            string tableAffected = "DoctorNotes"; //Which database table that got affected
            string columnAffected = columns; // Which database table column is affected
            int rowAffected = patientID; // Not needed just put 0
            int supNotified = supervisornotified;  //Not needed just put 0
            int userNotified = 1;// Not needed just put 0

            int success = _context.SaveChanges();



            if (success > 0)
            {
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientID, userIDInit, userIDApproved, null, additionalInfo,
                remarks, tableAffected, columnAffected, "", "", supNotified, userNotified, approved, "");
                return "Update Successfully. Row updated:" + success;
            }
            else
            {
                return "Update failed" + oldLogData + "///////////////" + logData;
            }



        }


        [HttpPut]
        [Route("api/Patient/updatePatientInfo")]
        public string updatePatientInfo(HttpRequestMessage bodyResult)
        {
            string patientJson = bodyResult.Content.ReadAsStringAsync().Result;
            string oldLogData = "";
            string logData = ""; // Longer details
            string columns = " ";
            JObject jsonGeneralInfo = JObject.Parse(patientJson);
            string token = (string)jsonGeneralInfo.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);
            //   shortcutMethod.printf("updateVital_String");
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;
            // shortcutMethod.printf("UserType not guardian/Invalid user");
            int patientID = (int)jsonGeneralInfo.SelectToken("patientID");
            Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted == 0));
            Patient oldHistory = new Patient();
            oldHistory = patient;
            if (patient == null)
                return null;
            string s1 = new JavaScriptSerializer().Serialize(patient);


            //Update new row
            string fName = (string)jsonGeneralInfo.SelectToken("FirstName");
            if (!fName.Equals(oldHistory.firstName))
            {
                oldLogData = oldLogData + " FirstName:" + oldHistory.firstName;
                logData = logData + " FirstName:" + fName;
                columns = columns + " FirstName";
                patient.firstName = fName;
            }
            string lName = (string)jsonGeneralInfo.SelectToken("LastName");
            if (!lName.Equals(oldHistory.lastName))
            {
                oldLogData = oldLogData + " LastName:" + oldHistory.lastName;
                logData = logData + " LastName:" + lName;
                columns = columns + " LastName";
                patient.lastName = lName;
            }
            string nric = (string)jsonGeneralInfo.SelectToken("NRIC");
            if (!nric.Equals(oldHistory.nric))
            {
                oldLogData = oldLogData + " NRIC:" + oldHistory.nric;
                logData = logData + " NRIC:" + nric;
                columns = columns + " NRIC";
                patient.nric = nric;
            }
            string address = (string)jsonGeneralInfo.SelectToken("Address");
            if (!nric.Equals(oldHistory.nric))
            {
                oldLogData = oldLogData + " NRIC:" + oldHistory.nric;
                logData = logData + " NRIC:" + nric;
                columns = columns + " NRIC";
                patient.address = address;
            }
            string hpNo = (string)jsonGeneralInfo.SelectToken("HandphoneNo");
            if (!hpNo.Equals(oldHistory.handphoneNo))
            {
                oldLogData = oldLogData + " HandphoneNo:" + oldHistory.handphoneNo;
                logData = logData + " HandphoneNo:" + hpNo;
                columns = columns + " HandphoneNo";
                patient.handphoneNo = hpNo;
            }
            string gender = (string)jsonGeneralInfo.SelectToken("Gender");
            if (!gender.Equals(oldHistory.gender))
            {
                oldLogData = oldLogData + " Gender:" + oldHistory.gender;
                logData = logData + " Gender:" + gender;
                columns = columns + " Gender";
                patient.gender = gender;
            }
            DateTime dob = (DateTime)jsonGeneralInfo.SelectToken("DOB");
            if (!dob.ToString().Equals(oldHistory.DOB.ToString()))
            {
                oldLogData = oldLogData + " DOB:" + oldHistory.DOB;
                logData = logData + " DOB:" + dob;
                columns = columns + " DOB";
                patient.DOB = dob;
            }
            string s2 = new JavaScriptSerializer().Serialize(patient);
            shortcutMethod.printf(s1 + "\n" + s2);

            var differences = oldHistory.CompareObj(patient);

            JObject jsonnewlog = new JObject();
            JObject jsonoldlog = new JObject();

            for (int i = 0; i < differences.Count(); i++)
            {
                string typeA = differences[i].valA.GetType().ToString();
                string typeB = differences[i].valB.GetType().ToString();
                if (typeA.Contains("Int") || typeB.Contains("Int"))
                {
                    jsonoldlog.Add(differences[i].PropertyName, Int32.Parse(differences[i].valA.ToString()));
                    jsonnewlog.Add(differences[i].PropertyName, Int32.Parse(differences[i].valB.ToString()));
                }
                else
                {
                    jsonoldlog.Add(differences[i].PropertyName, differences[i].valA.ToString());
                    jsonnewlog.Add(differences[i].PropertyName, differences[i].valB.ToString());
                }
            }





            int approved = 0;
            int supervisornotified = 1;
            int logCategoryID_BasedOnTypeDeleted = 5;
            String logDesc_BasedOnTypeDeleted = "Update" + columns + " info for patient"; ;



            string logDesc = logDesc_BasedOnTypeDeleted; // Short details
            int logCategoryID = logCategoryID_BasedOnTypeDeleted; // choose categoryID
                                                                  // int patientID = Vital.patientID;
                                                                  //////
                                                                  //  int userIDInit = shortcutMethod.getUserDetails(token, null).userID;
            int userIDInit = shortcutMethod.getUserDetails(token, null).userID;
            int userIDApproved = shortcutMethod.getUserDetails(token, null).userID;
            string additionalInfo = null; // None just null
            string remarks = null; // none just null
            string tableAffected = "patient"; //Which database table that got affected
            string columnAffected = columns; // Which database table column is affected
            int rowAffected = patientID; // Not needed just put 0
            int supNotified = supervisornotified;  //Not needed just put 0
            int userNotified = 1;// Not needed just put 0

            int success = _context.SaveChanges();



            if (success > 0)
            {
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientID, userIDInit, userIDApproved, null, additionalInfo,
                remarks, tableAffected, columnAffected, "", "", supNotified, userNotified, approved, "");
                return "Update Successfully. Row updated:" + success;
            }
            else
            {
                return "Update failed" + oldLogData + "///////////////" + logData;
            }
        }
        /* Select between update preference or guardian
        Required token,patientID,update
         if update Preference {preferredName,preferredLanguage}
         if update Guardian {guardianName,guardianContactNo}

              */
        [HttpPut]
        [Route("api/Patient/updatePreferenceOrGuardian")]
        public string updatePreferenceOrGuardian(HttpRequestMessage bodyResult)
        {
            string doctorNoteJson = bodyResult.Content.ReadAsStringAsync().Result;

            JObject jsonGeneralInfo = JObject.Parse(doctorNoteJson);
            string token = (string)jsonGeneralInfo.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);
            //   shortcutMethod.printf("updateVital_String");
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;
            // shortcutMethod.printf("UserType not guardian/Invalid user");
            int patientID = (int)jsonGeneralInfo.SelectToken("patientID");
            var personInfo = _context.Patients.Where(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault();
            Patient oldHistory = new Patient();
            oldHistory = personInfo;
            if (personInfo == null)
                return null;
            string s1 = new JavaScriptSerializer().Serialize(personInfo);

            String logDesc_BasedOnTypeDeleted = "";

            string oldLogData = "";
            string logData = "";// Longer details
            string columns = "";
            String update = (string)jsonGeneralInfo.SelectToken("update");
            //Update new row
            if (update.Equals("Preference"))
            {

                string pName = (string)jsonGeneralInfo.SelectToken("preferredName");
                if (!pName.Equals(oldHistory.preferredName))
                {
                    oldLogData = oldLogData + " preferredName:" + oldHistory.preferredName;
                    logData = logData + " preferredName:" + pName;
                    columns = columns + " preferredName";
                    personInfo.preferredName = pName;
                }
                string planguage = (string)jsonGeneralInfo.SelectToken("preferredLanguage");
                /*
                if (!planguage.Equals(oldHistory.preferredLanguage))
                {
                    oldLogData = oldLogData + " preferredLanguage:" + oldHistory.preferredLanguage;
                    logData = logData + " preferredLanguage:" + planguage;
                    columns = columns + " preferredLanguage";
                    personInfo.preferredLanguage = planguage;
                }*/

                logDesc_BasedOnTypeDeleted = "Update preference name and language info for patient";
            }
            else
            {/*
                string gName = (string)jsonGeneralInfo.SelectToken("guardianName");
                if (!gName.Equals(oldHistory.guardianName))
                {
                    oldLogData = oldLogData + " guardianName:" + oldHistory.guardianName;
                    logData = logData + " guardianName:" + gName;
                    columns = columns + " guardianName";
                    personInfo.guardianName = gName;
                }
                string gContact = (string)jsonGeneralInfo.SelectToken("guardianContactNo");
                if (!gContact.Equals(oldHistory.guardianContactNo))
                {
                    oldLogData = oldLogData + " guardianContactNo:" + oldHistory.guardianContactNo;
                    logData = logData + " guardianContactNo:" + gContact;
                    columns = columns + " guardianContactNo";
                    personInfo.guardianContactNo = gContact;
                }*/

                logDesc_BasedOnTypeDeleted = "Update" + columns + " info for patient"; ;
            }
            string s2 = new JavaScriptSerializer().Serialize(personInfo);
            shortcutMethod.printf(s1 + "\n" + s2);

            var differences = oldHistory.CompareObj(personInfo);

            JObject jsonnewlog = new JObject();
            JObject jsonoldlog = new JObject();

            for (int i = 0; i < differences.Count(); i++)
            {
                string typeA = differences[i].valA.GetType().ToString();
                string typeB = differences[i].valB.GetType().ToString();
                if (typeA.Contains("Int") || typeB.Contains("Int"))
                {
                    jsonoldlog.Add(differences[i].PropertyName, Int32.Parse(differences[i].valA.ToString()));
                    jsonnewlog.Add(differences[i].PropertyName, Int32.Parse(differences[i].valB.ToString()));
                }
                else
                {
                    jsonoldlog.Add(differences[i].PropertyName, differences[i].valA.ToString());
                    jsonnewlog.Add(differences[i].PropertyName, differences[i].valB.ToString());
                }
            }



            int approved = 0;
            int supervisornotified = 1;
            int logCategoryID_BasedOnTypeDeleted = 5;



            string logDesc = logDesc_BasedOnTypeDeleted; // Short details
            int logCategoryID = logCategoryID_BasedOnTypeDeleted; // choose categoryID
                                                                  // int patientID = Vital.patientID;
                                                                  //////
            int userIDInit = shortcutMethod.getUserDetails(token, null).userID;

            int userIDApproved = shortcutMethod.getUserDetails(token, null).userID;
            string additionalInfo = null; // None just null
            string remarks = null; // none just null
            string tableAffected = "Patient"; //Which database table that got affected
            string columnAffected = columns; // Which database table column is affected
            int rowAffected = patientID; // Not needed just put 0
            int supNotified = supervisornotified;  //Not needed just put 0
            int userNotified = 1;// Not needed just put 0

            int success = _context.SaveChanges();



            if (success > 0)
            {
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientID, userIDInit, userIDApproved, null, additionalInfo,
                remarks, tableAffected, columnAffected, "", "", supNotified, userNotified, approved, "");
                return "Update Successfully. Row updated:" + success;
            }
            else
            {
                return "Update failed" + oldLogData + "///////////////" + logData;
            }



        }


        //  {type,id,desc,token}
        //type ->Likes,Dislikes,Habits,Hobby
        [HttpPut]
        [Route("api/Patient/updatePreference")]
        public string updatePreference(HttpRequestMessage bodyResult)
        {
            string preferenceJson = bodyResult.Content.ReadAsStringAsync().Result;
            string oldLogData = "";
            string logData = "";// Longer details
            string columns = "";

            JObject jsonpreference = JObject.Parse(preferenceJson);
            string token = (string)jsonpreference.SelectToken("token");
            int patientId = (int)jsonpreference.SelectToken("patientId");
            string userType = shortcutMethod.getUserType(token, null);
            //   shortcutMethod.printf("updateVital_String");
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;
            // shortcutMethod.printf("UserType not guardian/Invalid user");


            JArray jsonPreferenceArray = (JArray)jsonpreference.SelectToken("Preference");


            // int patientID = Vital.patientID;
            int approved = 0;
            int supervisornotified = 1;
            int logCategoryID_BasedOnTypeDeleted = 5;                                                 //////
            int userIDInit = shortcutMethod.getUserDetails(token, null).userID;
            int logCategoryID = logCategoryID_BasedOnTypeDeleted; // choose categoryID
            int userIDApproved = shortcutMethod.getUserDetails(token, null).userID;
            string additionalInfo = null; // None just null
            string remarks = null; // none just null
            string tableAffected = "Multiple"; //Which database table that got affected
            string columnAffected = columns; // Which database table column is affected
            int rowAffected = 0; // Not needed just put 0
            int supNotified = supervisornotified;  //Not needed just put 0
            int userNotified = 1;// Not needed just put 0
            String res = "";
            String logDesc_BasedOnTypeDeleted = "Update" + columns + " info for Preference"; ;
            string logDesc = logDesc_BasedOnTypeDeleted; // Short details
            int success = 0;
            try
            {
                foreach (var item in jsonPreferenceArray.Children())
                {
                    var itemProperties = item.Children<JProperty>();
                    String typeElement = (string)itemProperties.FirstOrDefault(x => x.Name == "type").Value;
                    int idElement = (int)itemProperties.FirstOrDefault(x => x.Name == "id").Value;
                    string descElement = (string)itemProperties.FirstOrDefault(x => x.Name == "desc").Value;
                    /*
                    switch (typeElement)
                    {
                        case "Likes":
                            var likes = _context.Likes.Where(x => (x.likeID == idElement && x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault();
                            Like likesoldHistory = new Like();
                            if (descElement.Equals(likesoldHistory.likeItem))
                            {
                                oldLogData = oldLogData + " Likes:" + likesoldHistory.likeItem;
                                logData = logData + " Likes:" + descElement;
                                columns = " Likes";
                                likes.likeItem = descElement;
                                success = _context.SaveChanges();
                            }

                            break;
                        case "Dislikes":
                            var disLikes = _context.Dislikes.Where(x => (x.dislikeID == idElement && x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault();
                            Dislike dislikesoldHistory = new Dislike();
                            if (descElement.Equals(dislikesoldHistory.dislikeItem))
                            {

                                oldLogData = oldLogData + " Dislikes:" + dislikesoldHistory.dislikeItem;
                                logData = logData + " Dislikes:" + descElement;
                                columns = " Dislikes";
                                disLikes.dislikeItem = descElement;
                                success = _context.SaveChanges();
                            }
                            break;
                        case "Hobby":
                            var hobby = _context.Hobbieses.Where(x => (x.hobbyID == idElement && x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault();
                            Hobbies hobbyoldHistory = new Hobbies();
                            if (descElement.Equals(hobbyoldHistory.hobby))
                            {

                                oldLogData = oldLogData + " Hobby:" + hobbyoldHistory.hobby;
                                logData = logData + " Hobby:" + descElement;
                                columns = " Hobby";
                                hobby.hobby = descElement;
                                success = _context.SaveChanges();
                            }
                            break;
                        case "Habit":
                            var habits = _context.Habits.Where(x => (x.habitID == idElement && x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault();
                            Habit habitoldHistory = new Habit();
                            if (descElement.Equals(habitoldHistory.habit))
                            {

                                oldLogData = oldLogData + " Habit:" + habitoldHistory.habit;
                                logData = logData + " Habit:" + descElement;
                                columns = " Habit";
                                habits.habit = descElement;
                                success = _context.SaveChanges();
                            }
                            break;
                        default:
                            break;
                    }*/

                    if (success > 0)
                    {
                        // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                        shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientId, userIDInit, userIDApproved, null, additionalInfo,
                        remarks, tableAffected, columnAffected, "", "", supNotified, userNotified, approved, "");

                    }

                }
                return "Update Successfully.";
            }
            catch (Exception ex)
            {
                return ex.InnerException.Message;
            }

        }

        [HttpPut]
        [Route("api/Patient/updateSinglePreference")]
        public string updateSinglePreference(HttpRequestMessage bodyResult)
        {
            string preferenceJson = bodyResult.Content.ReadAsStringAsync().Result;
            string oldLogData = "";
            string logData = "";// Longer details
            string columns = "";

            JObject jsonpreference = JObject.Parse(preferenceJson);
            string token = (string)jsonpreference.SelectToken("token");
            int patientId = (int)jsonpreference.SelectToken("patientId");
            string typeElement = (string)jsonpreference.SelectToken("type");
            string descElement = (string)jsonpreference.SelectToken("desc");
            int idElement = (int)jsonpreference.SelectToken("id");
            string userType = shortcutMethod.getUserType(token, null);
            //   shortcutMethod.printf("updateVital_String");
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;
            // shortcutMethod.printf("UserType not guardian/Invalid user");

            int success = 0;



            // int patientID = Vital.patientID;

            try
            {

                /*
                switch (typeElement)
                {
                    case "Likes":
                        var likes = _context.Likes.Where(x => (x.likeID == idElement && x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault();
                        Like likesoldHistory = new Like();


                        oldLogData = oldLogData + " Likes:" + likesoldHistory.likeItem;
                        logData = logData + " Likes:" + descElement;
                        columns = " Likes";
                        likes.likeItem = descElement;
                        success = _context.SaveChanges();


                        break;
                    case "Dislikes":
                        var disLikes = _context.Dislikes.Where(x => (x.dislikeID == idElement && x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault();
                        Dislike dislikesoldHistory = new Dislike();


                        oldLogData = oldLogData + " Dislikes:" + dislikesoldHistory.dislikeItem;
                        logData = logData + " Dislikes:" + descElement;
                        columns = " Dislikes";
                        disLikes.dislikeItem = descElement;
                        success = _context.SaveChanges();

                        break;
                    case "Hobby":
                        var hobby = _context.Hobbieses.Where(x => (x.hobbyID == idElement && x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault();
                        Hobbies hobbyoldHistory = new Hobbies();


                        oldLogData = oldLogData + " Hobby:" + hobbyoldHistory.hobby;
                        logData = logData + " Hobby:" + descElement;
                        columns = " Hobby";
                        hobby.hobby = descElement;
                        success = _context.SaveChanges();

                        break;
                    case "Habit":
                        var habits = _context.Habits.Where(x => (x.habitID == idElement && x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault();
                        Habit habitoldHistory = new Habit();


                        oldLogData = oldLogData + " Habit:" + habitoldHistory.habit;
                        logData = logData + " Habit:" + descElement;
                        columns = " Habit";
                        habits.habit = descElement;
                        success = _context.SaveChanges();

                        break;
                    default:
                        break;
                }*/
                int approved = 0;
                int supervisornotified = 1;
                int logCategoryID_BasedOnTypeDeleted = 5;                                                 //////
                int userIDInit = shortcutMethod.getUserDetails(token, null).userID;
                int logCategoryID = logCategoryID_BasedOnTypeDeleted; // choose categoryID
                int userIDApproved = shortcutMethod.getUserDetails(token, null).userID;
                string additionalInfo = null; // None just null
                string remarks = null; // none just null
                string tableAffected = "Multiple"; //Which database table that got affected
                string columnAffected = columns; // Which database table column is affected
                int rowAffected = 0; // Not needed just put 0
                int supNotified = supervisornotified;  //Not needed just put 0
                int userNotified = 1;// Not needed just put 0
                String res = "";
                String logDesc_BasedOnTypeDeleted = "Update" + columns + " info for Preference"; ;
                string logDesc = logDesc_BasedOnTypeDeleted; // Short details

                if (success > 0)
                {
                    // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                    shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientId, userIDInit, userIDApproved, null, additionalInfo,
                    remarks, tableAffected, columnAffected, "", "", supNotified, userNotified, approved, "");

                }


                return "Update Successfully.";
            }
            catch (Exception ex)
            {
                return ex.InnerException.Message;
            }

        }


        [HttpPut]
        [Route("api/Patient/addPreferenceToDB")]
        public String addPreferenceToDB(HttpRequestMessage bodyResult)
        {
            string preferenceJson = bodyResult.Content.ReadAsStringAsync().Result;
            string oldLogData = "";
            string logData = "";// Longer details
            string columns = "";

            JObject jsonpreference = JObject.Parse(preferenceJson);
            string token = (string)jsonpreference.SelectToken("token");
            int patientId = (int)jsonpreference.SelectToken("patientId");

            string typeElement = (string)jsonpreference.SelectToken("type");
            string descElement = (string)jsonpreference.SelectToken("desc");
            string userType = shortcutMethod.getUserType(token, null);
            string tableAffected = ""; //Which database table that got affected

            //   shortcutMethod.printf("updateVital_String");
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;
            // shortcutMethod.printf("UserType not guardian/Invalid user");


            JArray jsonPreferenceArray = (JArray)jsonpreference.SelectToken("Preference");


            // int patientID = Vital.patientID;

            int success = 0;
            try
            {
                int socialId = 0;
                int lastId = 0;
                JArray jobjPatientSocialHistoryInfo = new JArray();
                var socialhistory = _context.SocialHistories.Where(x => (x.patientAllocationID == patientId && x.isApproved == 1 && x.isDeleted == 0)).ToList();
                for (int i = 0; i < 1; i++)
                {
                    JObject jobjSocialHistory = new JObject();

                    socialId = socialhistory[i].socialHistoryID;
                }
                /*
                switch (typeElement)
                {
                    case "Likes":
                        var insertIntoLikes = new Like()
                        {
                            likeItem = descElement,
                            isApproved = 1,
                            isDeleted = 0,
                            socialHistoryID = socialId

                        };
                        tableAffected = "Likes";

                        _context.Likes.Add(insertIntoLikes);
                        success = _context.SaveChanges();
                        lastId = insertIntoLikes.likeID;
                        break;
                    case "Dislikes":
                        var insertIntoDislikes = new Dislike()
                        {
                            dislikeItem = descElement,
                            isApproved = 1,
                            isDeleted = 0,
                            socialHistoryID = socialId
                        };
                        _context.Dislikes.Add(insertIntoDislikes);
                        success = _context.SaveChanges();
                        lastId = insertIntoDislikes.dislikeID;
                        break;
                    case "Hobby":
                        var insertIntoHobby = new Hobbies()
                        {
                            hobby = descElement,
                            isApproved = 1,
                            isDeleted = 0,
                            socialHistoryID = socialId
                        };
                        _context.Hobbieses.Add(insertIntoHobby);
                        success = _context.SaveChanges();
                        lastId = insertIntoHobby.hobbyID;
                        break;
                    case "Habit":
                        var insertIntoHabit = new Habit()
                        {
                            habit = descElement,
                            isApproved = 1,
                            isDeleted = 0,
                            socialHistoryID = socialId

                        };
                        _context.Habits.Add(insertIntoHabit);
                        success = _context.SaveChanges();
                        lastId = insertIntoHabit.habitID;
                        break;
                    default:
                        break;
                }*/
                int approved = 0;
                int supervisornotified = 0;
                int logCategoryID_BasedOnTypeDeleted = 5;                                                 //////
                int userIDInit = shortcutMethod.getUserDetails(token, null).userID;
                int logCategoryID = logCategoryID_BasedOnTypeDeleted; // choose categoryID
                int userIDApproved = shortcutMethod.getUserDetails(token, null).userID;
                string additionalInfo = null; // None just null
                string remarks = null; // none just null
                string columnAffected = columns; // Which database table column is affected
                int rowAffected = 0; // Not needed just put 0
                int supNotified = 0;  //Not needed just put 0
                int userNotified = 0;// Not needed just put 0
                String res = "";
                String logDesc_BasedOnTypeDeleted = "Insert" + columns + " info for Preference"; ;
                string logDesc = logDesc_BasedOnTypeDeleted; // Short details
                if (success > 0)
                {

                    // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                    shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientId, userIDInit, userIDApproved, null, additionalInfo,
                    remarks, tableAffected, columnAffected, "", "", supNotified, userNotified, approved, "");

                }


                return lastId.ToString();
            }
            catch (Exception ex)
            {
                return ex.InnerException.Message;
            }
        }


        //  {type,id,desc,token}
        //type ->Likes,Dislikes,Habits,Hobby
        [HttpPut]
        [Route("api/Patient/DeletePreference")]
        public string DeletePreference(HttpRequestMessage bodyResult)
        {
            string preferenceJson = bodyResult.Content.ReadAsStringAsync().Result;
            string oldLogData = "";
            string logData = "";// Longer details
            string columns = "";

            JObject jsonpreference = JObject.Parse(preferenceJson);
            string token = (string)jsonpreference.SelectToken("token");
            int patientId = (int)jsonpreference.SelectToken("patientId");
            int idElement = (int)jsonpreference.SelectToken("PreferenceId");
            string typeElement = (string)jsonpreference.SelectToken("type");
            string descElement = (string)jsonpreference.SelectToken("desc");
            string userType = shortcutMethod.getUserType(token, null);
            //   shortcutMethod.printf("updateVital_String");
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;
            // shortcutMethod.printf("UserType not guardian/Invalid user");

            int success = 0;
            JArray jsonPreferenceArray = (JArray)jsonpreference.SelectToken("Preference");



            try
            {
                /*
                switch (typeElement)
                {
                    case "Likes":
                        var likes = _context.Likes.Where(x => (x.likeID == idElement && x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault();
                        Like likesoldHistory = new Like();


                        oldLogData = oldLogData + " Likes:" + likesoldHistory.likeItem;
                        logData = logData + " Likes:" + descElement;
                        columns = " Likes";
                        likes.isDeleted = 1;
                        success = _context.SaveChanges();


                        break;
                    case "Dislikes":
                        var disLikes = _context.Dislikes.Where(x => (x.dislikeID == idElement && x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault();
                        Dislike dislikesoldHistory = new Dislike();


                        oldLogData = oldLogData + " Dislikes:" + dislikesoldHistory.dislikeItem;
                        logData = logData + " Dislikes:" + descElement;
                        columns = " Dislikes";
                        disLikes.isDeleted = 1;
                        success = _context.SaveChanges();

                        break;
                    case "Hobby":
                        var hobby = _context.Hobbieses.Where(x => (x.hobbyID == idElement && x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault();
                        Hobbies hobbyoldHistory = new Hobbies();

                        oldLogData = oldLogData + " Hobby:" + hobbyoldHistory.hobby;
                        logData = logData + " Hobby:" + descElement;
                        columns = " Hobby";
                        hobby.isDeleted = 1;
                        success = _context.SaveChanges();

                        break;
                    case "Habit":
                        var habits = _context.Habits.Where(x => (x.habitID == idElement && x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault();
                        Habit habitoldHistory = new Habit();


                        oldLogData = oldLogData + " Habit:" + habitoldHistory.habit;
                        logData = logData + " Habit:" + descElement;
                        columns = " Habit";
                        habits.isDeleted = 1;
                        success = _context.SaveChanges();

                        break;
                    default:
                        break;
                }*/
                // int patientID = Vital.patientID;
                int approved = 0;
                int supervisornotified = 0;
                int logCategoryID_BasedOnTypeDeleted = 5;                                                 //////
                int userIDInit = shortcutMethod.getUserDetails(token, null).userID;
                int logCategoryID = logCategoryID_BasedOnTypeDeleted; // choose categoryID
                int userIDApproved = shortcutMethod.getUserDetails(token, null).userID;
                string additionalInfo = null; // None just null
                string remarks = null; // none just null
                string tableAffected = "Multiple"; //Which database table that got affected
                string columnAffected = columns; // Which database table column is affected
                int rowAffected = 0; // Not needed just put 0
                int supNotified = 0;  //Not needed just put 0
                int userNotified = 0;// Not needed just put 0
                String res = "";
                String logDesc_BasedOnTypeDeleted = "Delete" + columns + " info for Preference"; ;
                string logDesc = logDesc_BasedOnTypeDeleted; // Short details

                if (success > 0)
                {
                    // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                    shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientId, userIDInit, userIDApproved, null, additionalInfo,
                    remarks, tableAffected, columnAffected, "", "", supNotified, userNotified, approved, "");

                }


                return "Update Successfully.";
            }
            catch (Exception ex)
            {
                return ex.InnerException.Message;
            }



        }

        [HttpGet]
        [Route("api/Patient/patientGeneralList")]
        public HttpResponseMessage patientGeneralList(string token, int patientID)
        {
            // addLogToDB();

            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            JObject jobjSend = new JObject();
            JObject jobjPatientInfo = new JObject();
            JArray jobjPatientSocialHistoryInfo = new JArray();



            string diet = "";
            string religion = "";
            string pet = "";
            string occupation = "";
            string education = "";
            string liveWith = "";
            var socialhistory = _context.SocialHistories.Where(x => (x.patientAllocationID == patientID && x.isApproved == 1 && x.isDeleted == 0)).ToList();


            for (int i = 0; i < socialhistory.Count(); i++)
            {

                JObject jobjSocialHistory = new JObject();
                jobjSend["SocialHistoryID"] = socialhistory[i].socialHistoryID;
                diet = socialhistory[i].List_Diet.value;

                religion = socialhistory[i].List_Religion.value;
                pet = socialhistory[i].List_Pet.value;
                occupation = socialhistory[i].List_Occupation.value;
                education = socialhistory[i].List_Education.value;

                liveWith = socialhistory[i].List_LiveWith.value;
                jobjPatientSocialHistoryInfo.Add(jobjSocialHistory);
            }




            JArray jArrDiet = new JArray();
            JArray jArrReligion = new JArray();
            JArray jArrPet = new JArray();
            JArray jArrOccupation = new JArray();
            JArray jArrEducation = new JArray();
            JArray jArrLiveWith = new JArray();

            // jobjSocialHistory["SocialHistoryID"] = socialhistory.socialHistoryID;

            var dietObj = _context.ListDiets.ToList();
            jobjSend["SelectedDietID"] = 0;
            jobjSend["SelectedReligionID"] = 0;
            jobjSend["SelectedPetID"] = 0;
            jobjSend["SelectedOccupationID"] = 0;
            jobjSend["SelectedEducationID"] = 0;
            jobjSend["SelectedLiveWithID"] = 0;
            if (dietObj == null)
                jobjSend["DietDesc"] = "";
            else
            {
                for (int i = 0; i < dietObj.Count(); i++)
                {
                    JObject jobjDiets = new JObject();

                    jobjDiets["DietID"] = dietObj[i].list_dietID;

                    String desc = dietObj[i].value;
                    if (desc.Equals(diet)) { jobjSend["SelectedDietID"] = i; }

                    jobjDiets["DietDesc"] = desc;
                    jArrDiet.Add(jobjDiets);
                }
            }
            var religionObj = _context.ListReligions.ToList();
            if (religionObj == null)
                jobjSend["ReligionDesc"] = "";
            else
            {
                for (int i = 0; i < religionObj.Count(); i++)
                {
                    JObject jobjReligion = new JObject();
                    jobjReligion["ReligionID"] = religionObj[i].list_religionID;
                    String desc = religionObj[i].value;
                    if (desc.Equals(religion)) { jobjSend["SelectedReligionID"] = i; }
                    jobjReligion["ReligionDesc"] = desc;
                    jArrReligion.Add(jobjReligion);
                }
            }
            var petObj = _context.ListPets.ToList();
            if (petObj == null)
                jobjSend["Pet"] = "";
            else
            {
                for (int i = 0; i < petObj.Count(); i++)
                {
                    JObject jobjPet = new JObject();
                    jobjPet["PetID"] = petObj[i].list_petID;
                    String desc = petObj[i].value;
                    if (desc.Equals(pet)) { jobjSend["SelectedPetID"] = i; }
                    jobjPet["PetyDesc"] = desc;
                    jArrPet.Add(jobjPet);
                }
            }
            var occupationObj = _context.ListOccupations.ToList();
            if (occupationObj == null)
                jobjSend["OccupationDesc"] = "";
            else
            {
                for (int i = 0; i < occupationObj.Count(); i++)
                {
                    JObject jobjOccupation = new JObject();
                    jobjOccupation["OccupationID"] = occupationObj[i].list_occupationID;
                    String desc = occupationObj[i].value;
                    if (desc.Equals(occupation)) { jobjSend["SelectedOccupationID"] = i; }
                    jobjOccupation["OccupationDesc"] = desc;
                    jArrOccupation.Add(jobjOccupation);
                }
            }

            //Retrive the list
            var educationObj = _context.ListEducations.ToList();
            if (educationObj == null)
                jobjSend["EducationDesc"] = "";
            else
            {
                for (int i = 0; i < educationObj.Count(); i++)
                {
                    JObject jobjEducation = new JObject();
                    jobjEducation["EducationID"] = educationObj[i].list_educationID;
                    String desc = educationObj[i].value;
                    if (desc.Equals(education)) { jobjSend["SelectedEducationID"] = i; }
                    jobjEducation["EducationDesc"] = desc;
                    jArrEducation.Add(jobjEducation);
                }
            }
            var liveWithObj = _context.ListLiveWiths.ToList();
            if (liveWithObj == null)
                jobjSend["LiveWithDesc"] = "";
            else
            {
                for (int i = 0; i < liveWithObj.Count(); i++)
                {
                    JObject jobjLiveWith = new JObject();
                    jobjLiveWith["LiveWithID"] = liveWithObj[i].value;
                    String desc = liveWithObj[i].value;
                    if (desc.Equals(liveWith)) { jobjSend["SelectedLiveWithID"] = i; }
                    jobjLiveWith["LiveWithDesc"] = desc;
                    jArrLiveWith.Add(jobjLiveWith);
                }
            }




            jobjSend["DietList"] = jArrDiet;
            jobjSend["ReligionList"] = jArrReligion;
            jobjSend["PetList"] = jArrPet;
            jobjSend["OccupationList"] = jArrOccupation;
            jobjSend["EducationList"] = jArrEducation;
            jobjSend["LiveWithList"] = jArrLiveWith;


            string output = JsonConvert.SerializeObject(jobjSend);
            string json = jobjSend.ToString(Newtonsoft.Json.Formatting.None);
            shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(jobjSend);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");



            return response;
        }










        /*
        {
          "token":"1234",
          "patientID":33,
          "updateBit":1
        }
        */

        //https://localhost:44300/api/Patient/updateScheduleBit
        [HttpPut]
        [Route("api/Patient/updateScheduleBit")]
        public String updateScheduleBit(HttpRequestMessage bodyResult)
        {
            string resultString = bodyResult.Content.ReadAsStringAsync().Result;
            JObject resultJObject = JObject.Parse(resultString);

            string token = (string)resultJObject.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);

            if (userType.Equals("NONE") || resultString == null)
                return null;

            int patientID = (int)resultJObject.SelectToken("patientID");
            Patient selectedPatient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted == 0));
            if (selectedPatient == null)
                return (String.Format("patientID {0} is either not found, not approved or deleted in Patient table", patientID));

            Log log1 = _context.Logs.FirstOrDefault(x => (x.isDeleted == 0 && x.approved == 0 && x.reject == 0 && x.tableAffected.Contains("Patient") && x.rowAffected == selectedPatient.patientID));

            if (userType.Equals("Caregiver") && log1 != null)   // check for existing log, if it exist, don't update
                return "Failed to update. This request has previously been made before."; //Send result to frontend. So, based on the result frontend can prompt a error message.
            else if (userType.Equals("Supervisor") && log1 != null)
                return "Please approve the request before making further changes.";

            int updateBit = (int)resultJObject.SelectToken("updateBit");

            List<string> patientList = new List<string>();

            Models.Patient allFieldOfUpdatedPatient = new Models.Patient();
            foreach (var properties in selectedPatient.GetType().GetProperties())
            {
                allFieldOfUpdatedPatient.GetType().GetProperty(properties.Name).SetValue(allFieldOfUpdatedPatient, properties.GetValue(selectedPatient, null), null);
            }

            var patientUpdates = selectedPatient;

            int logCategoryID_Based = 5;
            String logDesc_Based = "";
            int approved = 0;
            int supervisorNotified = 0;
            int userIDApproved = 3; // Supervisor

            string s1 = new JavaScriptSerializer().Serialize(selectedPatient);

            if (userType.Equals("Caregiver"))
            {
                patientUpdates = allFieldOfUpdatedPatient;
            }

            if (updateBit != -1 && updateBit != selectedPatient.updateBit)
            {
                patientUpdates.updateBit = updateBit;
                patientList.Add("updateBit");
            }

            if (logDesc_Based == "" && patientList.Count > 0)
                logDesc_Based = "Update Patient info";

            if (userType.Equals("Supervisor"))
            {
                approved = 1;
                supervisorNotified = 1;
                patientUpdates.isApproved = 1;
            }

            string s2 = new JavaScriptSerializer().Serialize(patientUpdates);

            string oldLogData = s1;
            string newlogData = s2;

            string logDesc = logDesc_Based; // Short details
            int logCategoryID = logCategoryID_Based; // choose categoryID
            int userIDInit = shortcutMethod.getUserDetails(token, null).userID; // Ownself

            string additionalInfo = null;
            string remarks = null;
            string tableAffected = "Patient";
            string columnAffected = string.Join(",", patientList);
            int rowAffected = patientUpdates.patientID;
            int userNotified = 1;

            if (patientList.Count > 0)
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(oldLogData, newlogData, logDesc, logCategoryID, patientUpdates.patientID, userIDInit, userIDApproved, null, additionalInfo,
                    remarks, tableAffected, columnAffected, "", "", supervisorNotified, userNotified, approved, "");
            _context.SaveChanges();

            if (userType.Equals("Caregiver"))
                return "Please wait for supervisor approval.";
            else
                return "Update Successfully.";
        }

    }
}
