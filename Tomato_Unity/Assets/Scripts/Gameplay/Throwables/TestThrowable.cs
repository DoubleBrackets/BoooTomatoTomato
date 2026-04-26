using UnityEngine;

namespace Gameplay.Throwables
{
    public class TestThrowable : MonoBehaviour, IThrowableObject
    {
        [SerializeField]
        private ThrowableObjectInfo _info;

        public ThrowableObjectInfo GetInfo()
        {
            return _info;
        }
    }
}