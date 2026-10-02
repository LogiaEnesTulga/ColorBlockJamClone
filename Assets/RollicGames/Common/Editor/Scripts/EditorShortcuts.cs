using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace RollicGames.Common.Editor
{
    [InitializeOnLoad]
    public static class EditorShortcuts
    {
        private const string RootScenePath = "Assets/Game/Scenes/Root.unity";
        private const string PreviousScenePathsKey = "RollicGames.Common.Editor.EditorShortcuts.PreviousScenePaths";
        private const char ScenePathSeparator = '|';

        static EditorShortcuts()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        [MenuItem("RollicGames/Play Root Scene %#r")]
        private static void PlayRootScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            SessionState.SetString(PreviousScenePathsKey, string.Join(ScenePathSeparator, GetOpenScenePaths()));

            EditorSceneManager.OpenScene(RootScenePath);
            EditorApplication.isPlaying = true;
        }

        [MenuItem("RollicGames/Play Root Scene %#r", true)]
        private static bool CanPlayRootScene()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        // Active scene first so it can be restored as the active one.
        private static List<string> GetOpenScenePaths()
        {
            var activeScene = SceneManager.GetActiveScene();
            var paths = new List<string>();
            if (!string.IsNullOrEmpty(activeScene.path))
            {
                paths.Add(activeScene.path);
            }

            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene != activeScene && scene.isLoaded && !string.IsNullOrEmpty(scene.path))
                {
                    paths.Add(scene.path);
                }
            }

            return paths;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredEditMode)
            {
                return;
            }

            var previousScenePaths = SessionState.GetString(PreviousScenePathsKey, string.Empty);
            SessionState.EraseString(PreviousScenePathsKey);
            if (string.IsNullOrEmpty(previousScenePaths))
            {
                return;
            }

            var paths = previousScenePaths.Split(ScenePathSeparator);
            var activeScene = EditorSceneManager.OpenScene(paths[0], OpenSceneMode.Single);
            for (var i = 1; i < paths.Length; i++)
            {
                EditorSceneManager.OpenScene(paths[i], OpenSceneMode.Additive);
            }

            SceneManager.SetActiveScene(activeScene);
        }
    }
}
