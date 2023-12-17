using Kekser.PowerSingleton.Attributes;
using UnityEngine;

namespace Kekser.Tests
{
    [PowerSingleton(Creation = PowerSingletonCreation.IfNeeded, CreationName = "IfNeededSingleton")]
    public class IfNeededSingleton : MonoBehaviour
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