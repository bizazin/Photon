using PunNetwork.ObjectPooling;
using UnityEngine;

namespace Services.PhotonPool
{
    public interface IPhotonPoolService
    {
        void PreparePools();
        void SetItemReady(string key, PhotonPoolObject poolObject);
        T ActivatePoolItem<T>(string key, Vector3 position, Quaternion rotation) where T : class;
        void DisablePoolItem(string key, object obj);
    }
}