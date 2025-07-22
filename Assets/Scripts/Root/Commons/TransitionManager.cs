using System;
using System.Threading;
using Automan.Root.View;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using VContainer;

#nullable enable

namespace Automan.Root
{
    /// <summary>
    /// シーン遷移マネージャー
    /// </summary>
    public sealed class TransitionManager : IDisposable
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

        private readonly string StageScenePrefix = "Stage";

        private readonly CancellationTokenSource _cancellationTokenSource = new();

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
        public async UniTask TransitionToAsync(Scene scene, Color fadeColor = default)
        {
            await TransitionToAsync(scene.GetSceneName(), fadeColor);
        }

        /// <summary>
        /// シーン遷移する
        /// </summary>
        /// <param name="stageNumber">ステージ番号</param>
        /// <param name="fadeColor">フェードの色</param>
        public async UniTask TransitionToAsync(int stageNumber, Color fadeColor = default)
        {
            await TransitionToAsync($"{StageScenePrefix}{stageNumber}", fadeColor);
        }

        private async UniTask TransitionToAsync(string sceneName, Color fadeColor)
        {
            await _transitionFadeView.FadeOutAsync(fadeColor, _cancellationTokenSource.Token);
            await SceneManager.LoadSceneAsync(sceneName).ToUniTask(cancellationToken: _cancellationTokenSource.Token);
            await _transitionFadeView.FadeInAsync(fadeColor, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
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