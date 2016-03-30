using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TapeRestApi.Models
{

    public class JobResponse
    {
        public JobResponse()
        {
            Message = "Job has been successfully submitted.";
        }

        [JsonProperty("job")]
        public int JobId { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class JobInfoResponse
    {
        [JsonProperty("jobInfo")]
        public JobInfo JobInfo { get; set; }

        [JsonProperty("response")]
        public Response Response { get; set; }

    }

    public class JobInfo
    {
        [JsonProperty("job")]
        public int JobId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("state")]
        public JobState State { get; set; }
        [JsonProperty("exitcode")]
        public int ExitCode { get; set; }
        [JsonProperty("datecompleted")]
        public DateTime DateCompleted { get; set; }

        [JsonIgnore]
        public DateTime BeginDateTime { get; set; }

        [JsonIgnore]
        public string SourceFilePath { get; set; }
    }

    public class JobListResponse
    {
        public JobListResponse()
        {
            JobList = new List<JobInfoResponse>();
        }

        [JsonProperty("jobList")]
        public List<JobInfoResponse> JobList { get; set; }
    }
}
