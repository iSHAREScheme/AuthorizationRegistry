using System.Threading.Tasks;

namespace iSHARE.Abstractions.Storage
{
    public interface IFileStorage
    {
        Task<string> ReadAsString(string path);
        Task Upload(string path, byte[] content);
        Task<bool> Exists(string path);
    }
}
