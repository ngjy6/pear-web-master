using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace NTU_FYP_REBUILD_17.Controllers
{
    public class AlbumController : ApiController
    {
        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();

        public AlbumController()
        {
            _context = new ApplicationDbContext();
        }


        //Get all photo of the patient from album
        //https://localhost:44300/api/Album/GetAlbum?token=1234&patientID=3
        [Route("api/Album/GetAlbum")]

        [HttpGet]
        public HttpResponseMessage GetAlbum(string token, int patientID)
        {

            _context = new ApplicationDbContext();

            JArray jarrayAlbum = new JArray();
            JObject o = new JObject();
            JObject albumArrayOj = new JObject();

            //Get all the photos via userId
            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            var album = _context.AlbumPatient.Where(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.isApproved == 1 && x.isDeleted == 0)).ToList();

            for (int i = 0; i < album.Count(); i++)
            {
                if (album[i].albumPath != null)
                {
                    o = new JObject();
                    int id = album[i].albumCatID;
                    var catName = _context.AlbumCategories.Single(Y => Y.albumCatID == id);
                    // o["catId"] = albumCat.albumCatID;
                    // o["catName"] = albumPath[i].albumCatName;
                    o["albumId"] = album[i].albumID;
                    o["albumPath"] = album[i].albumPath;
                    o["albumcatID"] = album[i].albumCatID;
                    o["patientId"] = patientAllocation.patientID;

                    o["albumcatName"] = catName.albumCatName;

                    // var albumCat = _context.AlbumCategories.SingleOrDefault(x => x.albumCatID == albumPath[i].albumCatID);
                }

                jarrayAlbum.Add(o);
            }

            albumArrayOj["Album"] = jarrayAlbum;

            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(albumArrayOj);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }


        //Get all photos by category
        [Route("api/Album/GetAlbumByCategory")]
        [HttpGet]
        public HttpResponseMessage GetAlbumByCategory(string token, int patientID, int catId)
        {

            ApplicationDbContext _context = new ApplicationDbContext();
            JArray jarrayAlbum = new JArray();
            JObject o = new JObject();
            JObject albumArrayOj = new JObject();

            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            var albumPath = _context.AlbumPatient.Where(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.albumCatID == catId && x.isApproved == 1 && x.isDeleted == 0)).ToList();


            for (int i = 0; i < albumPath.Count(); i++)
            {
                if (albumPath[i].albumPath != null)
                {
                    o = new JObject();
                    o["albumId"] = albumPath[i].albumID;
                    o["albumPath"] = albumPath[i].albumPath;
                    o["albumcatID"] = albumPath[i].albumCatID;

                }

                jarrayAlbum.Add(o);
            }

            albumArrayOj["Album"] = jarrayAlbum;

            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(albumArrayOj);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }






        //Get all album category
        //For own reference for the data in AlbumCategories table
        [Route("api/Album/GetAlbumCat")]

        [HttpGet]
        public HttpResponseMessage GetAlbumCat()
        {
            ApplicationDbContext _context = new ApplicationDbContext(); ;
            JArray jarrayAlbum = new JArray();
            JObject o = new JObject();
            JObject albumArrayOj = new JObject();
            var albumPath = _context.AlbumCategories.ToList();


            for (int i = 0; i < albumPath.Count(); i++)
            {
                o = new JObject();
                // o["catId"] = albumCat.albumCatID;
                // o["catName"] = albumPath[i].albumCatName;s
                o["albumCatId"] = albumPath[i].albumCatID;
                o["alumcatName"] = albumPath[i].albumCatName;

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

        [Route("api/Album/GetAlbumCatByPatientId")]
        [HttpGet]
        public HttpResponseMessage GetAlbumCatByPatientId(string token, int patientID)
        {

            ApplicationDbContext _context = new ApplicationDbContext(); ;
            JArray jarrayAlbum = new JArray();
            JObject o = new JObject();
            JObject albumArrayOj = new JObject();

            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            var albumList = _context.AlbumPatient.Where(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.albumCatID != 1 && x.isApproved == 1 && x.isDeleted == 0)).ToList();

            var albumPerpatient =
            from patientAlbum in albumList

            group patientAlbum by patientAlbum.albumCatID into albumGroup
            select new
            {
                Team = albumGroup.Key,
                Count = albumGroup.Count(),

            };


            foreach (var albumGroup in albumPerpatient)
            {
                o = new JObject();
                int catId = albumGroup.Team;
                var catName = _context.AlbumCategories.Single(Y => Y.albumCatID == catId && Y.isApproved == 1 && Y.isDeleted == 0);
                var album = _context.AlbumPatient.First(Z => Z.albumCatID == catId && Z.patientAllocationID == patientAllocation.patientAllocationID && Z.isApproved == 1 && Z.isDeleted == 0);

                o["catname"] = catName.albumCatName;
                o["count"] = albumGroup.Count;
                o["CategoryId"] = catId;
                o["imagePath"] = album.albumPath;

                jarrayAlbum.Add(o);
            }



            albumArrayOj["AlbumCat"] = jarrayAlbum;

            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(albumArrayOj);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }


        [Route("api/Album/UploadImage")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> UploadImage()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                int patientId = 0;
                int catId = 0;
                var httpRequest = HttpContext.Current.Request;
                var queryVals = Request.RequestUri.ParseQueryString();
                patientId = int.Parse(queryVals["patientid"]);
                catId = int.Parse(queryVals["catId"]);

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];

                    var personInfo = _context.Patients.Where(x => (x.patientID == patientId && x.isApproved == 1 && x.isDeleted == 0)).SingleOrDefault();

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
                            String path = "~/Image/" + personInfo.nric;
                            DirectoryInfo di = Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));

                            var filePath = HttpContext.Current.Server.MapPath(path + "/" + postedFile.FileName);
                            if (File.Exists(filePath))
                            {
                                return Request.CreateErrorResponse(HttpStatusCode.Created, "duliplate file");

                            }
                            postedFile.SaveAs(filePath);

                        }
                    }

                    var message1 = string.Format("Image Updated Successfully." + patientId);
                    addToAlbumDB(catId, patientId, "/Image/" + personInfo.nric + "/" + postedFile.FileName);

                    return Request.CreateErrorResponse(HttpStatusCode.Created, message1);
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

        public void addToAlbumDB(int catId, int patientId, String albumPath)
        {
            {
                var now = DateTime.Now;
                var insertIntoAlbumTable = new AlbumPatient()
                {
                    albumCatID = catId,
                    albumPath = albumPath,
                    createDateTime = now,
                    isDeleted = 0,
                    isApproved = 1              //For Now

                };
                _context.AlbumPatient.Add(insertIntoAlbumTable);
                _context.SaveChanges();

            }


        }



        public void deletePhoto(int albumId)
        {
            {
                var now = DateTime.Now;
                var album = _context.AlbumPatient.SingleOrDefault(x => (x.albumID == albumId && x.isApproved == 1 && x.isDeleted == 0));
                var filePath = HttpContext.Current.Server.MapPath(album.albumPath);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);


                    album.isDeleted = 1;

                    _context.SaveChanges();
                }



                _context.SaveChanges();




            }


        }

        //Get all photo of the patient from album
        //https://localhost:44300/api/Album/GetAlbumByCatId?token=1234&catId=3&patientID=4
        [Route("api/Album/GetAlbumByCatId")]
        [HttpGet]
        public HttpResponseMessage GetAlbumByCatId(string token, int catId, int patientID)
        {

            _context = new ApplicationDbContext();

            JArray jarrayAlbum = new JArray();
            JObject o = new JObject();
            JObject albumArrayOj = new JObject();

            //Get all the photos via userId
            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            var album = _context.AlbumPatient.Where(x => x.albumCatID == catId && x.patientAllocationID == patientAllocation.patientAllocationID && x.isApproved == 1 && x.isDeleted == 0).ToList();


            for (int i = 0; i < album.Count(); i++)
            {
                if (album[i].albumPath != null)
                {
                    o = new JObject();

                    var catName = _context.AlbumCategories.Single(Y => Y.albumCatID == catId);
                    // o["catId"] = albumCat.albumCatID;
                    // o["catName"] = albumPath[i].albumCatName;
                    o["albumId"] = album[i].albumID;
                    o["albumPath"] = album[i].albumPath;
                    o["albumcatID"] = album[i].albumCatID;
                    o["patientId"] = patientAllocation.patientID;

                    o["albumcatName"] = catName.albumCatName;
                    // var albumCat = _context.AlbumCategories.SingleOrDefault(x => x.albumCatID == albumPath[i].albumCatID);
                }

                jarrayAlbum.Add(o);
            }

            albumArrayOj["Album"] = jarrayAlbum;

            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(albumArrayOj);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }




    }
}
