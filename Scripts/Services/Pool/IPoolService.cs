using UnityEngine;

namespace Services.Pool
{
    public interface IPoolService
    {
        void SpawnPool<T>(string key, int amount) where T : Component;
        void AddObjectToPool(string key, object obj);
        T ActivatePoolItem<T>(string key, Vector3 position, Quaternion rotation) where T : class;
        void DisablePoolItem(string key, object obj);
        void RemovePool(string key);
    }
}