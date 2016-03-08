using System;
using System.Collections.Generic;
using System.IO;

namespace WebSocketClientDemo
{
    public class DataManager
    {
        public byte[] PackageData { get; }

        public readonly IList<byte[]> PackageDataList = new List<byte[]>();

        public DataManager()
        {
            int packageLength = 512 * 1024;
            PackageData = new byte[packageLength];

            if (!File.Exists("demo.avi"))
            {
                return;
            }

            using (FileStream fileStream = new FileStream("demo.avi", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fileStream.Read(PackageData, 0, PackageData.Length);
            }

            int dataLength = 0;
            bool isPackageDataFill = false;
            using (FileStream fileStream = new FileStream("demo.avi", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                long diff;
                do
                {
                    byte[] packageData = new byte[packageLength];

                    dataLength += fileStream.Read(packageData, 0, packageData.Length);
                    if (!isPackageDataFill)
                    {
                        Buffer.BlockCopy(packageData, 0, PackageData, 0, PackageData.Length);
                        isPackageDataFill = true;
                    }

                    diff = fileStream.Length - dataLength;
                    if (diff < packageLength)
                    {
                        packageLength = (int)diff;
                    }
                    PackageDataList.Add(packageData);
                } while (diff > 0);
            }
        }
    }
}
