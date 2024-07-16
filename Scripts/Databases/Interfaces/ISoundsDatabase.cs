using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Databases.Interfaces
{
    public interface ISoundsDatabase
    {
        IEnumerable<Enumerators.ESoundtrackName> SoundtrackTypes { get; }
        
        AudioClip GetSfxClip(Enumerators.ESoundFxName soundName);
        AudioClip GetSoundtracksClip(Enumerators.ESoundtrackName type);
    }
}