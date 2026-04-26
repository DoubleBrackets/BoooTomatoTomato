using UnityEngine;

/// <summary>
///     Instantiates global services on load, before the initial scene calls Awake
/// </summary>
public class GlobalServicesInstantiator : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InstantiateGlobalServices()
    {
        // NOTE: read docs to see directory requirements for Resources.Load!
        var prefab = Resources.Load<GameObject>("GlobalServices");
        // create the prefab in your scene
        GameObject inScene = Instantiate(prefab);

        // mark root as DontDestroyOnLoad();
        DontDestroyOnLoad(inScene);
    }
}