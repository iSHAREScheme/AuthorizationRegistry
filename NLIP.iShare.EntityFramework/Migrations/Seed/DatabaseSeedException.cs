using System;
using System.Runtime.Serialization;

namespace NLIP.iShare.EntityFramework
{
    [Serializable]
    public class DatabaseSeedException : Exception
    {
        public DatabaseSeedException()
        {
        }

        public DatabaseSeedException(string message) : base(message)
        {
        }

        public DatabaseSeedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DatabaseSeedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}