using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.App_Code;
using NTU_FYP_REBUILD_17.Dtos;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;
using System.Threading.Tasks;

namespace NTU_FYP_REBUILD_17.Controllers.Api
{
    public class AccountPageController : ApiController
    {
        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        Controllers.Synchronization.AccountMethod account = new Controllers.Synchronization.AccountMethod();
        Controllers.Synchronization.ScheduleMethod scheduler = new Controllers.Synchronization.ScheduleMethod();

        public AccountPageController()
        {
            _context = new ApplicationDbContext();
        }

        //https://localhost:44300/api/AccountPage/GetUserDetails_JSONString?token=1234&userID=3
        [HttpGet]
        [Route("api/AccountPage/GetUserDetails_JSONString")]
        public HttpResponseMessage GetUserDetails_JSONString(string token, int userID)
        {
            var response = this.Request.CreateResponse(HttpStatusCode.NotFound);

            ApplicationUser selectedUser = shortcutMethod.getUserDetails(token, userID);
            string userType = shortcutMethod.getUserType(token, userID);

            string userValidationResult = shortcutMethod.checkValidUser(selectedUser, userType);
            if (userValidationResult != "valid")
            {
                response.Content = new StringContent(userValidationResult, System.Text.Encoding.UTF8, "application/json");
                return response;
            }

            JObject userJObj = account.getUserAccount(selectedUser, userType);

            string jsonString = JsonConvert.SerializeObject(userJObj);
            response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        //https://localhost:44300/api/AccountPage/GetAccountDetails_JSONString?token=1234&userID=3
        [HttpGet]
        [Route("api/AccountPage/GetAccountDetails_JSONString")]
        public HttpResponseMessage GetAccountDetails_JSONString(string token, int userID)
        {
            var response = this.Request.CreateResponse(HttpStatusCode.NotFound);

            ApplicationUser selectedUser = shortcutMethod.getUserDetails(token, userID);
            string userType = shortcutMethod.getUserType(token, userID);

            string userValidationResult = shortcutMethod.checkValidUser(selectedUser, userType);
            if (userValidationResult != "valid")
            {
                response.Content = new StringContent(userValidationResult, System.Text.Encoding.UTF8, "application/json");
                return response;
            }

            JObject userJObj = account.getUserAccount(selectedUser, userType);

            string jsonString = JsonConvert.SerializeObject(userJObj);
            response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        /*
        {
            "token": "1234",
            "userID": 2,
            "email": "doctor@pear.com",
            "preferredName": "Daniel",
            "firstName": "Daniel",
            "lastName": "Lee",
            "address": "10 Biopolis Way #03-03/04 CHROMOS, BIOPOLIS, 138670, Singapore",
            "handphoneNo": "95554791",
            "officeNo": "64844928",
        }
        */

        //https://localhost:44300/api/AccountPage/updateAccountDetails
        [HttpPut]
        [Route("api/AccountPage/updateAccountDetails")]
        public string updateAccountDetails(HttpRequestMessage bodyResult)
        {
            string resultString = bodyResult.Content.ReadAsStringAsync().Result;
            if (resultString == null)
                return "empty body string";

            JObject resultJObject = JObject.Parse(resultString);

            string token = (string)resultJObject.SelectToken("token");
            int userID = (int)resultJObject.SelectToken("userID");
            ApplicationUser selectedUser = shortcutMethod.getUserDetails(token, userID);
            string userType = shortcutMethod.getUserType(token, userID);

            string userValidationResult = shortcutMethod.checkValidUser(selectedUser, userType);
            if (userValidationResult != "valid")
            {
                return userValidationResult;
            }

            string email = null;
            string preferredName = null;
            string firstName = null;
            string lastName = null;
            string address = null;
            string handphoneNo = null;
            string officeNo = null;

            if ((string)resultJObject.SelectToken("email") != null)
                email = (string)resultJObject.SelectToken("email");

            if ((string)resultJObject.SelectToken("preferredName") != null)
                preferredName = (string)resultJObject.SelectToken("preferredName");

            if ((string)resultJObject.SelectToken("firstName") != null)
                firstName = (string)resultJObject.SelectToken("firstName");

            if ((string)resultJObject.SelectToken("lastName") != null)
                lastName = (string)resultJObject.SelectToken("lastName");

            if ((string)resultJObject.SelectToken("address") != null)
                address = (string)resultJObject.SelectToken("address");

            if ((string)resultJObject.SelectToken("handphoneNo") != null)
                handphoneNo = (string)resultJObject.SelectToken("handphoneNo");

            if ((string)resultJObject.SelectToken("officeNo") != null)
                officeNo = (string)resultJObject.SelectToken("officeNo");

            //updateAccount(int userInitID, int userID, int userTypeID, string preferredName, string firstName, string lastName, string email, string address, string handphoneNo, string? officeNo, int? allowNotification, int? isLocked, string lockReason)
            string result = account.updateAccount(userID, userID, selectedUser.userTypeID, null, preferredName, firstName, lastName, email, address, handphoneNo, officeNo, null, null, null);

            return result;
        }

    }
}