using Utils;

namespace Services.Sound
{
    public interface ISoundService
    {
        void PlayMusicSingle(Enumerators.ESoundtrackName soundtrack);
        void PlaySound(Enumerators.ESoundFxName clip);
        void PlayMusic(Enumerators.ESoundtrackName musicType, bool isQueue = false, bool loop = true);
        void StopAll();
        void PlayMusicQueue(bool isLoop = true);
        void PlayRandomSound(params Enumerators.ESoundFxName[] clipNames);
    }

}