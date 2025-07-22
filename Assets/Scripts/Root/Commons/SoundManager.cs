using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

#nullable enable

namespace Automan.Root
{
    /// <summary>
    /// サウンドマネージャー
    /// </summary>
    public sealed class SoundManager
    {
        /// <summary>
        /// サウンド定義
        /// </summary>
        public enum Sound
        {
            None,
            Button,
            Stage,
            Start,
            Correct,
            Error,
            Transition,
            ClearJingle,
            GameoverJingle,
            AllClearJingle,
        }

        private readonly AudioSource _audioSource;
        private readonly IReadOnlyDictionary<Sound, AudioClip> _soundDictionary;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="audioSource">AudioSource</param>
        /// <param name="soundDictionary">サウンド辞書</param>
        [Inject]
        public SoundManager(AudioSource audioSource, IReadOnlyDictionary<Sound, AudioClip> soundDictionary)
        {
            _audioSource = audioSource;
            _soundDictionary = soundDictionary;
        }

        /// <summary>
        /// サウンドを再生する
        /// </summary>
        /// <param name="sound">再生するサウンド</param>
        public void Play(Sound sound)
        {
            if (_soundDictionary.TryGetValue(sound, out AudioClip audioClip))
            {
                _audioSource.PlayOneShot(audioClip);
            }
        }
    }
}