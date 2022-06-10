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

        int totalChanceCommon = serializedObject.FindProperty("totalChanceCommon").intValue;
        int totalChanceRare = serializedObject.FindProperty("totalChanceRare").intValue;
        int totalChanceDrops = serializedObject.FindProperty("totalChanceDrops").intValue;

        var style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 18;

        GUILayout.Label($"Total Chance Common = {totalChanceCommon}", style);

        if (totalChanceCommon != 100)
            EditorGUILayout.HelpBox("La percentuale totale non è 100", MessageType.Error);

        GUILayout.Label($"Total Chance Rare = {totalChanceRare}", style);

        if (totalChanceRare != 100)
            EditorGUILayout.HelpBox("La percentuale totale non è 100", MessageType.Error);

        GUILayout.Label($"Total Chance Drops = {totalChanceDrops}", style);

        if (totalChanceDrops != 100)
            EditorGUILayout.HelpBox("La percentuale totale non è 100", MessageType.Error);

    }

}
