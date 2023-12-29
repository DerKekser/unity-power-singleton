using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kekser.PowerSingleton.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Kekser.PowerSingleton
{
    public static class PowerSingletonManager
    {
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
        
        private static readonly Dictionary<Type, List<PowerSingletonData>> _powerSingletons = new Dictionary<Type, List<PowerSingletonData>>();
        private static bool _initialized;
        private static bool _quitting;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            if (!_initialized)
            {
                LookUpAttributes();
                SceneManager.activeSceneChanged += (arg0, arg1) => Initialize();
                Application.quitting += Quitting;
                _initialized = true;
            }
            AutoCreate();
        }
        
        private static void Quitting()
        {
            _quitting = true;
        }

        private static bool TryAddToDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                return false;
            dictionary.Add(key, value);
            return true;
        }
        
        private static bool TryToFindObjectOfType(Type type, out Object instance)
        {
            instance = Object.FindObjectOfType(type);
            return instance != null;
        }
        
        private static bool CheckType(Type type)
        {
            if(type.IsSubclassOf(typeof(MonoBehaviour)))
                return true;
            Debug.LogErrorFormat(PowerSingletonNoMonoBehaviour, type);
            return false;
        }

        //TODO: Add IL Weaving to automatically call this method
        private static void LookUpAttributes()
        {
            string definedIn = typeof(PowerSingletonAttribute).Assembly.GetName().Name;
            
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GlobalAssemblyCache 
                    || assembly.GetName().Name != definedIn 
                    && assembly.GetReferencedAssemblies().All(assemblyName => assemblyName.Name != definedIn))
                    continue;

                foreach (Type type in assembly.GetTypes())
                {
                    object[] attributes = type.GetCustomAttributes(typeof(PowerSingletonAttribute), false);
                    if (attributes.Length == 0 || !CheckType(type))
                        continue;
                    
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
                if (powerSingleton.Value.Any(data => data.Creation == PowerSingletonCreation.Always))
                    Get(powerSingleton.Key);
            }
        }

        public static Object Get(Type type)
        {
            if (_quitting)
                return null;
            
            Object instance = null;
            if (!_powerSingletons.ContainsKey(type))
            {
                if (!CheckType(type))
                    return null;
                
                if (!TryToFindObjectOfType(type, out instance))
                {
                    Debug.LogErrorFormat(NoPowerSingletonAttribute, type);
                    return null;
                }
                Debug.LogWarningFormat(NoPowerSingletonAttributeButInstanceInScene, type);
                return instance;
            }

            PowerSingletonData data = _powerSingletons[type].FirstOrDefault(psData => TryToFindObjectOfType(psData.Type, out instance));
            
            if (instance == null)
            {
                data = _powerSingletons[type].FirstOrDefault(psData => psData.Creation != PowerSingletonCreation.None);
                if (data.Creation == PowerSingletonCreation.None)
                {
                    Debug.LogErrorFormat(NoPowerSingletonAttribute, type);
                    return null;
                }
                instance = new GameObject(data.CreationName ?? data.Type.Name).AddComponent(data.Type);
            }
            
            if (data.DontDestroyOnLoad)
                Object.DontDestroyOnLoad(instance);
            return instance;
        }
    }
}