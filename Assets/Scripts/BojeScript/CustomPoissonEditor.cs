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
            poissonScript.deleteComputed();
            poissonScript.computePoints();
        }
        if (GUILayout.Button("Delete Instanciated"))
        {
            poissonScript.deleteComputed();
        }
    }
}
