using System;
using System.Runtime.Serialization;

namespace iSHARE.IdentityServer
{
    [Serializable]
    public class LoadCertificateException : Exception
    {
        public LoadCertificateException()
        {
        }

        public LoadCertificateException(string message) : base(message)
        {
        }

        public LoadCertificateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LoadCertificateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
