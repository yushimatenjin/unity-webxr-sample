using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CyberSceneGenerator))]
public class CyberSceneGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CyberSceneGenerator generator = (CyberSceneGenerator)target;

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Scene Generation", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Cyberspace Scene", GUILayout.Height(40)))
        {
            generator.GenerateScene();
            EditorUtility.SetDirty(generator);
        }

        EditorGUILayout.Space(5);

        EditorGUILayout.HelpBox(
            "This generates:\n" +
            "- Central monument with orbiting rings\n" +
            "- Floating geometric objects\n" +
            "- Ambient particles\n" +
            "- Sparkle particles around monument\n" +
            "- Light beams\n" +
            "- Dynamic lighting\n\n" +
            "Make sure to assign all materials before generating!",
            MessageType.Info
        );
    }
}

[CustomEditor(typeof(StarfieldGenerator))]
public class StarfieldGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        StarfieldGenerator generator = (StarfieldGenerator)target;

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Generate Starfield", GUILayout.Height(35)))
        {
            generator.GenerateStarfield();
            EditorUtility.SetDirty(generator);
        }

        EditorGUILayout.HelpBox(
            "Creates a spherical starfield around the scene.\n" +
            "Stars twinkle for added atmosphere.",
            MessageType.Info
        );
    }
}
