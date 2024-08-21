using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActionDoPunch))]
public class ActionInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Punch Scale"))
        {
            foreach (var temp in targets)
            {
                ActionScript myScript = (ActionScript)temp;
                myScript.StartAction();
            }
        }

        DrawDefaultInspector();
    }
}

[CustomEditor(typeof(ActionReceive))]
public class ActionReceiveInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Start Action"))
        {
            foreach (var temp in targets)
            {
                ActionScript myScript = (ActionScript)temp;
                myScript.StartAction();
            }
        }

        DrawDefaultInspector();
    }
}