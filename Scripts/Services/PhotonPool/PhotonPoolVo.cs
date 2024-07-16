using System.Collections.Generic;
using PunNetwork.ObjectPooling;
using UnityEngine;

namespace Services.PhotonPool
{
    public class PhotonPoolVo
    {
        public readonly Queue<object> Objects;
        private readonly Transform _parent;
        public int InitialSize { get; }
        public bool IsReady { get; private set; } 

        public PhotonPoolVo(string key, int initialSize)
        {
            _parent = new GameObject($"[Pool] {key}").transform;
            Objects = new Queue<object>();
            InitialSize = initialSize;
        }

        public void Add(PhotonPoolObject poolObject)
        {
            Objects.Enqueue(poolObject);
            
            if (Objects.Count >= InitialSize)
                IsReady = true;
            
            poolObject.transform.SetParent(_parent);
            poolObject.gameObject.SetActive(false);
        }
    }
}