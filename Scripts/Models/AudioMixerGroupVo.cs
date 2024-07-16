using System;
using UnityEngine.Audio;
using Utils;

namespace Models
{
    [Serializable]
    public class AudioMixerGroupVo
    {
        public Enumerators.EVolumeType Type;
        public AudioMixerGroup Group;
    }
}