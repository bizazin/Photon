using System;

namespace Services.SceneLoading
{
    public interface ISceneLoadingService
    {
        void LoadScene(string name, Action onLoaded = null);
    }
}