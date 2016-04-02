using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using TapeRestApi.Helpers;
using TapeRestApi.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TapeRestApi.Controllers
{
    [Route("api/[controller]")]
    public class JobController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/job/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet("info")]
        public object Get(int job, string format, string username, string password)
        {
            JobInfo jobInfo = JobHelper.GetJobInfo(job);
            JobListResponse jobListResponse = new JobListResponse();

            switch (jobInfo.State)
            {
                case JobState.RUNNING:
                    {
                        JobInfoResponse jobInfoResponse = new JobInfoResponse { JobInfo = jobInfo };
                        jobListResponse.JobList.Add(jobInfoResponse);
                    }

                    //return "{\"jobList\":[{\"jobInfo\":{\"job\":\"28\",\"state\":\"RUNNING\"}}]}";
                    return jobListResponse;
                case JobState.ERROR:
                    //TODO: return error response
                    return new {};

                case JobState.COMPLETED:
                    {
                        Response response = new Response
                        {
                            Statuses = new List<Status>
                            {
                                new Status
                                {
                                    StatusCode = "FS0347",
                                    StatusText = string.Format("File {0} has been retrieved.", jobInfo.SourceFilePath)
                                },
                                new Status
                                {
                                    StatusCode = "FS0390",
                                    StatusText = "1 out of 1 retrieves were successful."
                                }
                            }
                        };

                        JobInfoResponse jobInfoResponse = new JobInfoResponse
                        {
                            JobInfo = jobInfo,
                            Response = response
                        };

                        jobListResponse.JobList.Add(jobInfoResponse);
                    }

                    return jobListResponse;

                default:
                    return new { };
            }
        }

        // POST api/job
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/job/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/job/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
