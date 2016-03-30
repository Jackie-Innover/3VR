using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TapeRestApi.Models
{
    public class Response
    {
        [JsonProperty("statuses")]
        public List<Status> Statuses { get; set; }
    }
}
