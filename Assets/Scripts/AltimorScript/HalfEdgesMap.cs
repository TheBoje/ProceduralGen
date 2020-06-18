﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

// Créer un triangle isolé

public class HalfEdgesMap : MonoBehaviour
{
    private List<HalfEdge> m_halfEdges;
    static Material lineMaterial; // utilisé pour afficher le debug
    public Material mat;



    public void Init()
    {
        m_halfEdges = new List<HalfEdge>();
    }

    public int HalfEdgeCount()
    {
        return m_halfEdges.Count;
    }

    private void AddIsolatedDart(Vector3 position)
    {
        m_halfEdges.Add(new HalfEdge(position)); 
    }

    // Retourne l'index d'un brin lié au point passé en paramètre
    private int IndexOfDartByPoint(Vector3 pointIndex)
    {
        for(int i = 0; i < m_halfEdges.Count; i++)
        {
            if (m_halfEdges[i].Position == pointIndex)
                return i;
        }

        return -1;
    }

    // Retourne le brin du point suivant
    public HalfEdge NextDartOnPoint(HalfEdge dart)
    {
        return dart.Opposite.Next;
    }

    // Calcul l'angle sur l'axe des Y : retourne l'angle en degré
    private float ComputeAngle(Vector3 from, Vector3 to)
    {
        return Vector3.Angle(new Vector3(from.x, 0f, from.z), new Vector3(to.x, 0f, to.z));
    }

