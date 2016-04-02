using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TapeRestApi.Models
{

    public class MediaInfoResponse
    {
        public static readonly MediaInfoResponse DefaultMediaInfoResponse =new MediaInfoResponse
        {
            MediaInfos = new []
            {
                MediaInfo.DefaultMediaInfo
            },
            Statuses = new[]
            {
                new Status
                    {
                        StatusCode = "FS0000",
                        StatusNumber = 0,
                        StatusText = "Command Successful."
                    }
            }
        };

        [JsonProperty("medias")]
        public MediaInfo[] MediaInfos { get; set; }

        [JsonProperty("statuses")]
        public Status[] Statuses { get; set; }
    }

    public class MediaInfo
    {
        public static readonly MediaInfo DefaultMediaInfo = new MediaInfo
        {
            MediaId = "000001",
            LocationState = "Archive",
            CurrentArchive = "Archive1",
        };

        [JsonProperty("mediaId")]
        public string MediaId { get; set; }

        [JsonProperty("locationState")]
        public string LocationState { get; set; }

        [JsonProperty("currentArchive")]
        public string CurrentArchive { get; set; }

    }
}
