using System;
using System.Collections.Generic;
using Behaviours;
using Databases.Interfaces;
using DG.Tweening;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Services.Sound.Impls
{
    public class SoundService : ISoundService
    {
        private readonly IMonoPool<AudioSource> _pool;
        private readonly IAudioMixerProvider _audioMixerProvider;
        private readonly ISoundsDatabase _soundsDatabase;
        private readonly List<DisposeAfterPlaying> _playing = new();

        private readonly Queue<Enumerators.ESoundtrackName> _musicQueue = new();
        private bool _isLoopingQueue = true;

        public SoundService
        (
            ISoundsDatabase soundsDatabase,
            IMonoPool<AudioSource> pool,
            IAudioMixerProvider audioMixerProvider
        )
        {
            _soundsDatabase = soundsDatabase;
            _pool = pool;
            _audioMixerProvider = audioMixerProvider;
        }

        public void PlayMusicSingle(Enumerators.ESoundtrackName soundtrack)
        {
            StopAll();
            PlayMusic(soundtrack);
        }

        public void PlaySound(Enumerators.ESoundFxName clipName)
        {
            var source = _pool.Get();
            var clip = _soundsDatabase.GetSfxClip(clipName);
#if UNITY_EDITOR
            source.name = $"Sound: {clip.name}";
#endif
            source.outputAudioMixerGroup = _audioMixerProvider.AudioMixerEffects;
            ResetAndPlay(source, clip, false);
        }

        public void PlayRandomSound(params Enumerators.ESoundFxName[] clipNames)
        {
            var clipName = clipNames[Random.Range(0, clipNames.Length)];
            PlaySound(clipName);
        }

        public void PlayMusic(Enumerators.ESoundtrackName musicType, bool isQueue = false, bool loop = true)
        {
            var source = _pool.Get();
            var music = _soundsDatabase.GetSoundtracksClip(musicType);
#if UNITY_EDITOR
            source.name = $"Music: {music.name}: {(loop ? "Looped" : "Not looped")}";
#endif
            source.outputAudioMixerGroup = _audioMixerProvider.AudioMixerMusic;
            ResetAndPlay(source, music, loop, isQueue ? PlayNextInQueue : null);
        }

        public void PlayMusicQueue(bool isLoop = true)
        {
            _isLoopingQueue = isLoop;

            foreach (var musicType in _soundsDatabase.SoundtrackTypes)
                _musicQueue.Enqueue(musicType);

            PlayNextInQueue();
        }
        

        private void ResetAndPlay(AudioSource source, AudioClip clip, bool loop, Action onComplete = null)
        {
            source.clip = clip;
            source.loop = loop;
            source.volume = 1f;
            source.Play();
            _playing.Add(new DisposeAfterPlaying(source, disposeAfterPlaying =>
            {
                ReturnToPool(disposeAfterPlaying);
                onComplete?.Invoke();
            }));
        }

        private void PlayNextInQueue()
        {
            if (_musicQueue.Count == 0)
            {
                if (_isLoopingQueue)
                    PlayMusicQueue();
                return;
            }

            var nextMusic = _musicQueue.Dequeue();
            PlayMusic(nextMusic, true, false);
        }

        private void ReturnToPool(DisposeAfterPlaying disposeAfterPlaying)
        {
            _playing.Remove(disposeAfterPlaying);
            _pool.Free(disposeAfterPlaying.Source);
        }

        public void StopAll()
        {
            if (_playing.Count <= 0) return;
            var playingCopy = new List<DisposeAfterPlaying>(_playing);

            foreach (var playDisposable in playingCopy) playDisposable?.Dispose();

            _playing.Clear();

        }

        private class DisposeAfterPlaying
        {
            public AudioSource Source;
            private Tween _tween;
            private Action<DisposeAfterPlaying> _onComplete;

            public DisposeAfterPlaying(AudioSource source, Action<DisposeAfterPlaying> onComplete)
            {
                Source = source;
                _onComplete = onComplete;
                if (!source.loop)
                    _tween = DOVirtual.DelayedCall(source.clip.length, Dispose);
            }

            public void Dispose()
            {
                _onComplete(this);
                _tween.Kill();
                _tween = null;
                _onComplete = null;
                Source = null;
            }
        }
    }
}