using System;
using System.Runtime.Serialization;

namespace iSHARE.EmailClient
{
    [Serializable]
    public class EmailClientException: Exception
    {
        public EmailClientException()
        { 
        }

        public EmailClientException(string message) : base(message)
        {
        }

        public EmailClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EmailClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
