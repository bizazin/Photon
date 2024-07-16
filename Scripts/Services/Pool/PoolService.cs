using System;
using System.Collections.Generic;
using Photon.PhotonUnityNetworking.Code.Common;
using Photon.PhotonUnityNetworking.Code.Common.Factory;
using UnityEngine;

namespace Services.Pool
{
    public class PoolService : IPoolService
    {
        private readonly Dictionary<string, PoolVo> _poolDictionary = new();
        private readonly IGameFactory _gameFactory;
        
        public PoolService
        (
            IGameFactory gameFactory
        )
        {
            _gameFactory = gameFactory;
        }

        public void SpawnPool<T>(string key, int amount) where T : Component
        {
            if (!_poolDictionary.ContainsKey(key))
            {
                var objectPool = new Queue<object>();
                var parent = new GameObject($"[Pool] {key}").transform;

                for (var i = 0; i < amount; i++)
                {
                    var component = _gameFactory.CreateWithKey(key, Vector3.zero, Quaternion.identity)
                        .GetComponent<T>();
                    component.transform.SetParent(parent);
                    component.gameObject.SetActive(false);
                    objectPool.Enqueue(component);
                }

                _poolDictionary.Add(key, new PoolVo(parent, objectPool));
            }
            else
            {
                Debug.LogWarning("Pool with key " + key + " already exists.");
            }
        }

        public void AddObjectToPool(string key, object obj)
        {
            if (!_poolDictionary.ContainsKey(key))
            {
                var parent = new GameObject($"[Pool] {key}").transform;

                _poolDictionary.Add(key, new PoolVo(parent));
            }

            var poolInfoVo = _poolDictionary[key];

            poolInfoVo.Objects.Enqueue(obj);

            if (obj is not GameObject gameObject) return;
            gameObject.transform.SetParent(poolInfoVo.Parent);
            gameObject.SetActive(false);
        }

        public T ActivatePoolItem<T>(string key, Vector3 position, Quaternion rotation) where T : class
        {
            if (!_poolDictionary.TryGetValue(key, out var poolInfoVo))
            {
                Debug.LogWarning($"Pool with key {key} doesn't exist.");
                return null;
            }

            var objectToSpawn = poolInfoVo.Objects.Count == 0
                ? CreateAndReturnNewInstance<T>(key)
                : poolInfoVo.Objects.Dequeue();

            if (objectToSpawn is not T castedObject) return null;
            if (castedObject is not Component comp) return castedObject;
            comp.gameObject.SetActive(true);
            comp.transform.position = position;
            comp.transform.rotation = rotation;

            return castedObject;
        }

        public void DisablePoolItem(string key, object obj)
        {
            if (!_poolDictionary.ContainsKey(key))
            {
                Debug.LogWarning("Pool with key " + key + " doesn't exist.");
                return;
            }

            if (obj is Component comp) 
                comp.gameObject.SetActive(false);

            _poolDictionary[key].Objects.Enqueue(obj);
        }

        public void RemovePool(string key)
        {
            if (!_poolDictionary.TryGetValue(key, out var pool)) return;
            pool.Objects.Clear();
            _poolDictionary.Remove(key);
        }
        
        private T CreateAndReturnNewInstance<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Path cannot be null or empty", nameof(key));

            Component newObj = _gameFactory.CreateWithKey(key, Vector3.zero, Quaternion.identity).transform;
            newObj.gameObject.SetActive(false);

            return newObj as T;
        }
    }
}