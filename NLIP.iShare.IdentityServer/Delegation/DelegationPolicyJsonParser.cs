using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NLIP.iShare.IdentityServer.Delegation
{
    public class DelegationPolicyJsonParser
    {
        private readonly JObject _policyJson;

        public DelegationPolicyJsonParser(string json)
        {
            try
            {
                _policyJson = JObject.Parse(json);
            }
            catch (JsonReaderException jre)
            {
                throw new DelegationPolicyFormatException("JSON format is incorrect.", jre);
            }

            if (!_policyJson.HasValues)
            {
                throw new DelegationPolicyFormatException("The delegation policy doesn't have contents.");
            }
        }

        public string PolicyIssuer => GetValue("delegationEvidence.policyIssuer");

        public string AccessSubject => GetValue("delegationEvidence.target.accessSubject");

        private string GetValue(string path)
        {
            var res = _policyJson.SelectToken(path, false);

            return res?.ToString();
        }
    }
}
