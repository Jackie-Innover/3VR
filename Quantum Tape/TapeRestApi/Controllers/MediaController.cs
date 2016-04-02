using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using TapeRestApi.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TapeRestApi.Controllers
{
    [Route("api/[controller]")]
    public class MediaController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("fsmedinfo")]
        public MediaInfoResponse Get(string media, string format, string username, string password)
        {
            string currentArchive = media == "000000" ? "Vault1" : "Archive1";

            MediaInfoResponse mediaInfoResponse = new MediaInfoResponse
            {
                MediaInfos = new[]
                {
                    new MediaInfo
                    {
                        MediaId = media,
                        LocationState = "Archive",
                        CurrentArchive = currentArchive
                    }
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

            return mediaInfoResponse;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
