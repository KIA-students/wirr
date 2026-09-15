using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PhysicsLoadGenerator))]
public class PhysicsLoadGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PhysicsLoadGenerator generator =
            (PhysicsLoadGenerator)target;

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Physics Load Control",
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

        EditorGUILayout.LabelField(
            "Object Count",
            EditorStyles.boldLabel
        );

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(
            $"- {generator.ObjectsPerStep}",
            GUILayout.Height(30)))
        {
            generator.RemoveObjects();
        }

        if (GUILayout.Button(
            $"+ {generator.ObjectsPerStep}",
            GUILayout.Height(30)))
        {
            generator.AddObjects();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(6);

        EditorGUILayout.LabelField(
            "Object Scale",
            EditorStyles.boldLabel
        );

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(
            $"- {generator.ScaleStep:F2}",
            GUILayout.Height(28)))
        {
            Undo.RecordObject(
                generator,
                "Decrease physics object scale"
            );

            generator.DecreaseScale();

            EditorUtility.SetDirty(generator);
        }

        if (GUILayout.Button(
            $"+ {generator.ScaleStep:F2}",
            GUILayout.Height(28)))
        {
            Undo.RecordObject(
                generator,
                "Increase physics object scale"
            );

            generator.IncreaseScale();

            EditorUtility.SetDirty(generator);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(8);

        if (GUILayout.Button(
            "Apply Rigidbody Settings",
            GUILayout.Height(28)))
        {
            generator.ApplyRigidbodySettings();
        }

        if (GUILayout.Button(
            "Reset Experiment",
            GUILayout.Height(28)))
        {
            generator.ResetExperiment();
        }

        EditorGUILayout.Space(8);

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox(
                "Impulse is available in Play Mode and on the XR device.",
                MessageType.Info
            );
        }

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button(
            $"Impulse All ({generator.ImpulseStrength:F1})",
            GUILayout.Height(30)))
        {
            generator.ImpulseAll();
        }

        GUI.enabled = true;

        EditorGUILayout.Space(8);

        if (GUILayout.Button(
            "Clear Objects",
            GUILayout.Height(28)))
        {
            bool confirmed =
                EditorUtility.DisplayDialog(
                    "Clear physics objects",
                    "Remove all generated physics objects?",
                    "Clear",
                    "Cancel"
                );

            if (confirmed)
                generator.ClearObjects();
        }
    }
}
