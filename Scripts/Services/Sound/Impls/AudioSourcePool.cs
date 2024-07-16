using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Services.Sound.Impls
{
    public class AudioSourcePool : IMonoPool<AudioSource>
    {
        private readonly Transform _parent;
        private readonly List<AudioSource> _pool = new();

        public AudioSourcePool()
        {
            var parent = new GameObject {name = "SoundPool"};
            Object.DontDestroyOnLoad(parent);
            _parent = parent.transform;
        }

        public AudioSource Get()
        {
            if (_pool.Count == 0)
                return Create();
            var source = _pool[0];
            source.gameObject.SetActive(true);
            _pool.RemoveAt(0);
            return source;
        }

        private AudioSource Create()
        {
            var source = new GameObject().AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.reverbZoneMix = 0;
            source.transform.SetParent(_parent);
            return source;
        }

        public void Free(AudioSource source)
        {
#if UNITY_EDITOR
            if (_pool.Contains(source))
                throw new Exception("You trying to add same object twice!");
#endif
            source.gameObject.SetActive(false);
            _pool.Add(source);
        }
    }
}