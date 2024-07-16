using System;
using System.Collections;
using Behaviours;
using UnityEngine.SceneManagement;

namespace Services.SceneLoading
{
    public class SceneLoadingService : ISceneLoadingService
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public SceneLoadingService
        (
            ICoroutineRunner coroutineRunner
        )
        {
            _coroutineRunner = coroutineRunner;
        }

        public void LoadScene(string name, Action onLoaded = null)
        {
            _coroutineRunner.StartCoroutine(LoadSceneCoroutine(name, onLoaded));
        }

        private static IEnumerator LoadSceneCoroutine(string sceneName, Action onLoaded = null)
        {
            var loadingOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            while (loadingOperation is { isDone: false })
                yield return null;

            onLoaded?.Invoke();
        }
    }
}