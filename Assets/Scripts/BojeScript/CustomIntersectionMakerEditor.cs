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
            //intersectionScript.ClearIntersections();
            //poissonScript.deleteComputed();
            //poissonScript.StartCoroutine(poissonScript.threadedComputePoints());
            intersectionScript.ComputeRoad(false);
        }

        if(GUILayout.Button("Delete Triangles"))
        {
            intersectionScript.ClearIntersections();
            intersectionScript.ComputeRoad(true);
        }

        if (GUILayout.Button("Delete Roads"))
        {
            intersectionScript.ClearIntersections();
        }
    }
}
