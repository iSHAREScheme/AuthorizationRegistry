using System;
using Newtonsoft.Json;

namespace NLIP.iShare.SchemeOwner.Client.Models
{
    public class Certification
    {
        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("start_date")]
        public DateTime StartDate { get; set; }

        [JsonProperty("end_date")]
        public DateTime EndDate { get; set; }
    }

}
