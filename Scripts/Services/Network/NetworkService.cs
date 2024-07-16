using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Services.Network
{
    public class NetworkService : INetworkService
    {
        public async Task<string> GetRequest(string url, Dictionary<string, string> headers = null)
        {
            using var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            if (headers != null)
            {
                foreach (var header in headers)
                    uwr.SetRequestHeader(header.Key, header.Value);
            }

            var downloadHandler = new DownloadHandlerBuffer();
            uwr.downloadHandler = downloadHandler;

            var operation = uwr.SendWebRequest();

            while (!operation.isDone)
                await Task.Delay(100);

            if (uwr.result == UnityWebRequest.Result.Success)
                return System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data);
#if UNITY_EDITOR
            Debug.LogError($"Failed to load: {url} due to error: {uwr.error}");
#endif
            return null;

        }
    }
}