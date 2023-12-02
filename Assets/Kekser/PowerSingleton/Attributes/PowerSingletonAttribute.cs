using System;

namespace Game.Scripts.PowerSingleton.Attributes
{
    public enum PowerSingletonCreation
    {
        None,
        IfNeeded,
        Always
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class PowerSingletonAttribute : Attribute
    {
        private Type _type;
        private PowerSingletonCreation _creation = PowerSingletonCreation.None;
        private string _creationName = null;
        private bool _dontDestroyOnLoad = false;
        
        public PowerSingletonAttribute(Type type = null)
        {
            _type = type;
        }
        
        public Type Type => _type;

        public PowerSingletonCreation Creation
        {
            get => _creation;
            set => _creation = value;
        }
        
        public string CreationName
        {
            get => _creationName;
            set => _creationName = value;
        }

        public bool DontDestroyOnLoad
        {
            get => _dontDestroyOnLoad;
            set => _dontDestroyOnLoad = value;
        }
    }
}