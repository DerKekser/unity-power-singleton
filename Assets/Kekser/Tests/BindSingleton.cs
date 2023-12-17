using UnityEngine;

namespace Kekser.Tests
{
    public class BindSingleton : MonoBehaviour
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