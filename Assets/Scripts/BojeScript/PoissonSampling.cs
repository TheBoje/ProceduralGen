using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using UnityEngine;

public class PoissonSampling : MonoBehaviour
{
    [Header("Basic Settings")]

    [SerializeField]
    [Tooltip("Seed de génération")]
    private int randomSeed = 1;

    [SerializeField]
    [Range(0f, 100f)]
    [Tooltip("Distance minimale entre chaque points")]
    private float rayonPoisson = 10f;
    // Git master is working aswell ? 
    [SerializeField]
    [Tooltip("Nombre d'essais par nouveau point")]
    private int iterations = 100;

    [SerializeField]
    [Range(2, 2)]
    [Tooltip("WIP - NE PAS MODIFIER")]
    private int dimension = 2;

    [SerializeField]
    [Range(0f, 2000f)]
    [Tooltip("Taille sur X")]
    public int rangeX = 500;

    [SerializeField]
    [Range(0f, 2000f)]
    [Tooltip("Taille sur Z")]
    public int rangeZ = 500;


    [Header("Y Settings")]

    [SerializeField]
    [Tooltip("Active le positionnement sur Y")]
    private bool scaleYEnable = true;

    [SerializeField]
    [Range(0f, 100f)]
    [Tooltip("Taille sur Y")]
    public float scaleY = 2f;

    [SerializeField]
    [Range(0f, 10f)]
    [Tooltip("Taille du masque de bruit de Perlin")]
    public float perlinScale = 2f;

    [Header("Display Settings")]

    [SerializeField]
    [Tooltip("Active l'instanciation")]
    private bool instanciateEnable = true;

    [SerializeField]
    [Tooltip("Objet instancié a chaque point calculé")]
    private GameObject objetInstance = null;

    /*
    * [Header("Activity Settings")]

    * [SerializeField] private bool activityEnable = true;
    * [SerializeField] private int activityConcentration;
    */

    [Header("Debug Settings")]

    [SerializeField]
    [Tooltip("Active l'affichage des messages de debug dans la console")]
    private bool displayDebugLogs = true;

    // Creation du thread secondaire pour le calcul de poisson
    Thread threadComputePoints;


    private System.Random randGiver; // TODO Creer randomThread.cs car UnityEngine.Random.Range n'est pas autorisé dans un child thread - on utilise donc la bibliotheque C# directement
    private Vector3?[,] grid;
    private int rowsSize;
    private int colsSize;
    private int pointPoissonCount;


    void Update()
    {
        /*  
            Permet d'activer ou désactiver le logging dans la console 
            Attention: a un effet global - quand val=False, rien n'est affiché dans la console
        */
        if (UnityEngine.Debug.unityLogger.logEnabled != displayDebugLogs)
        {
            UnityEngine.Debug.unityLogger.logEnabled = displayDebugLogs;
        }
    }

