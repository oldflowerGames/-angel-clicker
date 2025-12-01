using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if (UNITY_EDITOR)

[CustomEditor(typeof(KeepSpriteSize))]
public class KeepSpriteSizeInspector : Editor
{
    KeepSpriteSize manager;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        manager = target as KeepSpriteSize;
        if (GUILayout.Button("Set Sprite Size"))
        {
            Undo.RecordObject(manager.gameObject, "Set Size");
            manager.SetSize();
            EditorUtility.SetDirty(manager);
        }
    }
}
#endif
