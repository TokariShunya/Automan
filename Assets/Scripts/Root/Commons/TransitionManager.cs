using System;
using Automan.Root.View;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Automan.Root
{
    /// <summary>
    /// シーン遷移マネージャー
    /// </summary>
    public sealed class TransitionManager
    {
        private readonly TransitionFadeView _transitionFadeView;

        /// <summary>
        /// シーン定義
        /// </summary>
        public enum Scene
        {
            Title,
            Result,
        }

        private readonly string _stageScenePrefix = "Stage";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="transitionFadeView">フェードのView</param>
        [Inject]
        public TransitionManager(TransitionFadeView transitionFadeView)
        {
            _transitionFadeView = transitionFadeView;
        }

        /// <summary>
        /// シーン遷移する
        /// </summary>
        /// <param name="scene">シーン定義</param>
        /// <param name="fadeColor">フェードの色</param>
        public async UniTask TransitionTo(Scene scene, Color fadeColor = default)
        {
            await TransitionTo(scene.GetSceneName(), fadeColor);
        }

        /// <summary>
        /// シーン遷移する
        /// </summary>
        /// <param name="stageNumber">ステージ番号</param>
        /// <param name="fadeColor">フェードの色</param>
        public async UniTask TransitionTo(int stageNumber, Color fadeColor = default)
        {
            await TransitionTo($"{_stageScenePrefix}{stageNumber}", fadeColor);
        }

        private async UniTask TransitionTo(string sceneName, Color fadeColor = default)
        {
            await _transitionFadeView.FadeOutAsync(fadeColor);
            await SceneManager.LoadSceneAsync(sceneName);
            await _transitionFadeView.FadeInAsync(fadeColor);
        }
    }

    /// <summary>
    /// シーン定義の拡張
    /// </summary>
    public static class SceneEnumExtensions
    {
        /// <summary>
        /// シーン定義からシーン名を得る
        /// </summary>
        /// <param name="scene">シーン定義</param>
        /// <returns>シーン名</returns>
        /// <exception cref="InvalidOperationException">シーン名を取得できません</exception>
        public static string GetSceneName(this TransitionManager.Scene scene)
        {
            return scene switch
            {
                TransitionManager.Scene.Title => "Title",
                TransitionManager.Scene.Result => "Result",
                _ => throw new InvalidOperationException("シーン名を取得できません")
            };
        }
    }

}