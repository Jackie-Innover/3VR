using System;
using System.Collections.Generic;
using System.IO;
using BackupLib;

namespace TapeSimulatorConsole
{
    public class NeedSendData
    {
        /// <summary>
        /// Gets or sets the file data.
        /// </summary>
        /// <value>
        /// The file data.
        /// </value>
        public byte[] FileData { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public FilePosition Position { get; set; }
    }

    public sealed class NeedSendDataManager
    {

        #region Fields

        public static readonly NeedSendDataManager Instance = new NeedSendDataManager();
        //private const long TransferBlockSize = 512 * 1024;
        public readonly List<NeedSendData> NeedSendDatas = new List<NeedSendData>();

        #endregion

        #region Life Time

        private NeedSendDataManager()
        {
            PrepareVideoContents();
        }

        #endregion

        #region Private Methods

        private void PrepareVideoContents()
        {
            if (!File.Exists(TapeSimulatorSetting.Instance.VideoFilePath))
            {
                Console.WriteLine("Video file {0} not exist, please check and restart the application.", TapeSimulatorSetting.Instance.VideoFilePath);
                return;
            }
            long transferBlockSize = 1024* TapeSimulatorSetting.Instance.TransferBlockSize;

            using (FileStream fileStream = new FileStream(TapeSimulatorSetting.Instance.VideoFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var position = FilePosition.Head;
                int totalReadSize = 0;
                while (totalReadSize < fileStream.Length)
                {
                    var blockData = new byte[transferBlockSize];
                    if (fileStream.Length - totalReadSize < transferBlockSize)
                    {
                        blockData = new byte[fileStream.Length - totalReadSize];
                    }
                    int curBlockSize = fileStream.Read(blockData, 0, blockData.Length);
                    totalReadSize += curBlockSize;
                    if (position == FilePosition.Head && totalReadSize >= fileStream.Length)
                    {
                        position = FilePosition.Total;
                    }
                    else if (position != FilePosition.Head && totalReadSize >= fileStream.Length)
                    {
                        position = FilePosition.Tail;
                    }
                    var needSendData = new NeedSendData()
                    {
                        FileData = blockData,
                        Position = position
                    };
                    NeedSendDatas.Add(needSendData);
                    position = FilePosition.Remain;
                }
            }
        }

        #endregion
    }
}
