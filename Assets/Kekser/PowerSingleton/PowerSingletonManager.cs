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
        
        private static Dictionary<Type, List<PowerSingletonData>> _powerSingletons = new Dictionary<Type, List<PowerSingletonData>>();
        private static bool _initialized;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_initialized)
                return;

            LookUpAttributes();
            AutoCreate();
            
            _initialized = true;
        }
        
        private static bool TryAddToDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                return false;
            dictionary.Add(key, value);
            return true;
        }

        //TODO: Add IL Weaving to automatically call this method
        private static void LookUpAttributes()
        {
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
                    _powerSingletons.TryAddToDictionary(attribute.Type, new List<PowerSingletonData>());
                    _powerSingletons[attribute.Type].Add(new PowerSingletonData
                    {
                        GenericType = attribute.Type,
                        Type = type,
                        Creation = attribute.Creation,
                        CreationName = attribute.CreationName,
                        DontDestroyOnLoad = attribute.DontDestroyOnLoad
                    });
                }
            }
        }

        private static void AutoCreate()
        {
            foreach (KeyValuePair<Type, List<PowerSingletonData>> powerSingleton in _powerSingletons)
            {
                foreach (PowerSingletonData data in powerSingleton.Value)
                {
                    if (data.Creation != PowerSingletonCreation.Always)
                        continue;
                    Get(powerSingleton.Key);
                    break;
                }
            }
        }

        public static Object Get(Type type)
        {
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
                return instance;
            }

            PowerSingletonData data = default;
            foreach (PowerSingletonData psData in _powerSingletons[type])
            {
                instance = Object.FindObjectOfType(psData.Type);
                if (instance == null)
                    continue;
                data = psData;
                break;
            }
            
            if (instance == null)
            {
                foreach (PowerSingletonData psData in _powerSingletons[type])
                {
                    if (psData.Creation == PowerSingletonCreation.None)
                        continue;
                    instance = new GameObject(psData.CreationName ?? psData.Type.Name).AddComponent(psData.Type);
                    data = psData;
                    break;
                }
            }
            
            if (instance == null)
            {
                Debug.LogError($"PowerSingletonManager: No PowerSingletonAttribute for type {type}, and no instance in scene");
                return null;
            }
            
            if (data.DontDestroyOnLoad)
                Object.DontDestroyOnLoad(instance);
            return instance;
        }
    }
}