using ThreeVR.Libraries.Common.Video;
using ThreeVR.Libraries.Common.WebSocket;

namespace WebAPI_Server.Tests.Extensions
{
    public static class Extension
    {
        public static NoVideoType GetNoVideoTypeByExtStorageFileStatus(this ExternalStorageFileStatus storageFileStatus)
        {
            switch (storageFileStatus)
            {
                case ExternalStorageFileStatus.Offline:
                    return NoVideoType.TapeOffline;
                case ExternalStorageFileStatus.Loading:
                    return NoVideoType.TapeLoading;
                case ExternalStorageFileStatus.LoadingPend:
                    return NoVideoType.TapePending;
                case ExternalStorageFileStatus.Tape:
                case ExternalStorageFileStatus.Archive:
                    return NoVideoType.Taped;
                case ExternalStorageFileStatus.Vault:
                    return NoVideoType.TapeValuted;
                default:
                    return NoVideoType.NoStoredVideo;
            }
        }
    }
}
