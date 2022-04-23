#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Razensoft.Ports
{
    public class MonoInputAdapterConnector
    {
        public static void Connect(MonoInputAdapter adapter, Component component, UnityEventBase @event)
        {
            Undo.RecordObject(component, "Connect UnityEvent with MonoInputAdapter");
            var eventCount = @event.GetPersistentEventCount();
            UnityEventTools.AddPersistentListener(@event);
            UnityEventTools.RegisterVoidPersistentListener(@event, eventCount, adapter.Invoke);

            var scene = PrefabStageUtility.GetCurrentPrefabStage()?.scene ?? SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
        }
    }
}
#endif
