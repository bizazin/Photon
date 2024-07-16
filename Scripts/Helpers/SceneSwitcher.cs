#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Utils;

namespace Helpers
{
    public class SceneSwitcher : EditorWindow
    {
        [MenuItem("Tools/Switch to Initial Scene %s")]
        public static void SwitchToInitialScene()
        {
            var scenePath = $"Assets/Scenes/{SceneNames.Initial}.unity";

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            else
                Debug.LogError("Scene switch cancelled.");
        }
    }
}
#endif
