using UnityEngine;

namespace Services.Sound
{
    public interface IMonoPool<T> where T : Object 
    {
        T Get();
        void Free(T instance);
    }
}