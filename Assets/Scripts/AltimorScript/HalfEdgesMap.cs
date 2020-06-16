using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

// Créer un triangle isolé

public class HalfEdgesMap : MonoBehaviour
{
    private List<HalfEdge> m_halfEdges;
    private List<Vector3> m_positions;
    static Material lineMaterial; // utilisé pour afficher le debug


    public void Init()
    {
        m_halfEdges = new List<HalfEdge>();
        m_positions = new List<Vector3>();
    }

    //  
    private void AddIsolatedDart(int indexPosition)
    {
        // TODO Le créer directement opposés
        HalfEdge dart = new HalfEdge(m_halfEdges.Count, m_halfEdges.Count, m_halfEdges.Count + 1, indexPosition);
        m_halfEdges.Add(dart);
        HalfEdge opposite = new HalfEdge(m_halfEdges.Count, m_halfEdges.Count, m_halfEdges.Count - 1, indexPosition);
        m_halfEdges.Add(opposite);
        
    }

    // Ajoute un point à la carte (isolé dans la carte)
    public void AddPoint(Vector3 position)
    {
        AddIsolatedDart(m_positions.Count);
        m_positions.Add(position);
    }

    // Retourne l'index d'un brin lié au point passé en paramètre
    private int IndexOfDartByPoint(int pointIndex)
    {
        for(int i = 0; i < m_halfEdges.Count; i++)
        {
            if (m_halfEdges[i].Position == pointIndex)
                return i;
        }

        return -1;
    }

    // Retourne le brin du point suivant
    public int NextDartOnPoint(int dart)
    {
        return m_halfEdges[m_halfEdges[dart].Opposite].Next;
    }

    // Calcul l'angle sur l'axe des Y : retourne l'angle en degré
    private float ComputeAngle(Vector3 from, Vector3 to)
    {
        return Vector3.Angle(new Vector3(from.x, 0f, from.z), new Vector3(to.x, 0f, to.z));
    }

    // Calcul les angles par rapport à l'horizontal pour trouver entre quel et quel brin nous devons mettre le nouveau brin : retourne le futur suivant (de pos1 vers pos2)
    private int ComputePreviousDart(int pos1, int pos2)
    {
        int previous = IndexOfDartByPoint(pos1); // Index du brin qui précèdera le prin allant de pos1 à pos2
        int maxAngleIndex = previous; // Index du brin formant l'angle maximum par rapport à l'horizontal
        int currentIndex = previous; // Index étant en train d'être étudié
        int firstIndex = previous; // premier index étudié

        // On calcul l'angle du vecteur par rapport à l'horizontal pour pouvoir le classer par rapport aux autres brins issus du point pos1
        float angle = ComputeAngle(m_positions[pos2] - m_positions[pos1], Vector3.forward);
        float maxAngle = angle;
        float minAngle = angle;

        while(firstIndex != currentIndex)
        {
            float currentAngle = ComputeAngle(m_positions[m_halfEdges[m_halfEdges[currentIndex].Opposite].Position] - m_positions[m_halfEdges[currentIndex].Position], Vector3.forward);
            if (angle > currentAngle)
            {
                previous = currentIndex;
            }

            if(maxAngle < currentAngle)
            {
                maxAngle = currentAngle;
                maxAngleIndex = currentIndex;
            }

            if (minAngle > currentAngle)
            {
                minAngle = currentAngle;
            }
        }

        if (minAngle == angle)
        {
            previous = maxAngleIndex;
        }

        return m_halfEdges[previous].Previous;
    }



