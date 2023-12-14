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
                _rawInstance = PowerSingletonManager.Get(typeof(T));
                _instance = _rawInstance as T;
                return _instance;
            }
        }
    }
}