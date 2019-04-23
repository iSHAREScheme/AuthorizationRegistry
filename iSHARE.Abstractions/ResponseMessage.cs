using Newtonsoft.Json;

namespace iSHARE.Abstractions
{
    public class ResponseMessage
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        public ResponseMessage(string message)
        {
            Message = message;
        }
    }
}
