using Kekser.PowerSingleton.Attributes;
using UnityEngine;

namespace Kekser.Tests
{
    [PowerSingleton]
    public class DefaultSingleton : MonoBehaviour
    {
        public static bool Created = false;
        
        private void Awake()
        {
            Created = true;
        }
        
        private void OnDestroy()
        {
            Created = false;
        }
    }
}