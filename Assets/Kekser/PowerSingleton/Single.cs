using UnityEngine;

namespace Kekser.PowerSingleton
{
    public static class Single<T> where T : class
    {
        private const string PowerSingletonCantBind = "PowerSingleton: Can't bind {0} to {1}";
        
        private static Object _rawInstance { get; set; }
        private static T _instance { get; set; }

        public static bool HasValue => _rawInstance != null;
        
        public static T Instance 
        {
            get
            {
                if (HasValue) return _instance;
                Bind(PowerSingletonManager.Get(typeof(T)));
                return _instance;
            }
        }
        
        public static void Bind(Object instance)
        {
            if (instance == null)
            {
                _rawInstance = null;
                _instance = null;
                return;
            }
            if (!(instance is T tInstance))
            {
                Debug.LogErrorFormat(PowerSingletonCantBind, instance.GetType(), typeof(T));
                return;
            }
            _rawInstance = instance;
            _instance = tInstance;
        }
    }
}
