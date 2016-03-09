using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TapeRestApi.Models
{
    //public class FileInfoItem
    //{
    //    public string FileName { get; set; }
    //    public string NewFileName { get; set; }
    //    public string Location { get; set; }
    //    public TapeMedia[] Medias { get; set; }
    //}

    //public class TapeMedia
    //{
    //    public int MediaId { get; set; }
    //    public string MediaName { get; set; }
    //    public int Copy { get; set; }
    //}

    public class FileInfoResponse
    {
        [JsonProperty("fileInfos")]
        public List<FsFileInfo> FileInfos { get; set; }
    }

    public class FileRetrieveResponse
    {
        [JsonProperty("statuses")]
        public List<Status> Statuses { get; set; }
    }
    public class FsFileInfo
    {
        [JsonProperty("fileName")]
        public string FileName { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("medias")]
        public List<Media> Medias { get; set; }
    }

    public class Media
    {
        [JsonProperty("mediaId")]
        public string MediaId { get; set; }
        [JsonProperty("copy")]
        public int Copy { get; set; }
    }

    public class Status
    {
        [JsonProperty("statusCode")]
        public string StatusCode { get; set; }
        [JsonProperty("statusNumber")]
        public int StatusNumber { get; set; }
        [JsonProperty("statusText")]
        public string StatusText { get; set; }
    }

}
