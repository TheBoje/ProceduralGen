using System.Collections;
using System.Collections.Generic;
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
            poissonScript.deleteComputed();
            intersectionScript.ComputeRoad();
        }
    }
}
