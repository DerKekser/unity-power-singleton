using System;
using System.Collections.Generic;
using System.Reflection;
using Kekser.PowerSingleton.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kekser.PowerSingleton
{
    public static class PowerSingletonManager
    {
        private struct PowerSingletonData
        {
            public Type GenericType;
            public Type Type;
            public PowerSingletonCreation Creation;
            public string CreationName;
            public bool DontDestroyOnLoad;
        }
        
        private static Dictionary<Type, PowerSingletonData> _powerSingletons = new Dictionary<Type, PowerSingletonData>();
        private static Dictionary<Type, Object> _cachedSingletons = new Dictionary<Type, Object>();
        private static bool _initialized;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            LookUpAttributes();
            AutoCreate();
        }
        
        private static bool TryAddToDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                return false;
            dictionary.Add(key, value);
            return true;
        }

        private static void LookUpAttributes()
        {
            if (_initialized)
                return;
            
            Assembly[] types = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in types)
            {
                if (!assembly.FullName.StartsWith("Assembly-CSharp"))
                    continue;

                foreach (Type type in assembly.GetTypes())
                {
                    var attributes = type.GetCustomAttributes(typeof(PowerSingletonAttribute), false);
                    if (attributes.Length <= 0) continue;
                    
                    if (!type.IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        Debug.LogError($"PowerSingletonManager: Type {type} is not a MonoBehaviour");
                        continue;
                    }
                    
                    PowerSingletonAttribute attribute = (PowerSingletonAttribute) attributes[0];
                    if (!_powerSingletons.TryAddToDictionary(attribute.Type, new PowerSingletonData
                    {
                        GenericType = attribute.Type,
                        Type = type,
                        Creation = attribute.Creation,
                        CreationName = attribute.CreationName,
                        DontDestroyOnLoad = attribute.DontDestroyOnLoad
                    }))
                    {
                        Debug.LogError($"PowerSingletonManager: Duplicate PowerSingletonAttribute for type {attribute.Type}");
                    }
                }
            }
            
            _initialized = true;
        }

        private static void AutoCreate()
        {
            foreach (KeyValuePair<Type, PowerSingletonData> powerSingleton in _powerSingletons)
            {
                if (powerSingleton.Value.Creation != PowerSingletonCreation.Always)
                    continue;
                
                Get(powerSingleton.Key);
            }
        }

        public static Object Get(Type type)
        {
            if (_cachedSingletons.ContainsKey(type) && _cachedSingletons[type] != null)
                return _cachedSingletons[type];

            Object instance = null;
            if (!_powerSingletons.ContainsKey(type))
            {
                instance = Object.FindObjectOfType(type);
                if (instance == null)
                {
                    Debug.LogError($"PowerSingletonManager: No PowerSingletonAttribute for type {type}, and no instance in scene");
                    return null;
                }
                Debug.LogWarning($"PowerSingletonManager: No PowerSingletonAttribute for type {type}, but found instance in scene");
                _cachedSingletons.TryAddToDictionary(type, instance);
                return instance;
            }
            
            PowerSingletonData data = _powerSingletons[type];
            instance = Object.FindObjectOfType(data.Type);
            if (instance == null && data.Creation != PowerSingletonCreation.None)
                instance = new GameObject(data.CreationName ?? data.Type.Name).AddComponent(data.Type);
            _cachedSingletons.TryAddToDictionary(type, instance);
            if (data.DontDestroyOnLoad)
                Object.DontDestroyOnLoad(instance);
            return instance;
        }
    }
}