    // Relie un brin à deux autres
    private void LinkDart(int previousA, int previousB, int A, int B)
    {
        int indexAB = m_halfEdges.Count;
        int indexBA = m_halfEdges.Count + 1;

        // de A vers B
        HalfEdge AB = new HalfEdge(m_halfEdges[previousB].Next, previousA, indexBA, A);
        m_halfEdges.Add(AB);

        // De B vers A
        HalfEdge BA = new HalfEdge(m_halfEdges[previousA].Next, previousB, indexAB, B);
        m_halfEdges.Add(BA);

        m_halfEdges[m_halfEdges[previousA].Next].Previous = indexBA;
        m_halfEdges[m_halfEdges[previousB].Next].Previous = indexAB;

        m_halfEdges[previousA].Next = indexAB;
        m_halfEdges[previousB].Next = indexBA;        
    }

    // Relie deux points en prenant leurs position en paramètre
    private void LinkTwoPoints(int A, int B)
    {
        int previousA = ComputePreviousDart(A, B);
        int previousB = ComputePreviousDart(B, A);

        LinkDart(previousA, previousB, A, B);
    }









    // Dessine une face à l'aide d'un Line renderer
    private List<int> ComputePointsFace(int firstEdge, List<HalfEdge> printList)
    {
        int currentIndex = firstEdge;
        List<int> indexPoints = new List<int>();

        indexPoints.Add(m_halfEdges[firstEdge].Position);

        do
        {
            currentIndex = m_halfEdges[currentIndex].Next;
            indexPoints.Add(m_halfEdges[currentIndex].Position);
            printList.Remove(m_halfEdges[currentIndex]);
        } while (currentIndex != firstEdge);

        return indexPoints;
    }



    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    // Dessine la carte
    public void OnRenderObject()
    {
        CreateLineMaterial();
        lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);

        List<HalfEdge> copy = new List<HalfEdge>(m_halfEdges);
        List<List<int>> facesList = new List<List<int>>(); // Liste des faces

        while(copy.Count > 0)
        {
            facesList.Add(ComputePointsFace(m_halfEdges.IndexOf(copy[0]), copy));
        }

        GL.Begin(GL.LINES);

        for(int i = 0; i < facesList.Count; i++)
        {
            for(int j = 1; j < facesList[i].Count; j++)
            {
                GL.Vertex3(m_positions[facesList[i][j - 1]].x, m_positions[facesList[i][j - 1]].y, m_positions[facesList[i][j - 1]].z);
                GL.Vertex3(m_positions[facesList[i][j]].x, m_positions[facesList[i][j]].y, m_positions[facesList[i][j]].z);
            }
        }

        GL.End();
        GL.PopMatrix();

    }

    public void Demo()
    {
        Init();

        m_positions.Add(new Vector3(0f, 0f, 0f));
        m_positions.Add(new Vector3(0f, 0f, 10f));
        m_positions.Add(new Vector3(10f, 0f, 5f));
        m_positions.Add(new Vector3(20f, 0f, 0f));

        // Face 1
        HalfEdge dart0 = new HalfEdge(1, 3, 4, 0);
        m_halfEdges.Add(dart0);

        HalfEdge dart1 = new HalfEdge(2, 0, 5, 1);
        m_halfEdges.Add(dart1);

        HalfEdge dart2 = new HalfEdge(3, 1, 6, 2);
        m_halfEdges.Add(dart2);

        HalfEdge dart3 = new HalfEdge(0, 2, 7, 3);
        m_halfEdges.Add(dart3);

        HalfEdge dart4 = new HalfEdge(7, 5, 0, 1);
        m_halfEdges.Add(dart4);

        HalfEdge dart5 = new HalfEdge(4, 6, 1, 2);
        m_halfEdges.Add(dart5);

        HalfEdge dart6 = new HalfEdge(5, 7, 2, 3);
        m_halfEdges.Add(dart6);

        HalfEdge dart7 = new HalfEdge(6, 4, 3, 0);
        m_halfEdges.Add(dart7);

        Debug.Log(m_halfEdges.Count);
        LinkTwoPoints(2, 0);
        Debug.Log(m_halfEdges.Count);

        //DrawMap();
    }

    private void Start()
    {
        Demo();
    }
}