using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlliedTimbers.Utilities;

namespace AlliedTimbers.Controllers.API
{
    [Authorize, RoutePrefix("api/media")]
    public class MediaController : Controller
    {
        private static string GetWhatsappFileType(string filePath)
        {
            var mime = MimeMapping.GetMimeMapping(filePath)
               .Split('/')
               .FirstOrDefault()?
               .ToLower();


            switch (mime)
            {
                case "image":
                    return "image";
                case "video":
                    return "video";
                case "audio":
                    return "audio";
                default:
                    return "file";
            }
        }

        [HttpPost]
        public ActionResult Upload(List<HttpPostedFileBase> files)
        {
            var content = new List<dynamic>();


            foreach (HttpPostedFileBase pFile in files)
            {
                var fname = pFile.FileName;
                var file = FileHandler.Upload(pFile);

                var serverFilePath = Server.MapPath(file);
                var fileInfo = new FileInfo(serverFilePath);

                var messageUrl = ResolveServerUrl(file);

                var messageType = GetWhatsappFileType(serverFilePath);

                content.Add(new { fileName = pFile.FileName, messageType, messageUrl, fileSize = fileInfo.Length });
            }

            return Json(content, "application/json");

        }

        public static string ResolveServerUrl(string serverUrl)
        {
            if (serverUrl.IndexOf("://", StringComparison.Ordinal) > -1)
            {
                return serverUrl;
            }

            Uri url = System.Web.HttpContext.Current.Request.Url;
            return url.Scheme + "://" + url.Authority + serverUrl;
        }

    }
}
