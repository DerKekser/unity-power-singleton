using UnityEngine;

namespace Kekser.PowerSingleton
{
    public static class Single<T> where T : class
    {
        private static T CachedInstance { get; set; }

        public static T Instance => CachedInstance ??= PowerSingletonManager.Get(typeof(T)) as T;
    }
}