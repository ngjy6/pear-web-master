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

namespace NTU_FYP_REBUILD_17.Controllers.Api
{
    public class GenerateScheduleController : ApiController
    {

        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();

        public GenerateScheduleController()
        {
            _context = new ApplicationDbContext();
        }

        //http://localhost:50217/api/GenerateSchedule/displayPatients_JSONString?token=1234
        [HttpGet]
        [Route("api/GenerateSchedule/displayPatients_JSONString")]
        public HttpResponseMessage displayPatients_JSONString(string token)
        {
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            JObject overallJObj = new JObject();
            JArray patientJArray = new JArray();

            var patientList = _context.Patients.Where(x => x.isApproved == 1 && x.isDeleted == 0).ToList();

            foreach (var viewPatient in patientList)
            {
                JArray jarrayAlbum = new JArray();
                JObject patientJObj = new JObject();

                var patient = _context.Patients.SingleOrDefault((x => (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted == 0)));
                if (patient == null)
                    return null;

                //patientJObj["NRIC"] = patient.nric.Remove(1, 4).Insert(1, "xxxx");
                //patientJObj["firstName"] = patient.firstName;
                //patientJObj["lastName"] = patient.lastName;

                patientJObj["Name"] = patient.lastName + " " + patient.firstName;

                var albumPath = (from pa in _context.PatientAllocations
                                 join p in _context.Patients on pa.patientID equals p.patientID
                                 join a in _context.AlbumPatient on pa.patientAllocationID equals a.patientAllocationID
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

                patientJArray.Add(patientJObj);
            }

            overallJObj["Patient"] = patientJArray;

            string output = JsonConvert.SerializeObject(overallJObj);
            string json = overallJObj.ToString(Newtonsoft.Json.Formatting.None);
            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(overallJObj);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}