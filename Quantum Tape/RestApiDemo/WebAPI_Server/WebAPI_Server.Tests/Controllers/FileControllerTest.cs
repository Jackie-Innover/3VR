using System;
using System.ComponentModel;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreeVR.Libraries.Common.Extensions;
using ThreeVR.Libraries.Common.Video;
using ThreeVR.Libraries.Common.WebSocket;
using ThreeVR.Libraries.RestApiUtilities;

namespace WebAPI_Server.Tests.Controllers
{
    [TestClass]
    public class FileControllerTest
    {

        private const string ManagedFolderPath = @"Z:\Managed";

        //[Serializable]
        //[Flags]
        //public enum ExternalStorageFileStatus
        //{
        //    [System.ComponentModel.Description("NotExist")]
        //    NotExist = 1,
        //    [System.ComponentModel.Description("Disk")]
        //    Disk = 2,
        //    [System.ComponentModel.Description("Tape API server went offline")]
        //    Offline = 4,
        //    [System.ComponentModel.Description("Loading from Tape is pending")]
        //    LoadingPend = 16,
        //    [System.ComponentModel.Description("File is loading from Tape")]
        //    Loading = 32,
        //    [System.ComponentModel.Description("File is in Tape")]
        //    Tape = 64,
        //    [System.ComponentModel.Description("File is in Tape")]
        //    Archive = 128,
        //    [System.ComponentModel.Description("File is in Vault")]
        //    Vault = 256,
        //    Other = 512
        //}

        private ExternalStorageFileStatus ParseLocationType(string filelocationValue)
        {
            string[] locationStrings = filelocationValue.Split(new[] { "AND" }, StringSplitOptions.RemoveEmptyEntries);
            ExternalStorageFileStatus fileLocationType = ExternalStorageFileStatus.NotExist;
            foreach (string item in locationStrings)
            {
                ExternalStorageFileStatus fileLocation;
                if (Enum.TryParse(item.Trim(), true, out fileLocation))
                {
                    fileLocationType = fileLocationType | fileLocation;
                }
                else
                {
                    /**
                     * For can not parse value, we assume the file is in Tape.
                     * For the current V2 tape restful service, file location will list below values:
                     * 1. Disk
                     * 2. Archive/Tape
                     * 3. Disk and Archive/Tape
                     */
                    fileLocationType = fileLocationType | ExternalStorageFileStatus.Tape;
                }
            }
            fileLocationType = fileLocationType & (~ExternalStorageFileStatus.NotExist);
            return fileLocationType;
        }

        [TestMethod]
        public void GetFsFileInfo()
        {
            //FileController fileController = new FileController();
            //fileController.GetFsFileInfo("abc.txt", "xml", "jackie", "jackie");

            //TapeRestApiHelper tapeRestApiHelper = new TapeRestApiHelper("http://localhost:42789/api/", "unmanaged", "techrep", "3MeDeee");
            ExternalStorageFileStatus status = ParseLocationType("Disk AND Tape");
            NoVideoType noVideoType = status.GetNoVideoTypeByExtStorageFileStatus();
            Console.WriteLine(noVideoType);
            Console.WriteLine(status);
        }

        [TestMethod]
        public void GetPackageFilePath()
        {
            string filePath = "/tape/demo/managed/2016_03_07/abc.tar";
            Console.WriteLine(filePath);

            Console.WriteLine(Path.GetFileName(filePath));
            Console.WriteLine(Path.GetRandomFileName());
            Console.WriteLine(Path.GetTempFileName());
            Console.WriteLine(Path.GetTempPath());
            Console.WriteLine(Path.GetDirectoryName(filePath));
            Console.WriteLine(Path.GetFileName(Path.GetDirectoryName(filePath)));

            FileInfo fileInfo=new FileInfo(filePath);
            Console.WriteLine("*****************************************");
            Console.WriteLine(fileInfo.Name);
            Console.WriteLine(fileInfo.DirectoryName);
            Console.WriteLine(fileInfo.Directory.Name);
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
    }
}
