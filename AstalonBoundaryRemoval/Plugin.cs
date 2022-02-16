using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace AstalonBoundaryRemoval
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        private static ConfigEntry<bool> removeUIBoundaries;
        private static ConfigEntry<bool> removeCinematicLetterboxing;

        public override void Load()
        {
            // Plugin startup logic
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            removeUIBoundaries = Config.Bind("General",
                                             "RemoveUIBoundaries",
                                             true,
                                             "Remove boundaries from in-game UI"
                                            );

            removeCinematicLetterboxing = Config.Bind("General",
                                             "RemoveCinematicLetterboxing",
                                             true,
                                             "Remove letterboxing from cinematics"
                                            );

            SceneManager.add_sceneLoaded((UnityAction<Scene, LoadSceneMode>)OnSceneLoaded);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Log.LogInfo($"Scene {scene.name} loaded.");
            if (scene.name == "GameplayMain")
            {
                if (removeUIBoundaries.Value)
                {
                    var uiBoundariesGO = GameObject.Find("GameWorld/Canvas/MainCameraHolder/Main Camera/UI Boundaries");
                    if (uiBoundariesGO != null)
                    {
                        Log.LogInfo("Disabling UI Boundaries");
                        DisableChildrenInGameObject(uiBoundariesGO);
                    }
                    else
                    {
                        Log.LogWarning("UI Boundaries GameObject does not exist");
                    }
                }

                if (removeCinematicLetterboxing.Value)
                {
                    var letterboxingGO = GameObject.Find("GameWorld/Canvas/Letterbox");
                    if (letterboxingGO != null)
                    {
                        Log.LogInfo("Disabling cinematic letterboxing.");
                        DisableChildrenInGameObject(letterboxingGO);
                    }
                    else
                    {
                        Log.LogWarning("Letterboxing GameObject does not exist");
                    }
                }
            }
        }

        private void DisableChildrenInGameObject(GameObject go)
        {
            foreach (var child in go.GetComponentsInChildren<Transform>())
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}