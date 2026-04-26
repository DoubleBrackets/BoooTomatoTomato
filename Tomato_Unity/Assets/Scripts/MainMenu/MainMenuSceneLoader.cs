using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using GameKit.Dependencies.Utilities.Types;
using UnityEngine;

namespace MainMenu
{
    public class MainMenuSceneLoader : MonoBehaviour
    {
        [Scene]
        [SerializeField]
        private string _gameplayScene;

        private void Awake()
        {
            NetworkManager networkManager = InstanceFinder.NetworkManager;
            networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
        }

        private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs args)
        {
            if (args.ConnectionState == LocalConnectionState.Started)
            {
                SceneLoadData sld = new(_gameplayScene)
                {
                    ReplaceScenes = ReplaceOption.All
                };
                InstanceFinder.SceneManager.LoadGlobalScenes(sld);
            }
        }
    }
}