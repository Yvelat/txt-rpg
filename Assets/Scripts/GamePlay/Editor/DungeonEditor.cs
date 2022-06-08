using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Dungeon))]
public class DungeonEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        int totalChance = serializedObject.FindProperty("totalChance").intValue;

        var style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 18;

        GUILayout.Label($"Total Chance = {totalChance}", style);

        if (totalChance != 100)
            EditorGUILayout.HelpBox("La percentuale totale non � 100", MessageType.Error);

    }

}
