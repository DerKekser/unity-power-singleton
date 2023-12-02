using Object = UnityEngine.Object;

namespace Kekser.PowerSingleton
{
    public static class Single<T> where T : class
    {
        public static T Instance => PowerSingletonManager.Get(typeof(T)) as T;
    }
}