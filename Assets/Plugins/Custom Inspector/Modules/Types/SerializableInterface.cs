using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CustomInspector
{
    [System.Serializable]
    public class SerializableInterface<T> : ISerializationCallbackReceiver
                                  where T : class
    {
        [MessageBox("You are overriding the default drawer of SerializableInterface. Please add the [Interface]-attribute to the property.")]
        [ReadOnly]
        [SerializeField] MonoBehaviour serializedReference = null; // MonoBehaviour, because UnityEngine.Object or Component bugs

        public T Value
        {
            get => castedReference;
            set
            {
                if (value == null)
                    this.serializedReference = null;
                else if (value is MonoBehaviour m)
                    this.serializedReference = m;
                else
                    throw new ArgumentException($"Unity Serialization only supports classes derived from {typeof(MonoBehaviour).FullName}." +
                                                $"\n{value.GetType().FullName} does not derive from it.");
            }
        }
        private T castedReference = null;


        public SerializableInterface()
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException($"SerializableInterface is only valid on Interfaces. " +
                                            $"T({typeof(T).FullName}) is not an Interface");
        }
        public SerializableInterface(MonoBehaviour value)
        : this()
        {
            if (value == null)
                return;

            if (value is T t)
            {
                this.serializedReference = value;
                Value = t;
            }
            else
            {
                throw new ArgumentException($"Could not set value: value is not type of {typeof(T).FullName}");
            }
        }

        //serialization
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {

        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (serializedReference == null)
            {
                castedReference = null;
            }
            else if (serializedReference is T casted)
            {
                castedReference = casted;
            }
            else
            {
                Debug.LogError($"SerializedValue on SerializableInterface is not type of {typeof(T).FullName}. Value gets discarded.");
                serializedReference = null;
                castedReference = null;
            }
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class InterfaceAttribute : PropertyAttribute { }
}