    // Calcul les angles par rapport à l'horizontal pour trouver entre quel et quel brin nous devons mettre le nouveau brin : retourne le futur précédent (de pos1 vers pos2)
    private HalfEdge ComputePreviousDart(HalfEdge firstPrevious, Vector3 pos1, Vector3 pos2)
    {
        HalfEdge previous = firstPrevious; // Index du brin qui précèdera le prin allant de pos1 à pos2
        HalfEdge maxAngleIndex = previous; // Index du brin formant l'angle maximum par rapport à l'horizontal
        HalfEdge currentIndex = previous; // Index étant en train d'être étudié
        HalfEdge firstIndex = previous; // premier index étudié

        // On calcul l'angle du vecteur par rapport à l'horizontal pour pouvoir le classer par rapport aux autres brins issus du point pos1
        float angle = ComputeAngle(pos2 - pos1, Vector3.forward);
        float maxAngle = angle;
        float minAngle = angle;

        while(firstIndex != currentIndex)
        {
            float currentAngle = ComputeAngle(currentIndex.Opposite.Position - currentIndex.Position, Vector3.forward);
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

        return previous.Previous;
    }



    // Relie un brin à deux autres
    private void AddEdge(HalfEdge previousA, HalfEdge previousB, Vector3 A, Vector3 B)
    {

        // de A vers B
        HalfEdge AB = new HalfEdge(A);

        // De B vers A
        HalfEdge BA = new HalfEdge(B);

        AB.SetHalfEdge(previousB.Next, previousA, BA);
        BA.SetHalfEdge(previousA.Next, previousB, AB);

        m_halfEdges.Add(AB);
        m_halfEdges.Add(BA);

        previousA.Next.Previous = BA;
        previousB.Next.Previous = AB;

        previousA.Next = AB;
        previousB.Next = BA;
    }

    // Relie une arête dégénérée à un brin non dégénéré
    private void LinkDegeneratedDart(HalfEdge previousDart, HalfEdge dart)
    {
        HalfEdge opposite = new HalfEdge(dart, previousDart, dart, previousDart.Next.Position);
        m_halfEdges.Add(opposite);

        dart.Previous = opposite;
        dart.Opposite = opposite;
        dart.Next = previousDart.Next;

        previousDart.Next.Previous = dart;
        previousDart.Next = opposite;
    }


    // Relie de points pour former une arête
    private void LinkTwoDegeneratedDarts(HalfEdge dart1, HalfEdge dart2)
    {
        dart1.SetHalfEdge(dart2, dart2, dart2);
        dart2.SetHalfEdge(dart1, dart1, dart1);
    }

    // Relie deux points en prenant leurs position en paramètre
    public void LinkTwoPoints(Vector3 A, Vector3 B)
    {
        int firstPreviousA = IndexOfDartByPoint(A);
        int firstPreviousB = IndexOfDartByPoint(B);
        //Debug.Log("fpA, fpB : " + firstPreviousA + " " + firstPreviousB);
        
        if(firstPreviousA >= 0 && firstPreviousB < 0) // si le point B n'a pas de brins
        {
            int previousA = ComputePreviousDart(firstPreviousA, A, B);
            LinkPointToDart(previousA, B);
        }
        else if(firstPreviousA < 0 && firstPreviousB >= 0) // si le point A n'a pas de brins
        {
            int previousB = ComputePreviousDart(firstPreviousB, B, A);
            LinkPointToDart(previousB, A);
        }
        else if(firstPreviousA < 0 && firstPreviousB < 0) // si les deux points n'ont pas de brins
        {
            LinkTwoIsolatedPoints(A, B);
        }
        else
        {
            if (!m_halfEdges[firstPreviousA].IsDegenerated(firstPreviousA) && !m_halfEdges[firstPreviousB].IsDegenerated(firstPreviousB)) // si aucuns des points ne sont dégénérés
            {
                int previousA = ComputePreviousDart(firstPreviousA, A, B);
                int previousB = ComputePreviousDart(firstPreviousB, B, A);

                LinkDart(previousA, previousB, A, B);
            }
            else if (!m_halfEdges[firstPreviousA].IsDegenerated(firstPreviousA) && m_halfEdges[firstPreviousB].IsDegenerated(firstPreviousB)) // si le point B est dégénéré
            {
                int previousA = ComputePreviousDart(firstPreviousA, A, B);
                LinkDegeneratedDart(previousA, firstPreviousB, A);
            }
            else if (m_halfEdges[firstPreviousA].IsDegenerated(firstPreviousA) && !m_halfEdges[firstPreviousB].IsDegenerated(firstPreviousB)) // si le point A est dégénéré
            {
                int previousB = ComputePreviousDart(firstPreviousB, B, A);
                LinkDegeneratedDart(previousB, firstPreviousA, B);
            }
            // TODO - faire la condition des deux points dégénérés
        }
    }


    // Dessine une face à l'aide d'un Line renderer
    private List<Vector3> ComputePointsFace(HalfEdge firstEdge, List<HalfEdge> halfEdges)
    {
        HalfEdge currentIndex = firstEdge;
        List<Vector3> points = new List<Vector3>();

        points.Add(firstEdge.Position);

        do
        {
            currentIndex = currentIndex.Next;
            points.Add(currentIndex.Position);
            halfEdges.Remove(currentIndex);
        } while (currentIndex != firstEdge);

        return points;
    }


    public int[] GetIndexesInOrder(List<DelaunayTriangulation.Triangle> triangles, List<Vector3> points)
    {
        List<int> indexes = new List<int>();

        foreach (DelaunayTriangulation.Triangle tri in triangles)
        {
            foreach (Vector3 point in tri.vertices)
            {
                indexes.Add(points.IndexOf(point));
            }
        }

        return indexes.ToArray();
    }

    // Extrusion
    private void ExtrudeFace(List<Vector3> points, float height)
    {
        DelaunayTriangulation delaunayTriangulation = new DelaunayTriangulation();
        List<DelaunayTriangulation.Triangle> triangles = delaunayTriangulation.Triangulate(points);
        Debug.Log(triangles.Count);
        int[] index = GetIndexesInOrder(triangles, points);


        //Debug.Log(GetIndexesInOrder(triangles, points.ToList()));

        ProBuilderMesh quad = ProBuilderMesh.Create(points.ToArray(),
            new Face[] { new Face(index)
        });

        quad.Extrude(quad.faces, ExtrudeMethod.FaceNormal, 4f);



        quad.Refresh();

        quad.ToMesh();
        quad.GetComponent<MeshRenderer>().material = mat;
    }

    // Extrusion de toutes les faces
    public void ExtrudeFaces(List<List<int>> facesList)
    {
        foreach(List<int> face in facesList)
        {
            List<Vector3> points = new List<Vector3>();
            foreach(int i in face)
            {
                points.Add(new Vector3(m_positions[i].x, 0f, m_positions[i].z));
            }
            ExtrudeFace(points, Random.Range(2f, 5f));
        }
    }

    // 
    public void GlobalExtrusion()
    {
        List<HalfEdge> copy = new List<HalfEdge>(m_halfEdges);
        List<List<int>> facesList = new List<List<int>>(); // Liste des faces

        while (copy.Count > 0)
        {
            facesList.Add(ComputePointsFace(m_halfEdges.IndexOf(copy[0]), copy));
        }

        Debug.Log("NB Faces : " + facesList.Count);
        //ExtrudeFaces(facesList);
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
    private void OnRenderObject()
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

        Debug.Log("NB Faces : " + facesList.Count);

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

        m_positions.Add(new Vector3(0f, 3f, 0f));
        m_positions.Add(new Vector3(0f, 7f, 10f));
        m_positions.Add(new Vector3(10f, 0f, 5f));
        m_positions.Add(new Vector3(20f, 0f, 0f));
        m_positions.Add(new Vector3(20f, 4f, 10f));
        m_positions.Add(new Vector3(15f, 0f, 10f));
        m_positions.Add(new Vector3(5f, 0f, 10f));
        m_positions.Add(new Vector3(10f, 9f, 10f));
        

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

        // dégénéré
        /*HalfEdge dart8 = new HalfEdge(8, 8, 9, 5);
        m_halfEdges.Add(dart8);

        HalfEdge dart9 = new HalfEdge(9, 9, 8, 5);
        m_halfEdges.Add(dart9);*/

        
        Debug.Log("Avant LinkTwoPoints(2, 0); : " + m_halfEdges.Count);
        LinkTwoPoints(2, 0);
        Debug.Log("Apres LinkTwoPoints(2, 0); : " + m_halfEdges.Count);

        /*Debug.Log("Avant LinkTwoPoints(3, 4); : " + m_halfEdges.Count);
        LinkTwoPoints(3, 4);
        Debug.Log("Apres LinkTwoPoints(3, 4); : " + m_halfEdges.Count);

        /*Debug.Log("Avant LinkTwoPoints(5, 2); : " + m_halfEdges.Count);
        LinkTwoPoints(5, 2);
        Debug.Log("Apres LinkTwoPoints(5, 2); : " + m_halfEdges.Count);
        */

    }

    private void Start()
    {

        Demo();
    }
}