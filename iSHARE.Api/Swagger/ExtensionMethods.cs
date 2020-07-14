using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace iSHARE.Api.Swagger
{
    internal static class ExtensionMethods
    {
        public static string FriendlyId(this Type type, bool fullyQualified = false)
        {
            string str = fullyQualified ? type.FullNameSansTypeArguments().Replace("+", ".") : type.Name;
            if (!type.GetTypeInfo().IsGenericType)
                return str;
            string[] array = ((IEnumerable<Type>)type.GetGenericArguments()).Select<Type, string>((Func<Type, string>)(t => t.FriendlyId(fullyQualified))).ToArray<string>();
            return new StringBuilder(str).Replace(string.Format("`{0}", (object)((IEnumerable<string>)array).Count<string>()), string.Empty).Append(string.Format("[{0}]", (object)string.Join(",", array).TrimEnd(','))).ToString();
        }

        private static string FullNameSansTypeArguments(this Type type)
        {
            if (string.IsNullOrEmpty(type.FullName))
                return string.Empty;
            string fullName = type.FullName;
            int length = fullName.IndexOf("[[");
            if (length != -1)
                return fullName.Substring(0, length);
            return fullName;
        }
    }
}
