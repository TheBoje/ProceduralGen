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

        return previous;
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

    // Dessine la carte
    public void DrawMap()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = m_halfEdges.Count;

        List<HalfEdge> copy = new List<HalfEdge>(m_halfEdges);
        List<List<int>> facesList = new List<List<int>>(); // Liste des faces

        while(copy.Count > 0)
        {
            facesList.Add(ComputePointsFace(m_halfEdges.IndexOf(copy[0]), copy));
        }

        int i = 0;
        int face = 0;

        while (i < lineRenderer.positionCount)
        {
            for (int vertex = 0; vertex < facesList[face].Count; vertex++)
            {
                lineRenderer.SetPosition(i, m_positions[facesList[face][vertex]]);
                i++;
            }
            face++;
        }
        

    }

    public void Demo()
    {
        Init();

        m_positions.Add(new Vector3(0f, 0f, 0f));
        m_positions.Add(new Vector3(0f, 0f, 10f));
        m_positions.Add(new Vector3(10f, 0f, 5f));
        m_positions.Add(new Vector3(20f, 0f, 0f));

        // Face 1
        HalfEdge dart1 = new HalfEdge(1, 2, 3, 0); // 0
        m_halfEdges.Add(dart1);
        HalfEdge dart2 = new HalfEdge(2, 0, 4, 1); // 1
        m_halfEdges.Add(dart2);
        HalfEdge dart3 = new HalfEdge(0, 1, 5, 2); // 2
        m_halfEdges.Add(dart3);

        HalfEdge opp1 = new HalfEdge(9, 4, 0, 1);  // 3
        m_halfEdges.Add(opp1);
        HalfEdge opp2 = new HalfEdge(3, 8, 1, 2);  // 4
        m_halfEdges.Add(opp2);
        HalfEdge opp3 = new HalfEdge(6, 7, 2, 0);  // 5
        m_halfEdges.Add(opp3);

        // Face 2
        HalfEdge dart12 = new HalfEdge(7, 5, 8, 2); // 6
        m_halfEdges.Add(dart12);
        HalfEdge dart22 = new HalfEdge(5, 6, 9, 3); // 7
        m_halfEdges.Add(dart22);

        HalfEdge opp12 = new HalfEdge(4, 9, 6, 3);  // 8
        m_halfEdges.Add(opp12);
        HalfEdge opp22 = new HalfEdge(8, 3, 7, 0);  // 9
        m_halfEdges.Add(opp22);


        DrawMap();
    }

    private void Start()
    {
        Demo();
    }
}