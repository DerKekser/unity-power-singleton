using Kekser.PowerSingleton.Attributes;
using UnityEngine;

namespace Kekser.Tests
{
    [PowerSingleton(typeof(IInterfaceSingleton))]
    public class InterfaceSingleton : MonoBehaviour, IInterfaceSingleton
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