using FineArtAPI.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace FineArtAPI.Controllers
{
    public class PostingImageController : ApiController
    {
        FineArtEntities db = new FineArtEntities();
        [HttpPost]
        [Route("Postings/UploadImage")]
        public async Task<HttpResponseMessage> PostUserImage()
        {
            User user = db.Users.Where(u => u.Username == User.Identity.Name).FirstOrDefault();
            if (user.RoleId != 4)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Only student");
            }
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 2048 * 2048 * 1;

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
                            var message = string.Format("Please Upload a file upto 4mb.");
                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {
                            var filePath = HttpContext.Current.Server.MapPath("~/Image/" + postedFile.FileName + extension);
                            if (File.Exists(filePath))
                            {
                                String message = "File name already exists!!! Please choose another name";
                                return Request.CreateResponse(HttpStatusCode.Conflict, message);
                            }
                            postedFile.SaveAs(filePath);
                        }
                    }
                    return Request.CreateErrorResponse(HttpStatusCode.Created, postedFile.FileName); ;
                }
                var res = string.Format("Please Upload a image.");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var res = string.Format("some Message");
                dict.Add("error", res);
                return Request.CreateResponse(HttpStatusCode.NotFound, dict);
            }
        }
    }
}