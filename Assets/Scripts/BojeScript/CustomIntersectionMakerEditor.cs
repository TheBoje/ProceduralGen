using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEditor;

// Ajoute des fonctionnalités dans l'inspecteur pour le script de type IntersectionMaker
[CustomEditor(typeof(IntersectionMaker))]
public class CustomIntersectionMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // On dessine l'inspecteur standard
        DrawDefaultInspector();
        // Recuperation des differents scripts 
        PoissonSampling poissonScript = (PoissonSampling)GameObject.Find("GenManager").GetComponent<PoissonSampling>();
        Town townScript = (Town)GameObject.Find("GenManager").GetComponent<Town>();
        HalfEdgesMap halfEdgesMap = (HalfEdgesMap)GameObject.Find("HalfEdge").GetComponent<HalfEdgesMap>();
        IntersectionMaker intersectionScript = (IntersectionMaker)target;
        // Chaque condition dessine un bouton execute le code mis en <then>
        if (GUILayout.Button("Generate Roads"))
        {
            //intersectionScript.ClearIntersections();
            //poissonScript.deleteComputed();
            //poissonScript.StartCoroutine(poissonScript.threadedComputePoints());
            intersectionScript.ComputeRoad(false);
        }

        if (GUILayout.Button("Delete Triangles"))
        {
            intersectionScript.ClearIntersections();
            intersectionScript.ComputeRoad(true);
        }

        if (GUILayout.Button("Delete Roads"))
        {
            intersectionScript.ClearIntersections();
        }

        if(GUILayout.Button("Add Houses"))
        {
            townScript.BuildTown();
        }
    }
}
