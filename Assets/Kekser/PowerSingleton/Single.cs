using UnityEngine;

namespace Kekser.PowerSingleton
{
    public static class Single<T> where T : class
    {
        private static Object _rawInstance;
        private static T _instance { get; set; }

        public static T Instance {
            get
            {
                if (_rawInstance != null) return _instance;
                Bind(PowerSingletonManager.Get(typeof(T)));
                return _instance;
            }
        }
        
        public static void Bind(Object instance)
        {
            _rawInstance = instance;
            _instance = instance as T;
        }
    }
}