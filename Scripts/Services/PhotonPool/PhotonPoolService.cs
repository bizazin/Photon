using System;
using System.Collections.Generic;
using System.Linq;
using Photon.PhotonUnityNetworking.Code.Common;
using Photon.Pun;
using PunNetwork.ObjectPooling;
using PunNetwork.Services.RoomPlayer;
using UnityEngine;

namespace Services.PhotonPool
{
    public class PhotonPoolService : IPhotonPoolService
    {
        private readonly IRoomPlayersService _roomPlayersService;
        private readonly Dictionary<string, PhotonPoolVo> _poolDictionary = new();
        private bool _isPoolsPrepared;

        public PhotonPoolService
        (
            IRoomPlayersService roomPlayersService
        )
        {
            _roomPlayersService = roomPlayersService;
        }

        public void PreparePools()
        {
            InitializePool(Enumerators.GameObjectEntryKey.Bullet, 20);

            if (PhotonNetwork.IsMasterClient)
                SpawnPhotonPools();
        }

        public void SetItemReady(string key, PhotonPoolObject poolObject)
        {
            var poolVo = _poolDictionary[key];
            poolVo.Add(poolObject);

            if (poolVo.IsReady && !_isPoolsPrepared)
                CheckPoolsAreReady();
        }

        public T ActivatePoolItem<T>(string key, Vector3 position, Quaternion rotation) where T : class
        {
            if (!_poolDictionary.TryGetValue(key, out var poolVo))
            {
                Debug.LogWarning($"Pool with key {key} doesn't exist.");
                return null;
            }

            var objectToSpawn = poolVo.Objects.Count == 0
                ? CreateAndReturnNewInstance<T>(key)
                : poolVo.Objects.Dequeue();

            if (objectToSpawn is not T castedObject) return null;
            if (castedObject is not PhotonPoolObject photonPoolObject) return castedObject;
            photonPoolObject.ActivateBase(position, rotation);

            return castedObject;
        }

        public void DisablePoolItem(string key, object obj)
        {
            if (!_poolDictionary.ContainsKey(key))
            {
                Debug.LogWarning("Pool with key " + key + " doesn't exist.");
                return;
            }

            if (obj is PhotonPoolObject photonPoolObject)
                photonPoolObject.DisableBase();
            
            _poolDictionary[key].Objects.Enqueue(obj);
        }

        private static T CreateAndReturnNewInstance<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Path cannot be null or empty", nameof(key));

            Component newObj = PhotonNetwork.Instantiate(key, Vector3.zero, Quaternion.identity).transform;

            return newObj.GetComponent<T>();
        }

        private void InitializePool(Enumerators.GameObjectEntryKey gameObjectEntryKey, int size) =>
            _poolDictionary.Add(gameObjectEntryKey.ToString(), new PhotonPoolVo(gameObjectEntryKey.ToString(), size));

        private void SpawnPhotonPools()
        {
            foreach (var key in _poolDictionary.Keys)
                for (var i = 0; i < _poolDictionary[key].InitialSize; i++)
                    PhotonNetwork.Instantiate(key, Vector3.zero, Quaternion.identity);
        }

        private void CheckPoolsAreReady()
        {
            var isAllReady = _poolDictionary.Keys.All(key => _poolDictionary[key].IsReady);

            if (!isAllReady) return;

            _isPoolsPrepared = true;

            _roomPlayersService.SendLocalPoolsPrepared();
        }
    }
}