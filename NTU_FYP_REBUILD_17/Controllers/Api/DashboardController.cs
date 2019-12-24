using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.Dtos;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;

using System.Globalization;

namespace NTU_FYP_REBUILD_17.Controllers.Api
{
    public class DashboardController : ApiController
    {

        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();

        public DashboardController()
        {
            _context = new ApplicationDbContext();
        }

        //http://localhost:50217/api/Dashboard/displayPatients_JSONString?token=1234&userID=3
        [HttpGet]
        [Route("api/Dashboard/displayPatients_JSONString")]
        public HttpResponseMessage displayPatients_JSONString(string token, int userID)
        {
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            JArray overallJArray = new JArray();
            DateTime date = DateTime.Now;

            //var patientAllocationList = _context.PatientAllocations.Where(x => (x.caregiverID == userID && x.isApproved == 1 && x.isDeleted != 1)).ToList();

            var patientAllocationList = (from pa in _context.PatientAllocations
                                         join p in _context.Patients on pa.patientID equals p.patientID
                                         where pa.isApproved == 1 && pa.isDeleted != 1
                                         where p.isApproved == 1 && p.isActive == 1 && p.isDeleted != 1 && (p.endDate == null || p.endDate >= date) && (p.inactiveDate == null || p.inactiveDate > date) && p.startDate <= date
                                         orderby p.firstName ascending
                                         select pa).ToList();

            foreach (var curPatient in patientAllocationList)
            {
                JArray jarrayAlbum = new JArray();
                JArray jarraySchedule = new JArray();

                JObject patientJObj = new JObject();

                var viewPatient = _context.Patients.SingleOrDefault(x => x.patientID == curPatient.patientID);
                var patient = _context.Patients.SingleOrDefault((x => (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted != 1)));
                if (patient == null)
                    return null;

                patientJObj["firstName"] = patient.firstName;
                patientJObj["lastName"] = patient.lastName;

                List<PatientSchedule> viewModel = new List<PatientSchedule>();

                var routine = (from s in _context.Schedules
                               join pa in _context.PatientAllocations on s.patientAllocationID equals pa.patientAllocationID
                               join p in _context.Patients on pa.patientID equals p.patientID
                               //join c in _context.CentreActivities on s.centreActivityID equals c.centreActivityID
                               join r in _context.Routines on p.patientID equals r.patientAllocationID
                               where s.isApproved == 1 && s.isDeleted != 1
                               where pa.isApproved == 1 && pa.isDeleted != 1
                               where p.isApproved == 1 && p.isDeleted != 1
                               where s.dateStart == date.Date
                               where p.patientID == viewPatient.patientID
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
                                where p.patientID == viewPatient.patientID
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
                                }).ToList();

                var patientUnion = activity.Concat(routine).OrderBy(m => m.scheduleId).ToList();

                if (patientUnion.Count > 0)
                {
                    // Split the result into interval before adding it into the list
                    // This will make the front end side easier to work with

                    //List<PatientSchedule> newSlot = new List<PatientSchedule>();

                    DateTime timeStart = DateTime.ParseExact("09:00:00", "HH:mm:ss", CultureInfo.InvariantCulture);
                    //DateTime timeEnd = DateTime.ParseExact("17:00:00", "HH:mm:ss", CultureInfo.InvariantCulture);

                    DateTime tempStart = timeStart;
                    DateTime tempEnd = timeStart;

                    foreach (PatientSchedule data in patientUnion)
                    {
                        TimeSpan timeDiff = data.timeEnd - data.timeStart;
                        // Getting Number of half hour slot for this activity
                        double noOfSlot = timeDiff.TotalMilliseconds / 1800000;

                        for (int x = 0; x < noOfSlot; x++)
                        {
                            //tempEnd = tempEnd.AddMinutes(30);

                            PatientSchedule newData = new PatientSchedule(data);
                            newData.timeStart = data.timeStart.Add(new TimeSpan(0, (30 * x), 0));
                            newData.timeEnd = data.timeStart.Add(new TimeSpan(0, (30 * (x + 1)), 0));

                            viewModel.Add(newData);

                            //tempStart = tempStart.AddMinutes(30);
                        }

                    }

                }

                foreach (PatientSchedule curAct in viewModel)
                {
                    JObject b = new JObject();

                    if (curAct.centreActivityTitle == null)
                    {
                        b["eventName"] = curAct.routineName;
                        b["startTime"] = curAct.timeStart;
                        b["endTime"] = curAct.timeEnd;
                    }
                    else
                    {
                        b["eventName"] = curAct.centreActivityTitle;
                        b["startTime"] = curAct.timeStart;
                        b["endTime"] = curAct.timeEnd;
                    }

                    jarraySchedule.Add(b);
                }

                patientJObj["Schedule"] = jarraySchedule;


                //var dateAndTime = DateTime.Now;
                //var date = dateAndTime.Date;
                //int patientALlocationID = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted != 1)).patientAllocationID;
                //var schedule = _context.Schedules.Where(x => (x.patientAllocationID == patientALlocationID && x.isApproved == 1 && x.isDeleted != 1 && x.dateStart == date)).ToList();
                //for (int m = 0; m < schedule.Count(); m++)
                //{
                //    JObject b = new JObject();
                //    int? scheduleCentreActivityID = schedule[m].centreActivityID;
                //    shortcutMethod.printf("" + scheduleCentreActivityID);
                //    var centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == scheduleCentreActivityID && x.isApproved == 1 && x.isDeleted != 1));
                //    shortcutMethod.printf("Deciding");
                //    if (centreActivity == null)
                //    {
                //        shortcutMethod.printf("centreActivity is null");
                //        int? scheduleRoutineID = schedule[m].routineID;
                //        var routine = _context.Routines.SingleOrDefault(x => (x.routineID == scheduleRoutineID && x.isApproved == 1 && x.isDeleted == 0));
                //        JArray tempSchedule = new JArray();
                //        b["eventName"] = routine.eventName;
                //        b["startTime"] = routine.startTime;
                //        b["endTime"] = routine.endTime;
                //    }
                //    else
                //    {
                //        shortcutMethod.printf("centreActivity is not null");
                //        shortcutMethod.printf(schedule[m].scheduleID + "timeStart" + schedule[m].timeStart + "timeEnd:" + schedule[m].timeEnd);
                //        b["eventName"] = centreActivity.activityTitle;
                //        b["startTime"] = schedule[m].timeStart;
                //        b["endTime"] = schedule[m].timeEnd;
                //        //shortcutMethod.printf(jarraySchedule.ToString());
                //    }
                //    jarraySchedule.Add(b);
                //}
                //patientJObj["Schedule"] = jarraySchedule;

                //var patientAllocation = _context.PatientAllocations.SingleOrDefault( x => x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted == 0)
                var albumPath = (from pa in _context.PatientAllocations
                                 join p in _context.Patients on pa.patientID equals p.patientID
                                 join a in _context.AlbumPatient on pa.patientAllocationID equals a.patientAllocationID
                                 where a.albumCatID == 1
                                 where pa.isApproved == 1 && pa.isDeleted != 1
                                 where p.isApproved == 1 && p.isDeleted != 1
                                 where a.isApproved == 1 && a.isDeleted != 1
                                 where pa.patientID == viewPatient.patientID
                                 select a).SingleOrDefault();
                //var albumPath = _context.Albums.SingleOrDefault(x => (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted != 1));
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

        //http://localhost:50217/api/Dashboard/displayPatients_JSONString?token=1234
        [HttpGet]
        [Route("api/Dashboard/displayPatients_JSONString")]
        public HttpResponseMessage displayPatients_JSONString(string token)
        {
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            JArray overallJArray = new JArray();

            DateTime date = DateTime.Now;

            //var patientAllocationList = _context.PatientAllocations.Where(x => (x.caregiverID == userID && x.isApproved == 1 && x.isDeleted != 1)).ToList();

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
                var patient = _context.Patients.SingleOrDefault((x => (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted != 1)));
                if (patient == null)
                    return null;

                patientJObj["firstName"] = patient.firstName;
                patientJObj["lastName"] = patient.lastName;

                //var dateAndTime = DateTime.Now;
                //var date = dateAndTime.Date;

                List<PatientSchedule> viewModel = new List<PatientSchedule>();
    
                var routine = (from s in _context.Schedules
                                join pa in _context.PatientAllocations on s.patientAllocationID equals pa.patientAllocationID
                                join p in _context.Patients on pa.patientID equals p.patientID
                                //join c in _context.CentreActivities on s.centreActivityID equals c.centreActivityID
                                join r in _context.Routines on p.patientID equals r.patientAllocationID
                               where s.isApproved == 1 && s.isDeleted != 1
                                where pa.isApproved == 1 && pa.isDeleted != 1
                                where p.isApproved == 1 && p.isDeleted != 1
                                where s.dateStart == date.Date
                                where p.patientID == viewPatient.patientID
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
                                where p.patientID == viewPatient.patientID
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
                                }).ToList();

                var patientUnion = activity.Concat(routine).OrderBy(m => m.scheduleId).ToList();

                if (patientUnion.Count > 0)
                {
                    // Split the result into interval before adding it into the list
                    // This will make the front end side easier to work with

                    //List<PatientSchedule> newSlot = new List<PatientSchedule>();

                    DateTime timeStart = DateTime.ParseExact("09:00:00", "HH:mm:ss", CultureInfo.InvariantCulture);
                    //DateTime timeEnd = DateTime.ParseExact("17:00:00", "HH:mm:ss", CultureInfo.InvariantCulture);

                    DateTime tempStart = timeStart;
                    DateTime tempEnd = timeStart;

                    foreach (PatientSchedule data in patientUnion)
                    {
                        TimeSpan timeDiff = data.timeEnd - data.timeStart;
                        // Getting Number of half hour slot for this activity
                        double noOfSlot = timeDiff.TotalMilliseconds / 1800000;

                        for (int x = 0; x < noOfSlot; x++)
                        {
                            //tempEnd = tempEnd.AddMinutes(30);

                            PatientSchedule newData = new PatientSchedule(data);
                            newData.timeStart = data.timeStart.Add(new TimeSpan(0, (30 * x), 0));
                            newData.timeEnd = data.timeStart.Add(new TimeSpan(0, (30 * (x+1)), 0));

                            viewModel.Add(newData);

                            //tempStart = tempStart.AddMinutes(30);
                        }

                    }

                }

                foreach (PatientSchedule curAct in viewModel)
                {
                    JObject b = new JObject();

                    if (curAct.centreActivityTitle == null)
                    {
                        b["eventName"] = curAct.routineName;
                        b["startTime"] = curAct.timeStart;
                        b["endTime"] = curAct.timeEnd;
                    }
                    else
                    {
                        b["eventName"] = curAct.centreActivityTitle;
                        b["startTime"] = curAct.timeStart;
                        b["endTime"] = curAct.timeEnd;
                    }

                    jarraySchedule.Add(b);
                }

                patientJObj["Schedule"] = jarraySchedule;



















                //int patientALlocationID = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted != 1)).patientAllocationID;
                //var schedule = _context.Schedules.Where(x => (x.patientAllocationID == patientALlocationID && x.isApproved == 1 && x.isDeleted != 1 && x.dateStart == date)).ToList();
                //for (int m = 0; m < schedule.Count(); m++)
                //{
                //    JObject b = new JObject();
                //    int? scheduleCentreActivityID = schedule[m].centreActivityID;
                //    shortcutMethod.printf("" + scheduleCentreActivityID);
                //    var centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == scheduleCentreActivityID && x.isApproved == 1 && x.isDeleted != 1));
                //    shortcutMethod.printf("Deciding");
                //    if (centreActivity == null)
                //    {
                //        shortcutMethod.printf("centreActivity is null");
                //        int? scheduleRoutineID = schedule[m].routineID;
                //        var routine = _context.Routines.SingleOrDefault(x => (x.routineID == scheduleRoutineID && x.isApproved == 1 && x.isDeleted != 1));
                //        JArray tempSchedule = new JArray();
                //        b["eventName"] = routine.eventName;
                //        b["startTime"] = routine.startTime;
                //        b["endTime"] = routine.endTime;
                //    }
                //    else
                //    {
                //        shortcutMethod.printf("centreActivity is not null");
                //        shortcutMethod.printf(schedule[m].scheduleID + "timeStart" + schedule[m].timeStart + "timeEnd:" + schedule[m].timeEnd);
                //        b["eventName"] = centreActivity.activityTitle;
                //        b["startTime"] = schedule[m].timeStart;
                //        b["endTime"] = schedule[m].timeEnd;
                //        //shortcutMethod.printf(jarraySchedule.ToString());
                //    }
                //    jarraySchedule.Add(b);
                //}
                //patientJObj["Schedule"] = jarraySchedule;





                //var patientAllocation = _context.PatientAllocations.SingleOrDefault( x => x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted != 1)
                var albumPath = (from pa in _context.PatientAllocations
                                 join p in _context.Patients on pa.patientID equals p.patientID
                                 join a in _context.AlbumPatient on pa.patientAllocationID equals a.patientAllocationID
                                 where a.albumCatID == 1
                                 where pa.isApproved == 1 && pa.isDeleted != 1
                                 where p.isApproved == 1 && p.isDeleted != 1
                                 where a.isApproved == 1 && a.isDeleted != 1
                                 where pa.patientID == viewPatient.patientID
                                 select a).SingleOrDefault();
                //var albumPath = _context.Albums.SingleOrDefault(x => (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted != 1));
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

        //http://localhost:50217/api/Dashboard/displayActivities_JSONString?token=1234
        [HttpGet]
        [Route("api/Dashboard/displayActivities_JSONString")]
        public HttpResponseMessage displayActivities_JSONString(string token)
        {
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            JArray overallJArray = new JArray();

            var allActivies = (from act in _context.CentreActivities
                               orderby act.activityTitle ascending
                               where act.isDeleted != 1 && act.isApproved == 1
                               select act.activityTitle).ToList();

            foreach (var curActivities in allActivies)
            {
                overallJArray.Add(curActivities);
            }

            string output = JsonConvert.SerializeObject(overallJArray);
            string json = overallJArray.ToString(Newtonsoft.Json.Formatting.None);
            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(overallJArray);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}