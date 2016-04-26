using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TapeRestApi.Models
{

    public class ArchiveResponse
    {
        [JsonProperty("archives")]
        public ArchiveInfo[] ArchiveInfos { get; set; }
    }

    public enum ArchiveType
    {
        [Description("Tape")]
        SCSI,
        [Description("Vault")]
        Stage
    }

    public class ArchiveInfo
    {
        public static ArchiveInfo DefaultArchiveInfo = new ArchiveInfo
        {
            Name = "Archive1",
            Type = ArchiveType.SCSI
        };

        [JsonProperty("archiveName")]
        public string Name { get; set; }

        [JsonProperty("archiveType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ArchiveType Type { get; set; }
    }

    public class FullArchiveInfo
    {
        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("actionRequired")]
        public string ActionRequired { get; set; }

        [JsonProperty("currentArchiveName")]
        public string CurrentArchiveName { get; set; }

        [JsonProperty("currentArchiveType")]
        public string CurrentArchiveType { get; set; }

        [JsonProperty("pendingArchiveName")]
        public string PendingArchiveName { get; set; }

        [JsonProperty("pendingArchiveType")]
        public string PendingArchiveType { get; set; }

        [JsonProperty("vaulted")]
        public bool Vaulted { get; set; }
    }
}
