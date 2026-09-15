using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RenderLoadGenerator))]
public class RenderLoadGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RenderLoadGenerator generator =
            (RenderLoadGenerator)target;

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Scene Load Control",
            EditorStyles.boldLabel
        );

        EditorGUILayout.LabelField(
            "Current objects",
            generator.transform.childCount.ToString()
        );

        EditorGUILayout.LabelField(
            "Current scale",
            generator.ObjectScale.ToString("F2")
        );

        EditorGUILayout.Space(6);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(
            $"- Scale ({generator.ScaleStep:F2})",
            GUILayout.Height(30)))
        {
            Undo.RecordObject(generator, "Decrease Object Scale");
            generator.DecreaseScale();
            EditorUtility.SetDirty(generator);
        }

        if (GUILayout.Button(
            $"+ Scale ({generator.ScaleStep:F2})",
            GUILayout.Height(30)))
        {
            Undo.RecordObject(generator, "Increase Object Scale");
            generator.IncreaseScale();
            EditorUtility.SetDirty(generator);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(6);

        if (GUILayout.Button(
            $"+ Add {generator.ObjectsPerStep} Objects",
            GUILayout.Height(32)))
        {
            generator.AddObjects();
        }

        EditorGUILayout.Space(4);

        if (GUILayout.Button(
            "Clear Objects",
            GUILayout.Height(26)))
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Clear generated objects",
                "Remove all generated test objects?",
                "Clear",
                "Cancel"
            );

            if (confirmed)
                generator.ClearObjects();
        }
    }
}
