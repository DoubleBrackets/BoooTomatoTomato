using PurrNet;
using UnityEngine;

namespace Networking
{
    public class NetworkDebug : MonoBehaviour
    {
        [SerializeField]
        private NetworkManager _networkManager;

        private void Update()
        {
            if (_networkManager.isClient)
            {
                // Does purrnet have a ping display wtf 
            }
        }
    }
}