using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PoissonSampling))]
public class CustomPoissonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PoissonSampling poissonScript = (PoissonSampling)target;
        if (GUILayout.Button("Poisson Sampling"))
        {
            poissonScript.StartCoroutine(poissonScript.threadedComputePoints());
        }
        if (GUILayout.Button("Delete Instanciated"))
        {
            poissonScript.deleteComputed();
        }
        if (GUILayout.Button("Display Points"))
        {
            poissonScript.displayGrid();
        }
    }
}
