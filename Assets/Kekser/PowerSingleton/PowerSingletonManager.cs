using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Kekser.PowerSingleton.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kekser.PowerSingleton
{
    public static class PowerSingletonManager
    {
        private const string AssembliesToIgnoreRegex = @"^Unity\.|^UnityEngine\.|^mscorlib|^System\.|^Mono\.";
        
        private const string PowerSingletonNoMonoBehaviour = "PowerSingletonManager: Type {0} is not a MonoBehaviour";
        private const string NoPowerSingletonAttribute = "PowerSingletonManager: No PowerSingletonAttribute for type {0}, and no instance in scene";
        private const string NoPowerSingletonAttributeButInstanceInScene = "PowerSingletonManager: No PowerSingletonAttribute for type {0}, but found instance in scene";
        
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
        public static void Initialize()
        {
            if (!_initialized)
            {
                LookUpAttributes();
                UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (arg0, arg1) =>
                {
                    Initialize();
                };
                _initialized = true;
            }
            AutoCreate();
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
                if (Regex.IsMatch(assembly.FullName, AssembliesToIgnoreRegex))
                    continue;

                foreach (Type type in assembly.GetTypes())
                {
                    var attributes = type.GetCustomAttributes(typeof(PowerSingletonAttribute), false);
                    if (attributes.Length <= 0) continue;
                    
                    if (!type.IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        Debug.LogErrorFormat(PowerSingletonNoMonoBehaviour, type);
                        continue;
                    }
                    
                    PowerSingletonAttribute attribute = (PowerSingletonAttribute) attributes[0];
                    Type attributeType = attribute.Type ?? type;
                    _powerSingletons.TryAddToDictionary(attributeType, new List<PowerSingletonData>());
                    _powerSingletons[attributeType].Add(new PowerSingletonData
                    {
                        GenericType = attributeType,
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
                }
            }
        }

        public static Object Get(Type type)
        {
            Object instance = null;
            if (!_powerSingletons.ContainsKey(type))
            {
                if (!type.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    Debug.LogErrorFormat(PowerSingletonNoMonoBehaviour, type);
                    return null;
                }
                
                instance = Object.FindObjectOfType(type);
                if (instance == null)
                {
                    Debug.LogErrorFormat(NoPowerSingletonAttribute, type);
                    return null;
                }
                Debug.LogWarningFormat(NoPowerSingletonAttributeButInstanceInScene, type);
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
                Debug.LogErrorFormat(NoPowerSingletonAttribute, type);
                return null;
            }
            
            if (data.DontDestroyOnLoad)
                Object.DontDestroyOnLoad(instance);
            return instance;
        }
    }
}