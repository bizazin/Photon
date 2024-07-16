using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Network
{
    public interface INetworkService
    {
        Task<string> GetRequest(string url, Dictionary<string, string> headers = null);
    }
}