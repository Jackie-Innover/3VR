using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TapeRestApi.Config;
using TapeRestApi.Models;

namespace TapeRestApi.Helpers
{
    public static class TapeFileHelper
    {
        private static string getAbsoluteFilePath(string filePath)
        {
            try
            {
                filePath = filePath.Trim(Path.DirectorySeparatorChar);
                FileInfo fileInfo = new FileInfo(filePath);
                return Path.Combine(GlobalSetting.ManagedFolderPath, fileInfo.Directory.Name, fileInfo.Name);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static string getFileLocation(string filePath)
        {
            string fileLocation = "Vault";

            if (File.Exists(filePath))
            {
                fileLocation = "Archive";
            }

            return fileLocation;
        }

        public static FsFileInfo GetFileInfoItem(string filePath)
        {
            string absoluteFilePath = getAbsoluteFilePath(filePath);
            string fileLocation = getFileLocation(absoluteFilePath);

            FsFileInfo fileInfo = new FsFileInfo
            {
                FileName = filePath,
                //Location = "Disk AND Archive",
                Location = fileLocation,
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
    }
}
