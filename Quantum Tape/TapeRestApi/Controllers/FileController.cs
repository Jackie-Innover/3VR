using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using TapeRestApi.Config;
using TapeRestApi.Helpers;
using TapeRestApi.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TapeRestApi.Controllers
{
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        // GET: api/values
        [HttpGet]
        public string Get()
        {
            string message = @"file/fsfileinfo?file=/2016_02_01/894b8eda-8e28-45a1-9d68-100000000001_2016_02_01_poleevent.00.tar&userName=Jackie&password=Jackie&format=json" + Environment.NewLine +
                             "file/fsretrieve/new?file=/2016_02_01/894b8eda-8e28-45a1-9d68-100000000001_2016_02_01_poleevent.00.tar&newfile=jackie.tar&userName=Jackie&password=Jackie&format=json";

            return new HtmlString(message).ToString();
        }

        [HttpGet("fsfileinfo")]
        public FileInfoResponse Get(string file, string userName, string password, string format, bool archiveInfo)
        {
            FsFileInfo fileInfo = TapeFileHelper.GetFileInfoItem(file, archiveInfo);

            FileInfoResponse response = new FileInfoResponse
            {
                FileInfos = new List<FsFileInfo>
                {
                    fileInfo
                }
            };

            return response;
        }

        [HttpGet("fsretrieve/new")]
        public object Get(string file, string mode, string newFile, string userName, string password, string format)
        {
            CommandMode commandMode;
            if (!Enum.TryParse(mode, true, out commandMode))
            {
                commandMode = CommandMode.Sync;
            }

            FileInfo fileInfo = new FileInfo(file);
            string managedFilePath = Path.Combine(GlobalSetting.ManagedFolderPath, fileInfo.Directory.Name, fileInfo.Name);
            string unmanagedCacheFilePath = Path.Combine(GlobalSetting.UnManagedCacheFolderPath, fileInfo.Name);

            MoveFile(unmanagedCacheFilePath, managedFilePath);

            switch (commandMode)
            {
                case CommandMode.Async:
                    return AsyncRetrieveFile(managedFilePath, unmanagedCacheFilePath);
                default:
                    return SyncRetrieveFile(managedFilePath, unmanagedCacheFilePath);
            }

        }

        private static void MoveFile(string unmanagedCacheFilePath, string managedFilePath)
        {

            if (System.IO.File.Exists(unmanagedCacheFilePath))
            {
                System.IO.File.Delete(unmanagedCacheFilePath);
            }

            if (!Directory.Exists(GlobalSetting.UnManagedCacheFolderPath))
            {
                Directory.CreateDirectory(GlobalSetting.UnManagedCacheFolderPath);
            }

            try
            {
                System.IO.File.Copy(managedFilePath, unmanagedCacheFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private JobResponse AsyncRetrieveFile(string managedFilePath, string unmanagedCacheFilePath)
        {
            return JobHelper.AddJob(managedFilePath, unmanagedCacheFilePath);

        }

        private FileRetrieveResponse SyncRetrieveFile(string managedFilePath, string unmanagedCacheFilePath)
        {

            Thread.Sleep(3000);

            Status status = new Status
            {
                StatusCode = "FS0347",
                StatusText = "retrieve success"
            };

            FileRetrieveResponse response = new FileRetrieveResponse
            {
                Statuses = new List<Status>
                {
                    status
                }
            };

            return response;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }     

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }        
    }
}
