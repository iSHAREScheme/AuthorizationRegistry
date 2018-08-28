using System.Collections.Generic;
using System.Threading.Tasks;

namespace NLIP.iShare.Abstractions.Email
{
    public interface ITemplateService
    {
        Task<string> GetTransformed(string templateKey, Dictionary<string, string> templateFields);
    }
}
