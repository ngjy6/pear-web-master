using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NTU_FYP_REBUILD_17.Controllers.Api
{
    public class HolidayExperienceController : ApiController
    {
        [Route("api/HolidayExperience/GetHolidayExperience")]
        [HttpGet]
        public HttpResponseMessage GetHolidayExperience()
        {

            ApplicationDbContext _context = new ApplicationDbContext(); ;
           
            JArray jarrayAlbum = new JArray();
            JObject o = new JObject();
            JObject albumArrayOj = new JObject();
            var albumPath = _context.HolidayExperiences.ToList();

            for (int i = 0; i < albumPath.Count(); i++)
            {
                o = new JObject();
                o["HolidayExpID"] = albumPath[i].holidayExpID;
                o["Country"] = _context.ListCountries.SingleOrDefault(x => (x.list_countryID == albumPath[i].countryID)).value;
                o["SocialHistoryID"] = albumPath[i].socialHistoryID;
                
                // var albumCat = _context.AlbumCategories.SingleOrDefault(x => x.albumCatID == albumPath[i].albumCatID);


                jarrayAlbum.Add(o);
            }

            albumArrayOj["AlbumCat"] = jarrayAlbum;

            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(albumArrayOj);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}
