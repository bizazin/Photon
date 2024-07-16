using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.Audio;
using static Utils.Enumerators;

namespace Behaviours
{
    public class AudioMixerProvider : MonoBehaviour, IAudioMixerProvider
    {
        private const string Volume = "Volume";
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioMixerGroupVo[] _audioMixerGroupVos;

        private readonly Dictionary<EVolumeType, AudioMixerGroup> _audioMixerGroups = new();

        public AudioMixerGroup AudioMixerMusic => _audioMixerGroups[EVolumeType.Music];
        public AudioMixerGroup AudioMixerEffects => _audioMixerGroups[EVolumeType.Effects];
        public AudioMixerGroup AudioMixerVideo => _audioMixerGroups[EVolumeType.Video];
        
        private void OnEnable()
        {
            foreach (var audioMixerGroupVo in _audioMixerGroupVos)
                _audioMixerGroups.Add(audioMixerGroupVo.Type, audioMixerGroupVo.Group);
        }

        public void SetVolumeByType(EVolumeType volumeType, float value)
        {
            if (volumeType == EVolumeType.Master)
            {
                _audioMixer.SetFloat(_audioMixer.name + Volume, LinearToDecibel(value));
                return;
            }

            _audioMixer.SetFloat(_audioMixerGroups[volumeType].name + Volume, LinearToDecibel(value));
        }
        
        private float LinearToDecibel(float linear)
        {
            if (linear == 0)
                return -80;

            return Mathf.Clamp(20f * Mathf.Log10(linear), -80, 0);
        }
    }

    public interface IAudioMixerProvider
    {
        AudioMixerGroup AudioMixerMusic { get; }
        AudioMixerGroup AudioMixerEffects { get; }
        AudioMixerGroup AudioMixerVideo { get; }
        void SetVolumeByType(EVolumeType volumeType, float value);
    }
}