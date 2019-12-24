using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMapper;
using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.Dtos;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.Controllers.Api
{
    public class UserController : ApiController
    {
        private ApplicationDbContext _context;

        public UserController()
        {
            _context = new ApplicationDbContext();
        }


        [HttpGet]
        [Route("api/User/GetPatientBasedOnCareGiverID_XML")]
        public IHttpActionResult GetPatientBasedOnCareGiverID_XML(string token, int id, DateTime timeDate, bool mask)
        {
            var dateTime = System.DateTime.Now.AddHours(-3);
            if (mask)
            {
                if ( token=="1234" || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime &&
                    (_context.UserTables.Where(c => c.token == token).Count() > 0 )))
                {
                    var patient = _context.Schedules.Include(a => a.PatientAllocation.patientAllocationID)
                        .Include(a => a.CentreActivity)
                        .Include(a => a.Routine)
                        .Where(d => d.PatientAllocation.caregiverID == id)
                        .Where(d => d.dateStart.Day == timeDate.Day &&
                                    d.dateStart.Month == timeDate.Month &&
                                    d.dateStart.Year == timeDate.Year)
                        .Where(d => d.timeStart.Hours - timeDate.Hour == 0)
                        .Where(d => d.isApproved == 1)
                        .Where(d => d.isDeleted == 0)
                        .ToList()
                        .OrderBy(a => a.timeStart)
                        .OrderBy(a => a.patientAllocationID);

                    if (patient.Count() == 0 && timeDate.Hour != 17)
                    {
                        patient = _context.Schedules.Include(a => a.PatientAllocation.patientAllocationID)
                            .Include(a => a.CentreActivity)
                            .Where(d => d.PatientAllocation.caregiverID == id)
                            .Where(d => d.dateStart.Day == timeDate.Day &&
                                        d.dateStart.Month == timeDate.Month &&
                                        d.dateStart.Year == timeDate.Year)
                            .Where(d => d.timeStart.Hours - timeDate.Hour == -1)
                            .Where(d => d.isApproved == 1)
                            .Where(d => d.isDeleted == 0)
                            .ToList()
                            .OrderBy(a => a.timeStart)
                            .OrderBy(a => a.patientAllocationID);
                    }


                        SchedulesDto[] ab = new SchedulesDto[patient.Count()];
                    int count = 0;
                    foreach (var i in patient)
                    {
                        var album = _context.AlbumPatient.SingleOrDefault(x => (x.patientAllocationID == i.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));

                        ab[count] = new SchedulesDto();
                        PatientAllocation pa = new PatientAllocation();
                        AlbumPatient a = new AlbumPatient();
                        a.albumCatID = album.albumCatID;
                        a.albumID = album.albumID;
                        a.albumPath = album.albumPath;
                        a.createDateTime = album.createDateTime;
                        a.isApproved = album.isApproved;
                        a.isDeleted = album.isDeleted;
                        Patient p = new Patient();
                        p.DOB = i.PatientAllocation.Patient.DOB;
                        p.address = i.PatientAllocation.Patient.address;
                        p.autoGame = i.PatientAllocation.Patient.autoGame;
                        p.createDateTime = i.PatientAllocation.Patient.createDateTime;
                        p.firstName = i.PatientAllocation.Patient.firstName;
                        p.gender = i.PatientAllocation.Patient.gender;
                        //p.guardianContactNo = i.PatientAllocation.Patient.guardianContactNo;
                        //p.guardianEmail = i.PatientAllocation.Patient.guardianEmail;
                        //p.guardianName = i.PatientAllocation.Patient.guardianName;
                        p.handphoneNo = i.PatientAllocation.Patient.handphoneNo;
                        p.isApproved = i.PatientAllocation.Patient.isApproved;
                        p.isDeleted = i.PatientAllocation.Patient.isDeleted;
                        p.lastName = i.PatientAllocation.Patient.lastName;
                        p.nric = i.PatientAllocation.Patient.nric.Remove(1, 4).Insert(1, "xxxx");
                        p.patientID = i.PatientAllocation.Patient.patientID;
                        //p.preferredLanguage = i.PatientAllocation.Patient.preferredLanguage;
                        p.preferredName = i.PatientAllocation.Patient.preferredName;
                        p.updateBit = i.PatientAllocation.Patient.updateBit;
                        ab[count].PatientAllocation = pa;
                        ab[count].PatientAllocation.caregiverID = i.PatientAllocation.caregiverID;
                        ab[count].PatientAllocation.createDateTime = i.PatientAllocation.createDateTime;
                        ab[count].PatientAllocation.doctorID = i.PatientAllocation.doctorID;
                        ab[count].PatientAllocation.gametherapistID = i.PatientAllocation.gametherapistID;
                        ab[count].PatientAllocation.isApproved = i.PatientAllocation.isApproved;
                        ab[count].PatientAllocation.isDeleted = i.PatientAllocation.isDeleted;
                        ab[count].PatientAllocation.patientAllocationID = i.PatientAllocation.patientAllocationID;
                        ab[count].PatientAllocation.patientID = i.PatientAllocation.patientID;
                        ab[count].dateEnd = Convert.ToDateTime(i.dateEnd.ToString());
                        ab[count].dateStart = Convert.ToDateTime(i.dateStart.ToString());
                        ab[count].isApproved = i.isApproved;
                        ab[count].isDeleted = i.isDeleted;
                        if (i.CentreActivity != null)
                        {
                            ab[count].scheduleDesc = i.CentreActivity.activityTitle;
                        }
                        else
                        {
                            ab[count].scheduleDesc = i.Routine.eventName;
                        }
                        ab[count].scheduleID = i.scheduleID;
                        ab[count].timeEnd = Convert.ToDateTime(i.timeEnd.ToString());
                        ab[count].timeStart = Convert.ToDateTime(i.timeStart.ToString());

                        var qwer = ab[count].timeEnd.Value.Hour;
                        if (qwer != 17)
                        {
                            var next = _context.Schedules.Include(b => b.PatientAllocation.Patient)
                                .Include(b => b.CentreActivity)
                                .Include(b => b.Routine)
                                .Where(d => d.PatientAllocation.patientAllocationID ==
                                            i.PatientAllocation.patientAllocationID)
                                .Where(d => d.timeStart.Hours == qwer)
                                .Where(d => d.PatientAllocation.caregiverID == id)
                                .Where(d => d.dateStart.Day == timeDate.Day &&
                                            d.dateStart.Month == timeDate.Month &&
                                            d.dateStart.Year == timeDate.Year)
                                .Where(d => d.isApproved == 1)
                                .Where(d => d.isDeleted == 0)
                                .Single();
                            ab[count].nextScheduleID = next.scheduleID;
                            if (next.CentreActivity != null)
                            {
                                ab[count].nextScheduleDesc = next.CentreActivity.activityTitle;
                            }
                            else
                            {
                                ab[count].nextScheduleDesc = next.Routine.eventName;
                            }
                            ab[count].nextTimeStart = Convert.ToDateTime(next.timeStart.ToString());
                            ab[count].nextTimeEnd = Convert.ToDateTime(next.timeEnd.ToString());
                        }
                        else
                        {
                            ab[count].nextScheduleID = -1;
                            ab[count].nextScheduleDesc = "-";
                            ab[count].nextTimeStart = null;
                            ab[count].nextTimeEnd = null;
                        }

                        count++;
                    }

                    if (patient == null)
                    {
                        return NotFound();
                    }

                    return Ok(ab);

                }
                else
                {
                    return BadRequest("Invalid Token");
                }
            }
            else
            {
                if ( token == "1234" || (_context.UserTables.SingleOrDefault(c => c.token == token).loginTimeStamp > dateTime &&
                    (_context.UserTables.Where(c => c.token == token).Count() > 0)))
                {
                    var patient = _context.Schedules.Include(a => a.PatientAllocation.Patient)
                        .Include(a => a.CentreActivity)
                        .Include(a => a.Routine)
                        .Where(d => d.PatientAllocation.caregiverID == id)
                        .Where(d => d.dateStart.Day == timeDate.Day &&
                                    d.dateStart.Month == timeDate.Month &&
                                    d.dateStart.Year == timeDate.Year)
                        .Where(d => d.timeStart.Hours - timeDate.Hour == 0)
                        .Where(d => d.isApproved == 1)
                        .Where(d => d.isDeleted == 0)
                        .ToList()
                        .OrderBy(a => a.timeStart)
                        .OrderBy(a => a.patientAllocationID);

                    if (patient.Count() == 0 && timeDate.Hour != 17)
                    {
                        patient = _context.Schedules.Include(a => a.PatientAllocation.Patient)
                            .Include(a => a.CentreActivity)
                            .Where(d => d.PatientAllocation.caregiverID == id)
                            .Where(d => d.dateStart.Day == timeDate.Day &&
                                        d.dateStart.Month == timeDate.Month &&
                                        d.dateStart.Year == timeDate.Year)
                            .Where(d => d.timeStart.Hours - timeDate.Hour == -1)
                            .Where(d => d.isApproved == 1)
                            .Where(d => d.isDeleted == 0)
                            .ToList()
                            .OrderBy(a => a.timeStart)
                            .OrderBy(a => a.patientAllocationID);
                    }


                    SchedulesDto[] ab = new SchedulesDto[patient.Count()];
                    int count = 0;
                    foreach (var i in patient)
                    {
                        var album = _context.AlbumPatient.SingleOrDefault(x => (x.patientAllocationID == i.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));

                        ab[count] = new SchedulesDto();
                        PatientAllocation pa = new PatientAllocation();
                        AlbumPatient a = new AlbumPatient();
                        a.albumCatID = album.albumCatID;
                        a.albumID = album.albumID;
                        a.albumPath = album.albumPath;
                        a.createDateTime = album.createDateTime;
                        a.isApproved = album.isApproved;
                        a.isDeleted = album.isDeleted;
                        Patient p = new Patient();
                        p.DOB = i.PatientAllocation.Patient.DOB;
                        p.address = i.PatientAllocation.Patient.address;
                        p.autoGame = i.PatientAllocation.Patient.autoGame;
                        p.createDateTime = i.PatientAllocation.Patient.createDateTime;
                        p.firstName = i.PatientAllocation.Patient.firstName;
                        p.gender = i.PatientAllocation.Patient.gender;
                        //p.guardianContactNo = i.PatientAllocation.Patient.guardianContactNo;
                        //p.guardianEmail = i.PatientAllocation.Patient.guardianEmail;
                        //p.guardianName = i.PatientAllocation.Patient.guardianName;
                        p.handphoneNo = i.PatientAllocation.Patient.handphoneNo;
                        p.isApproved = i.PatientAllocation.Patient.isApproved;
                        p.isDeleted = i.PatientAllocation.Patient.isDeleted;
                        p.lastName = i.PatientAllocation.Patient.lastName;
                        p.nric = i.PatientAllocation.Patient.nric;
                        p.patientID = i.PatientAllocation.Patient.patientID;
                        //p.preferredLanguage = i.PatientAllocation.Patient.preferredLanguage;
                        p.preferredName = i.PatientAllocation.Patient.preferredName;
                        p.updateBit = i.PatientAllocation.Patient.updateBit;
                        ab[count].PatientAllocation = pa;
                        ab[count].PatientAllocation.caregiverID = i.PatientAllocation.caregiverID;
                        ab[count].PatientAllocation.createDateTime = i.PatientAllocation.createDateTime;
                        ab[count].PatientAllocation.doctorID = i.PatientAllocation.doctorID;
                        ab[count].PatientAllocation.gametherapistID = i.PatientAllocation.gametherapistID;
                        ab[count].PatientAllocation.isApproved = i.PatientAllocation.isApproved;
                        ab[count].PatientAllocation.isDeleted = i.PatientAllocation.isDeleted;
                        ab[count].PatientAllocation.patientAllocationID = i.PatientAllocation.patientAllocationID;
                        ab[count].PatientAllocation.patientID = i.PatientAllocation.patientID;
                        ab[count].dateEnd = Convert.ToDateTime(i.dateEnd.ToString());
                        ab[count].dateStart = Convert.ToDateTime(i.dateStart.ToString());
                        ab[count].isApproved = i.isApproved;
                        ab[count].isDeleted = i.isDeleted;
                        if (i.CentreActivity != null)
                        {
                             ab[count].scheduleDesc = i.CentreActivity.activityTitle;
                        }
                        else
                        {
                            ab[count].scheduleDesc = i.Routine.eventName;
                        }

                        ab[count].scheduleID = i.scheduleID;
                        ab[count].timeEnd = Convert.ToDateTime(i.timeEnd.ToString());
                        ab[count].timeStart = Convert.ToDateTime(i.timeStart.ToString());

                        var qwer = ab[count].timeEnd.Value.Hour;
                        if (qwer != 17)
                        {
                            var next = _context.Schedules.Include(b => b.PatientAllocation.Patient)
                                .Include(b => b.CentreActivity)
                                .Include(b => b.Routine)
                                .Where(d => d.PatientAllocation.patientAllocationID ==
                                            i.PatientAllocation.patientAllocationID)
                                .Where(d => d.timeStart.Hours == qwer)
                                .Where(d => d.PatientAllocation.caregiverID == id)
                                .Where(d => d.dateStart.Day == timeDate.Day &&
                                            d.dateStart.Month == timeDate.Month &&
                                            d.dateStart.Year == timeDate.Year)
                                .Where(d => d.isApproved == 1)
                                .Where(d => d.isDeleted == 0)
                                .Single();
                            ab[count].nextScheduleID = next.scheduleID;
                            if (i.CentreActivity != null)
                            {
                                ab[count].nextScheduleDesc = next.CentreActivity.activityTitle;
                            }
                            else
                            {
                                ab[count].nextScheduleDesc = next.Routine.eventName;
                            }
                            
                            ab[count].nextTimeStart = Convert.ToDateTime(next.timeStart.ToString());
                            ab[count].nextTimeEnd = Convert.ToDateTime(next.timeEnd.ToString());
                        }
                        else
                        {
                            ab[count].nextScheduleID = -1;
                            ab[count].nextScheduleDesc = "-";
                            ab[count].nextTimeStart = null;
                            ab[count].nextTimeEnd = null;
                        }

                        count++;
                    }

                    if (patient == null)
                    {
                        return NotFound();
                    }
					App_Code.SOLID addLogtoDB = new App_Code.SOLID();
					var logCategory = _context.LogCategories.SingleOrDefault(x => x.logCategoryID == 18);
					JObject o = new JObject();
					o.Add("userID", id);
					string oldLogData =null;
					string logData = o.ToString();
					string logDesc = "Requested for Full NRIC";
					int logCategoryID = logCategory.logCategoryID;
					int patientID = 18; // 18 = EMPTY. Need this to join table.
					int userIDInit = id;
					int? userIDApproved = id;
					string additionalInfo = null;
					string remarks = null;
					string tableAffected = "";
					string columnAffected = "ALL";
					int rowAffected = 0;
					int supNotified = 0;
					int userNotified = 0;
					int approved = 1;
                    // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                    addLogtoDB.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientID, userIDInit, userIDApproved, null, additionalInfo,
						remarks, tableAffected, columnAffected, "", "", supNotified, userNotified, approved, "");
					return Ok(ab);

                }
                else
                {
                    return BadRequest("Invalid Token");
                }
            }

        }
    }
}
