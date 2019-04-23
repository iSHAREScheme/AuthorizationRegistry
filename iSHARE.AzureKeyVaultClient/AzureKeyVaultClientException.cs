using System;
using System.Runtime.Serialization;

namespace iSHARE.AzureKeyVaultClient
{
    [Serializable]
    public class AzureKeyVaultClientException : Exception
    {
        public AzureKeyVaultClientException()
        {
        }

        public AzureKeyVaultClientException(string message) : base(message)
        {
        }

        public AzureKeyVaultClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AzureKeyVaultClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
