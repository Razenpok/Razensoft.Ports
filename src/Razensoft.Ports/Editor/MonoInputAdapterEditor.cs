using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Razensoft.Ports
{
    [CustomEditor(typeof(MonoInputAdapter), true)]
    public class MonoInputAdapterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Connect"))
            {
                Connect();
            }
        }
        
        private MonoInputAdapter Adapter => (MonoInputAdapter) target;

        private void Connect()
        {
            if (Adapter.TryGetComponent<Button>(out var button))
            {
                Connect(Adapter, button.onClick);
                return;
            }

            var components = Adapter.GetComponents<Component>();
            foreach (var component in components)
            {
                if (TryConnect(component))
                {
                    return;
                }
            }
        }

        private bool TryConnect(Component component)
        {
            var type = component.GetType();
            var publicEvents = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            if (publicEvents.Any(m => TryConnect(component, m)))
            {
                return true;
            }

            var privateEvents = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.GetCustomAttribute<SerializeField>() != null)
                .ToArray();
            if (privateEvents.Any(m => TryConnect(component, m)))
            {
                return true;
            }

            return false;
        }

        private bool TryConnect(Component component, FieldInfo member)
        {
            if (!typeof(UnityEventBase).IsAssignableFrom(member.FieldType))
            {
                return false;
            }

            var @event = member.GetValue(component) as UnityEventBase;
            if (@event == null)
            {
                return false;
            }

            Connect(component, @event);
            return true;
        }

        private void Connect(Component component, UnityEventBase @event)
        {
            MonoInputAdapterConnector.Connect(Adapter, component, @event);
        }
    }
}
