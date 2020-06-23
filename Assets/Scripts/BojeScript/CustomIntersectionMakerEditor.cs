using UnityEditor;
using UnityEngine;

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
            intersectionScript.ComputeRoad(false);
            //intersectionScript.StartCoroutine(intersectionScript.threadedComputeRoad());
        }

        if (GUILayout.Button("Delete Roads"))
        {
            //intersectionScript.ClearInstanciated();
        }

        if (GUILayout.Button("Add Houses"))
        {
            halfEdgesMap.ExtrudeAllFaces(50f, 200f);
        }
    }
}