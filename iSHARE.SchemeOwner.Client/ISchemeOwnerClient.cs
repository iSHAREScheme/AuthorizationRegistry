using System.Threading.Tasks;
using iSHARE.SchemeOwner.Client.Models;

namespace iSHARE.SchemeOwner.Client
{
    public interface ISchemeOwnerClient
    {
        Task<Party> GetParty(string partyId);
    }
}
