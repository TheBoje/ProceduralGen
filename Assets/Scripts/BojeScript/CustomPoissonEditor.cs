using UnityEditor;
using UnityEngine;

// Ajoute des fonctionnalités dans l'inspecteur pour le script de type PoissonSampling
[CustomEditor(typeof(PoissonSampling))]
public class CustomPoissonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // On dessine l'inspecteur standard
        DrawDefaultInspector();
        // Recuperation du script utilisé
        PoissonSampling poissonScript = (PoissonSampling)target;
        // Chaque condition dessine un bouton execute le code mis en <then>

        if (GUILayout.Button("Poisson Sampling"))
        {
            // appel de la fonction threadedComputePoints en Coroutine car la fonction retourne un IEnumerator
            poissonScript.StartCoroutine(poissonScript.threadedComputePoints());
        }

        if (GUILayout.Button("Delete Instanciated"))
        {
            poissonScript.StartCoroutine(poissonScript.deleteComputed());
        }

        if (GUILayout.Button("Display Points"))
        {
            poissonScript.StartCoroutine(poissonScript.displayGrid());
        }
    }
}