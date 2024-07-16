using System;
using System.Collections.Generic;
using Databases.Interfaces;
using UnityEngine;
using static Utils.Enumerators;

namespace Databases
{
    [CreateAssetMenu(menuName = "Databases/SoundsDatabase", fileName = "SoundsDatabase")]
    public class SoundsDatabase : ScriptableObject, ISoundsDatabase
    {
        [SerializeField] private AudioFileFx[] _sfx;
        [SerializeField] private AudioFileSoundtrack[] _soundtracks;
        private Dictionary<ESoundFxName, AudioClip> _sfxDictionary;
        private Dictionary<ESoundtrackName, AudioClip> _soundtracksDictionary;
        private readonly List<ESoundtrackName> _soundtrackTypes = new();
        
        public IEnumerable<ESoundtrackName> SoundtrackTypes => _soundtrackTypes;

        private void OnEnable()
        {
            _sfxDictionary = new Dictionary<ESoundFxName, AudioClip>();
            _soundtracksDictionary = new Dictionary<ESoundtrackName, AudioClip>();
            
            foreach (var fx in _sfx) 
                _sfxDictionary.Add(fx.Name, fx.Clip);

            foreach (var fx in _soundtracks)
            {
                _soundtrackTypes.Add(fx.Type);
                _soundtracksDictionary.Add(fx.Type, fx.Clip);
            }
        }

        public AudioClip GetSfxClip(ESoundFxName soundName)
        {
            try
            {
                return _sfxDictionary[soundName];
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"[{nameof(SoundsDatabase)}] Sfx with name {soundName} was not present in the dictionary. {e.StackTrace}");
            }
        }

        public AudioClip GetSoundtracksClip(ESoundtrackName type)
        {
            try
            {
                return _soundtracksDictionary[type];
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"[{nameof(SoundsDatabase)}] Soundtrack with name {type} was not present in the dictionary. {e.StackTrace}");
            }
        }
    }
    
    [Serializable]
    public struct AudioFileFx
    {
        public ESoundFxName Name;
        public AudioClip Clip;
    }
    
    [Serializable]
    public struct AudioFileSoundtrack
    {
        public ESoundtrackName Type;
        public AudioClip Clip;
    }
}