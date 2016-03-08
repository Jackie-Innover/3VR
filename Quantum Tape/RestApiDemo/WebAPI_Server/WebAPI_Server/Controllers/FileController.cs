
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using System.Xml.Linq;
using WebAPI_Server.Models;

namespace WebAPI_Server.Controllers
{
    public class FileController : ApiController
    {
        private const string ManagedFolderPath = @"Z:\Managed";
        private const string UnManagedFolderPath = @"Z:\UnManaged";
        private const string UnManagedCacheFolderPath = @"Z:\UnManaged\UnManaged_Cache";

        // GET: api/File
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/File/5
        //public string Get(int id)
        //{
        //    return "value";
        //}
        private FsFileInfo GetNewFsFileInfo(string fileName, string location)
        {
            FsFileInfo fileInfo = new FsFileInfo
            {
                FileName = fileName,
                //Location = "Disk AND Archive",
                Location = location,
                Medias = new List<Media>
                {
                    new Media
                    {
                        MediaId = "tape_demo",
                        Copy = 1
                    }
                }
            };

            return fileInfo;
        }

        [HttpGet]
        public object FsFileInfo(string file, string format, string username, string password)
        {

            string fullFilePath = getAbsoluteFilePath(file);

            string fileLocation = "Vault";

            if (File.Exists(fullFilePath))
            {
                fileLocation = "Archive";
            }

            FsFileInfo fileInfo = GetNewFsFileInfo(file, fileLocation);

            switch (format.ToLower())
            {
                case "json":
                    {
                        FileInfoResponse fileInfoResponse = new FileInfoResponse
                        {
                            FileInfos = new List<FsFileInfo>
                            {
                                fileInfo
                            }
                        };

                        string[] fileNames = file.Split(new[] { "file" }, StringSplitOptions.None);
                        if (fileNames.Length > 2)
                        {
                            for (int i = 2; i < fileNames.Length; i++)
                            {


                                fileInfoResponse.FileInfos.Add(GetNewFsFileInfo("Demo_File.txt", fileLocation));
                            }
                        }

                        return fileInfoResponse;
                    }
                case "xml":
                    {
                        XElement fileInfoElement = new XElement("fileInfos",
                            new XElement("fileInfo", new XElement("fileName", file), new XElement("location", "Archive"),
                                new XElement("medias",
                                    new XElement("media", new XElement("mediaId", "tape_demo"), new XElement("copy", 1)))));
                        string[] fileNames = file.Split(new[] { "file" }, StringSplitOptions.None);
                        if (fileNames.Length > 2)
                        {
                            for (int i = 2; i < fileNames.Length; i++)
                            {
                                fileInfoElement.Add(new XElement("fileInfo", new XElement("fileName", "demo.txt"),
                                    new XElement("location", "Archive"),
                                    new XElement("medias",
                                        new XElement("media", new XElement("mediaId", "tape_demo"), new XElement("copy", 1)))));
                            }
                        }

                        //return Ok(fileInfoElement);
                        fileInfoElement.Save("fileinfo.xml");
                        XElement rootElement = XElement.Load("fileinfo.xml");
                        return rootElement.ToString();
                    }
                //return Ok(rootElement);
                default:
                    //return Ok("Default");
                    return "Default";
            }

            //return fileInfoContent
        }
        
        private string getAbsoluteFilePath(string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                return Path.Combine(ManagedFolderPath, fileInfo.Directory.Name, fileInfo.Name);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        [HttpGet]
        public object FsRetrieve(string file, string format, string username, string password, string newfile)
        {
            FileInfo fileInfo = new FileInfo(file);
            string managedFilePath = Path.Combine(ManagedFolderPath, fileInfo.Directory.Name, fileInfo.Name);
            string unmanagedCacheFilePath = Path.Combine(UnManagedCacheFolderPath, fileInfo.Name);

            if (File.Exists(unmanagedCacheFilePath))
            {
                File.Delete(unmanagedCacheFilePath);
            }

            File.Copy(managedFilePath, unmanagedCacheFilePath);

            Status status = new Status
            {
                StatusCode = "FS0347",
                StatusText = "retrieve success"
            };

            switch (format.ToLower())
            {
                case "json":
                    //var fileInfoContent = JsonConvert.SerializeObject(status);
                    //return Ok(fileInfoContent);
                    return new FileRetrieveResponse
                    {
                        Statuses = new List<Status>
                        {
                            status
                        }
                    };

                case "xml":
                    XElement fileInfoElement = new XElement("statuses",
                        new XElement("status", new XElement("statusCode", "FS0347"), new XElement("statusText", "retrieve success")));


                    //return Ok(fileInfoElement);
                    return fileInfoElement.ToString();
                default:
                    return "default";
            }
        }

        [HttpGet]
        public object DriveCheck(string fileName)
        {
            string response = string.Empty;
            string filePath = Path.Combine(ManagedFolderPath, fileName);

            response += "FileName: " + fileName + "<br />";
            response += "FilePath: " + filePath + "<br />";
            response += "Exists: " + File.Exists(filePath) + "<br />";

            response += "Folder Exists: " + Directory.Exists(ManagedFolderPath);

            return response;
        }

        //// POST: api/File
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/File/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/File/5
        //public void Delete(int id)
        //{
        //}
    }
}
