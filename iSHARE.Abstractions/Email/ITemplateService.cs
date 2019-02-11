using System.Collections.Generic;
using System.Threading.Tasks;

namespace iSHARE.Abstractions.Email
{
    public interface ITemplateService
    {
        Task<string> GetTransformed(string templateKey, Dictionary<string, string> templateFields);
    }
}
