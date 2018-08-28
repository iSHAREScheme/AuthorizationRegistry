using System;
using System.Runtime.Serialization;

namespace NLIP.iShare.AuthorizationRegistry.Client
{
    public class AuthorizationRegistryClientException : Exception
    {
        public AuthorizationRegistryClientException()
        {
        }

        public AuthorizationRegistryClientException(string message) : base(message)
        {
        }

        public AuthorizationRegistryClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public AuthorizationRegistryClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
