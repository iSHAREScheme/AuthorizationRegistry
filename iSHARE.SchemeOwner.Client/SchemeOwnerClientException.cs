using System;
using System.Runtime.Serialization;

namespace iSHARE.SchemeOwner.Client
{
    [Serializable]
    public class SchemeOwnerClientException : Exception
    {
        public SchemeOwnerClientException()
        {
        }

        public SchemeOwnerClientException(string message) : base(message)
        {
        }

        public SchemeOwnerClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SchemeOwnerClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}