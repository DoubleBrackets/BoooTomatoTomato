using System.IO;
using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using GameKit.Dependencies.Utilities.Types;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public class GameplaySceneLoader : MonoBehaviour
    {
        [Scene]
        [SerializeField]
        private string _mainMenuScene;

        private void Awake()
        {
            NetworkManager networkManager = InstanceFinder.NetworkManager;
            networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
            networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        }

        private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs args)
        {
            if (InstanceFinder.NetworkManager.IsServerStarted)
            {
                return;
            }

            if (args.ConnectionState == LocalConnectionState.Stopped)
            {
                // Already in offline scene.
                if (SceneManager.GetActiveScene().name == GetSceneName(_mainMenuScene))
                {
                    return;
                }

                SceneManager.LoadScene(_mainMenuScene);
            }
        }

        private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs args)
        {
            if (args.ConnectionState == LocalConnectionState.Stopped)
            {
                // Already in offline scene.
                if (SceneManager.GetActiveScene().name == GetSceneName(_mainMenuScene))
                {
                    return;
                }

                SceneManager.LoadScene(_mainMenuScene);
            }
        }

        private string GetSceneName(string fullPath)
        {
            return Path.GetFileNameWithoutExtension(fullPath);
        }
    }
}