    /// <summary>Ajoute des points séparés de au moins 2*rayonPoisson dans grid[,]</summary>
    public void computePoints() // TODO poissonManager() qui gere l'appel des fonctions en fonction des booléens
    {


        //==================================================//
        //      Initialisation of all local variables       //
        //==================================================//

        // Contient la liste des points dont la proximité est potentiellement libre
        // Des qu'un point est crée, on le place dans cette liste pour pouvoir placer les points adjascents s'il y a assez de place
        List<Vector3> active = new List<Vector3>();
        // WIP - Liste des points concernant des zones d'activité 
        List<Vector3> activityPoints = new List<Vector3>();

        // Point temporaire aléatoire proche d'un point sélectionné dans activityPoints - représente aussi le premier point placé
        Vector3 newPos = new Vector3();
        // Point pioché dans la liste active
        Vector3 activePos = new Vector3();


        // Initialisation du random (utilisé dans randomRangeFloatThreadSafe et randomRangeIntThreadSafe) utilisant la seed (modifiable dans l'inspecteur)
        randGiver = new System.Random(randomSeed);

        // grid[,] est de taille [rowsSize, colsSize], et chaque cellule est de taille cellSize
        float cellSize = (float)(rayonPoisson / Math.Sqrt(dimension));
        rowsSize = (int)Math.Ceiling((float)rangeX / (float)cellSize);
        colsSize = (int)Math.Ceiling((float)rangeZ / (float)cellSize);

        // Initialisation de la grille 
        grid = new Vector3?[colsSize, rowsSize];
        for (int i = 0; i < colsSize; i++)
        {
            for (int j = 0; j < rowsSize; j++)
            {
                // La grille est initialisée avec null sur chaque case
                grid[i, j] = null;
            }
        }

        // Initialisation du nombre de points calculés
        pointPoissonCount = 0;
        // Initialisation de la stopwatch de la fonction 
        Stopwatch stopwatchPoissonCompute = new Stopwatch();
        // true = premier passage, false = passages suivants
        bool firstRun = true;

        // Premier point placé aléatoirement sur la grille
        newPos = new Vector3(randomRangeFloatThreadSafe(0.0f, (float)rangeX), 0f, randomRangeFloatThreadSafe(0.0f, (float)rangeZ));
        // On utilise la position par rapport a cellSize pour placer le point dans la grille
        Vector3Int newPosFloored = floorVector3(newPos, cellSize);
        grid[newPosFloored.x, newPosFloored.z] = newPos;
        // On ajoute le point a la liste des points dont des cases adjascentes sont potentiellement libres
        active.Add(newPos);


        //==================================================//
        //          Actual algorithm                        //
        //==================================================//

        // Depart de la stopwatch
        stopwatchPoissonCompute.Start();


        while (active.Count > 0 || firstRun)
        {
            // Permet de vérifier si le point n'a AUCUNE de place libre adjascentes
            bool isFound = false;
            // Choix d'un point aléatoire dans la liste des points ayant possiblement des places adjascentes libres
            int randomIndex = randomRangeIntThreadSafe(0, active.Count);
            activePos = active[randomIndex];
            firstRun = false;
            // Tentative de placement d'un point de position aléatoire (iterations-fois maximum) dans la proximitée de activePos 
            for (int n = 0; n < iterations; n++)
            {
                // Vecteur aléatoire déterminant une direction 
                newPos = new Vector3(randomRangeFloatThreadSafe(-1.0f, 1.0f), 0f, randomRangeFloatThreadSafe(-1.0f, 1.0f)).normalized;

                float randomMagnitude = randomRangeFloatThreadSafe(0.0f, (float)(2 * rayonPoisson));
                // Multiplié par une magnitude
                newPos = newPos * randomMagnitude;
                // Auquel on ajoute le point cible (activePos)
                newPos += (Vector3)activePos;
                // Calcul de sa position dans la grille
                newPosFloored = floorVector3((Vector3)newPos, cellSize);
                // Vérification que newPos est dans [0, rangeX | 0, rangeY] et que la case dans la grille est libre
                if (0 <= newPos.x && newPos.x < rangeX && 0 <= newPos.z && newPos.z < rangeZ && 0 <= newPosFloored.x && newPosFloored.x < colsSize && 0 <= newPosFloored.z && newPosFloored.z < rowsSize && grid[newPosFloored.x, newPosFloored.z] == null)
                {
                    bool isCorrectDistance = true;
                    // Verification que tous les points dans les cases adjascentes de la grille sont a distance suffisante
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (newPosFloored.x + i >= 0 && newPosFloored.x + i < colsSize && newPosFloored.z + j >= 0 && newPosFloored.z + j < rowsSize && grid[newPosFloored.x + i, newPosFloored.z + j] != null)
                            {
                                float distance = Vector3.Distance(newPos, (Vector3)grid[newPosFloored.x + i, newPosFloored.z + j]);
                                if (distance < 2 * rayonPoisson)
                                {
                                    isCorrectDistance = false;
                                    break;
                                }
                            }
                        }
                    }
                    // Si le point aléatoire newPos est correct, alors on le place dans la grille et on l'ajoute dans la liste des points actifs
                    if (isCorrectDistance)
                    {
                        grid[newPosFloored.x, newPosFloored.z] = newPos;
                        isFound = true;
                        active.Add(newPos);
                        pointPoissonCount += 1;
                    }
                }
            }
            if (!isFound)
            {
                // S'il n'y a pas de place a côté du point, on le retire des actifs
                active.Remove(active[randomIndex]);
            }
        }
        // Application d'un bruit de perlin
        if (scaleYEnable)
        {
            computePointsHeight();
        }
        // Arret de la stopwatch
        stopwatchPoissonCompute.Stop();

        UnityEngine.Debug.Log("PoissonSampling::computePoints - Computed " + pointPoissonCount + " points in " + stopwatchPoissonCompute.ElapsedMilliseconds + " ms | " + (float)stopwatchPoissonCompute.ElapsedMilliseconds / (float)pointPoissonCount + "ms / pt");

    }

    /// <summary>Lance la fonction computePoints() dans un thread different en mode background</summary>
    public IEnumerator threadedComputePoints()
    {

        // Supprime les points déjà instanciés (contenus dans la liste instanciatedPoints)
        StartCoroutine(deleteComputed());
        // Initialisation du nouveau thread et mise en mode arrière plan
        threadComputePoints = new Thread(computePoints);
        // Depart du thread
        threadComputePoints.Start();
        threadComputePoints.IsBackground = true;
        UnityEngine.Debug.Log("PoissonSampling::threadedComputePoints - Starting");
        // IEnumerator stuff: reprend la corroutine juste après le yield a chaque frame, nécessaire
        while (threadComputePoints.IsAlive)
        {
            yield return null;
        }
        // Affichage des points
        if (!threadComputePoints.IsAlive && instanciateEnable)
        {
            StartCoroutine(displayGrid());
        }
        UnityEngine.Debug.Log("PoissonSampling::threadedComputePoints - Finished");
    }


    /// <summary>Float random [a, b[</summary>
    // Remplace UnityEngine.Random.Range(a,b) car non accessible dans un thread alternatif
    public float randomRangeFloatThreadSafe(float a, float b)
    {
        float result;
        result = a + (float)randGiver.NextDouble() * (b - a);
        return result;
    }

    /// <summary>Int random [a, b]</summary>
    // Remplace UnityEngine.Random.Range(a,b) car non accessible dans un thread alternatif
    public int randomRangeIntThreadSafe(int a, int b)
    {
        int result;
        result = randGiver.Next(a, b);
        return result;
    }

    /// <summary>Instancie tous les points présents dans grid en tant qu'enfant de GenManager</summary>
    public IEnumerator displayGrid()
    {
        Stopwatch stopwatchDisplayGrid = new Stopwatch();
        int internaldisplayGridCount = 0;
        stopwatchDisplayGrid.Start();
        for (int i = 0; i < colsSize; i++)
        {
            for (int j = 0; j < rowsSize; j++)
            {
                if (grid[i, j] != null)
                {
                    GameObject resultInstance = Instantiate(objetInstance, (Vector3)grid[i, j], Quaternion.identity);
                    resultInstance.transform.parent = gameObject.transform.Find("PoissonSampling").transform;
                    internaldisplayGridCount += 1;
                    if (internaldisplayGridCount > 100)
                    {
                        internaldisplayGridCount = 0;
                        yield return null;
                    }
                }
            }
        }
        stopwatchDisplayGrid.Stop();

        UnityEngine.Debug.Log("PoissonSampling::displayGrid - Placed " + pointPoissonCount + " points in  " + stopwatchDisplayGrid.ElapsedMilliseconds + " ms | " + (float)stopwatchDisplayGrid.ElapsedMilliseconds / (float)pointPoissonCount + "ms / pt");
    }

    /// <summary>Instancie le point newPos en tant qu'enfant de GenManager</summary>
    private void displayPoint(Vector3 newPos)
    {
        if (instanciateEnable)
        {
            Vector3 pt = newPos;
            GameObject resultInstance = Instantiate(objetInstance, pt, Quaternion.identity);
            resultInstance.transform.parent = gameObject.transform;
        }
    }

    /// <summary>Supprime l'ensemble des points instanciés contenus dans la liste instanciatedPoints</summary>
    public IEnumerator deleteComputed()
    {
        int internalDeleteComputedCount = 0;
        Transform intersectionTransform = transform.Find("PoissonSampling");
        for (int i = intersectionTransform.childCount; i > 0; i--)
        {
            Destroy(intersectionTransform.GetChild(i - 1).gameObject);
            internalDeleteComputedCount += 1;
            if (internalDeleteComputedCount > 100)
            {
                internalDeleteComputedCount = 0;
                yield return null;
            }
        }
    }

    /// <summary>Applique a tous les points de grid[,] une hauteur provenant d'un bruit de Perlin
    /// Parametrable depuis l'inspecteur dans GenManager</summary>
    // Note: is thread friendly
    public void computePointsHeight()
    {
        for (int i = 0; i < colsSize; i++)
        {
            for (int j = 0; j < rowsSize; j++)
            {
                if (grid[i, j] != null)
                {
                    Vector3 temp = (Vector3)grid[i, j];
                    // Récupération de la valeur dans le bruit de Perlin par rapport a sa position (*scaleY)
                    temp.y = perlinNoiseGeneratePoint(temp.x, temp.z, rangeX, rangeZ, perlinScale) * scaleY;
                    // Remise dans la grid 
                    grid[i, j] = temp;
                }
            }
        }
    }

    /// <summary>Calcule le vecteur de la position dans la grille (par rapport a cellSize) du point vec</summary>
    public Vector3Int floorVector3(Vector3 vec, float cellSize)
    {
        Vector3Int result;
        result = new Vector3Int((int)Math.Floor(vec.x / cellSize), 0, (int)Math.Floor(vec.z / cellSize));
        return result;
    }

    /// <summary>Calcule la valeur du point (x,y) dans un plan de taille (width, height)*scale</summary>
    private float perlinNoiseGeneratePoint(float x, float y, float width, float height, float scale)
    {
        return Mathf.PerlinNoise((float)((x / width) * scale), (float)((y / height) * scale));
    }

    /// <summary>Getter de la grid contenant tous les points de PoissonDiskSampling (calculée dans computePoints)</summary>
    public Vector3?[,] poissonGrid
    {
        get { return grid; }
    }
    public int getRowSize
    {
        get { return rowsSize; }
    }
    public int getColSize
    {
        get { return colsSize; }
    }
}
