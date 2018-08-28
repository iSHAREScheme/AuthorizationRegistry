using System;
using System.Runtime.Serialization;

namespace NLIP.iShare.EntityFramework
{
    [Serializable]
    public class EFExtensionsException : Exception
    {
        public EFExtensionsException()
        {
        }

        public EFExtensionsException(string message) : base(message)
        {
        }

        public EFExtensionsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EFExtensionsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}