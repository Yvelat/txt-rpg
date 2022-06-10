using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonsterBase))]
public class MonsterBaseEditor : Editor
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
            EditorGUILayout.HelpBox("La percentuale totale non è 100", MessageType.Error);

    }
}
