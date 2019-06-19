using System.Threading.Tasks;
using iSHARE.Models;
using iSHARE.Models.Capabilities;

namespace iSHARE.Api.Interfaces
{
    public interface ICapabilitiesService
    {
        Task<Response<Capabilities>> Get();
    }
}
