using Microsoft.AspNetCore.Hosting;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using NLIP.iShare.Abstractions.Email;

namespace NLIP.iShare.EmailClient
{
    public class TemplateService : ITemplateService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ConcurrentDictionary<string, string> _templates;
        public TemplateService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _templates = new ConcurrentDictionary<string, string>();
        }

        public async Task<string> GetTransformed(string templateKey, Dictionary<string, string> templateFields)
        {
            var template = await GetTemplate(templateKey);

            var parsedTemplate = Transform(template, templateFields);
            return parsedTemplate;
        }

        private async Task<string> GetTemplate(string templateKey)
        {
            if (!_templates.TryGetValue(templateKey, out string item))
            {
                using (StreamReader sourceReader = File.OpenText(GetTemplatePath(templateKey)))
                {
                    _templates[templateKey] = await sourceReader.ReadToEndAsync();
                }
            }

            var template = _templates[templateKey];
            return template;
        }

        private string GetTemplatePath(string templateName)
        {
            return _hostingEnvironment.WebRootPath
                   + Path.DirectorySeparatorChar
                   + "templates"
                   + Path.DirectorySeparatorChar
                   + templateName;
        }

        private string Transform(string template, Dictionary<string, string> values)
        {
            var parsed = template;
            foreach (var entry in values)
            {
                parsed = parsed.Replace("{" + entry.Key + "}", entry.Value, false, CultureInfo.InvariantCulture);
            }

            return parsed;
        }
    }
}
