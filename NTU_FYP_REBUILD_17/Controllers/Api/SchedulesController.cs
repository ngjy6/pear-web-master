using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using NTU_FYP_REBUILD_17.Dtos;
using NTU_FYP_REBUILD_17.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace NTU_FYP_REBUILD_17.Controllers.Api
{
    public class SchedulesController : ApiController
    {
        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        Controllers.Synchronization.ScheduleMethod scheduler = new Controllers.Synchronization.ScheduleMethod();

        public SchedulesController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetPatientBasedOnDoctorID(int id)
        {
            var patient = _context.Schedules.ToList()
                .Where(d => d.patientAllocationID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient.ToList().Select(Mapper.Map<Schedule, SchedulesDto>));
        }

        //https://localhost:44300/api/ScheduleController/generateWeeklySchedule
        [HttpGet]
        [Route("api/ScheduleController/generateWeeklySchedule")]
        public HttpResponseMessage generateWeeklySchedule()
        {
            string logDesc = "Weekly schedule generation";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            string remarks = "initiating auto weekly schedule";

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, null, logDesc, logCategoryID, null, null, null, null, null, remarks, "schedule", "ALL", null, null, null, 1, 1, null);

            scheduler.generateWeeklySchedule(true, false);

            remarks = "auto weekly schedule generated";
            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, null, logDesc, logCategoryID, null, null, null, null, null, remarks, "schedule", "ALL", null, null, null, 1, 1, null);

            //string jsonString = JsonConvert.SerializeObject(scheduledActivities);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            //response.Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        //https://localhost:44300/api/ScheduleController/generateDailySchedule
        [HttpGet]
        [Route("api/ScheduleController/generateDailySchedule")]
        public HttpResponseMessage generateDailySchedule()
        {
            string logDesc = "Daily schedule generation";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            string remarks = "initiating auto daily schedule";

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, null, logDesc, logCategoryID, null, null, null, null, null, remarks, "schedule", "ALL", null, null, null, 1, 1, null);

            scheduler.generateWeeklySchedule(false, true);

            remarks = "auto daily schedule generated";
            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, null, logDesc, logCategoryID, null, null, null, null, null, remarks, "schedule", "ALL", null, null, null, 1, 1, null);

            //string jsonString = JsonConvert.SerializeObject(scheduledActivities);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            //response.Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        /* from here onwards, for testing */
        //https://localhost:44300/api/ScheduleController/weeklyTest
        [HttpGet]
        [Route("api/ScheduleController/weeklyTest")]
        public HttpResponseMessage weeklyTest()
        {
            JArray schedule = scheduler.generateWeeklySchedule(true, false);

            string jsonString = JsonConvert.SerializeObject(schedule);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        //https://localhost:44300/api/ScheduleController/dailyTest
        [HttpGet]
        [Route("api/ScheduleController/dailyTest")]
        public HttpResponseMessage dailyTest()
        {
            JArray schedule = scheduler.generateWeeklySchedule(false, true);

            string jsonString = JsonConvert.SerializeObject(schedule);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        //https://localhost:44300/api/ScheduleController/generateAllDailySchedule
        [HttpGet]
        [Route("api/ScheduleController/generateAllDailySchedule")]
        public HttpResponseMessage generateAllDailySchedule()
        {
            JArray schedule = scheduler.generateWeeklySchedule(false, false);

            string jsonString = JsonConvert.SerializeObject(schedule);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}
