using Kekser.PowerSingleton.Attributes;
using UnityEngine;

namespace Kekser.Tests
{
    [PowerSingleton(Creation = PowerSingletonCreation.Always, CreationName = "AlwaysSingleton")]
    public class AlwaysSingleton : MonoBehaviour
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