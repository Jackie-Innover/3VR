﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using TapeRestApi.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TapeRestApi.Controllers
{
    [Route("api/[controller]")]
    public class ArchiveController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("vsarchiveqry")]
        public ArchiveResponse Get(string archive, string format)
        {

            ArchiveResponse archiveResponse = new ArchiveResponse
            {
                ArchiveInfos = new[]
                {
                    new ArchiveInfo
                    {
                        Name = archive,
                        Type = archive == "Archive1" ? ArchiveType.SCSI : ArchiveType.Stage
                    }
                }
            };

            return archiveResponse;
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
