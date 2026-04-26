using System;
using System.IO;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using GameKit.Dependencies.Utilities.Types;
using UnityEngine;

namespace MainMenu
{
    /// <summary>
    ///     When connecting move over to gameplay scene
    /// </summary>
    public class MainMenuSceneLoader : MonoBehaviour
    {
        [Scene]
        [SerializeField]
        private string _gameplayScene;

        private NetworkManager _networkManager;

        private void Awake()
        {
            _networkManager = InstanceFinder.NetworkManager;
            _networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
        }

        private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs args)
        {
            if (args.ConnectionState == LocalConnectionState.Started)
            {
                SceneLoadData sld = new(GetSceneName(_gameplayScene))
                {
                    ReplaceScenes = ReplaceOption.All
                };
                InstanceFinder.SceneManager.LoadGlobalScenes(sld);
            }
        }

        private string GetSceneName(string fullPath)
        {
            return Path.GetFileNameWithoutExtension(fullPath);
        }
    }
}