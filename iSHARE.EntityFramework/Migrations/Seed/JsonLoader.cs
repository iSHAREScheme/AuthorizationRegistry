using System.IO;
using System.Reflection;

namespace iSHARE.EntityFramework.Migrations.Seed
{
    public static class JsonLoader
    {
        public static string GetByName(string fileName, string environment, string namespaceName, Assembly assembly)
        {
            var resourceName = $"{namespaceName}.{environment}.{fileName}";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new DatabaseSeedException(
                        $"Json resource {resourceName} was not found in the `{assembly.FullName}` assembly. " +
                        "Neither the json file is present nor is built as embedded resource.");
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
