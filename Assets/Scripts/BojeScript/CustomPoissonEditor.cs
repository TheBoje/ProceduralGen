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

        PoissonSampling myScript = (PoissonSampling)target;
        if (GUILayout.Button("Poisson Sampling"))
        {
            myScript.deleteComputed();
            myScript.computePoints();
        }
        if (GUILayout.Button("Delete Instanciated"))
        {
            myScript.deleteComputed();
        }
    }
}
