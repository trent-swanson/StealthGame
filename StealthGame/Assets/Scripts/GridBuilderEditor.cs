using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateGrid))]
public class GridBuilderEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        GUIStyle customButton = new GUIStyle("button");
        customButton.fontSize = 17;
        customButton.fontStyle = FontStyle.Bold;

        GenerateGrid myScript = (GenerateGrid)target;
        GUILayout.Space(40);
        if (GUILayout.Button("Generate Grid", customButton, GUILayout.Height(45))) {
            myScript.Generate();
        }
        GUILayout.Space(15);
    }
}
