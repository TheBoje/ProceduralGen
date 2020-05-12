using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(IntersectionMaker))]
public class CustomIntersectionMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PoissonSampling poissonScript = (PoissonSampling)GameObject.Find("GenManager").GetComponent<PoissonSampling>();
        IntersectionMaker intersectionScript = (IntersectionMaker)target;
        if (GUILayout.Button("Generate Roads"))
        {
            intersectionScript.ClearIntersections();
            poissonScript.deleteComputed();
            intersectionScript.ComputeRoad();
        }

        if (GUILayout.Button("Delete Roads"))
        {
            intersectionScript.ClearIntersections();
        }
    }
}
