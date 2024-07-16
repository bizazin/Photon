using System.Collections.Generic;
using UnityEngine;

namespace Services.Pool
{
    public class PoolVo
    {
        public readonly Queue<object> Objects;
        public readonly Transform Parent;

        public PoolVo(Transform parent, Queue<object> objects = null)
        {
            Parent = parent;
            Objects = objects ?? new Queue<object>();
        }
    }
}