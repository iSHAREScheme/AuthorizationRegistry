using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace NLIP.iShare.Configuration.Configurations
{
    [Serializable]
    public class ConfigurationException : Exception
    {
        public ConfigurationException()
        {
        }

        public ConfigurationException(string message) : base(message)
        {
        }

        public ConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public static ConfigurationException CreateFromKey(string keyName)
          => new ConfigurationException($"Either the `{keyName}` key was not found in the configuration or its value was not set.");

        public static void AssertNotNull(string value, string keyName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw CreateFromKey(keyName);
            }
        }

        public static void AssertUri(string value, string keyName)
        {
            AssertNotNull(value, keyName);

            if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
            {
                throw new ConfigurationException($"The value of `{keyName}` key must be a valid URI.");
            }
        }

        public static void AssertThumbprint(string value, string keyName, bool required = true)
        {
            if (required)
            {
                AssertNotNull(value, keyName);
            }

            
            if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value, "^[A-Fa-f0-9]{40}$"))
            {
                throw new ConfigurationException($"The value of `{keyName}` doesn't have a SHA-1 like value.");
            }
        }
    }
}
