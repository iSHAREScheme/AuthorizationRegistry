using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NLIP.iShare.SchemeOwner.Client.Models
{
    public class Party
    {
        [JsonProperty("party_id")]
        public string PartyId { get; set; }

        [JsonProperty("party_name")]
        public string Name { get; set; }

        [JsonProperty("adherence")]
        public Adherence Adherence { get; set; }

        [JsonProperty("certifications")]
        public IEnumerable<Certification> Certifications { get; set; }

        [JsonProperty("capability_url")]
        public string CapabilityUrl { get; set; }

        public bool IsValid => Adherence.Status == "Active"
                       && Adherence.StartDate <= DateTime.Now
                       && Adherence.EndDate >= DateTime.Now;
    }

}
