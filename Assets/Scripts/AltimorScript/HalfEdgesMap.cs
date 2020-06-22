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


    /// <summary>
    /// Initialise les paramètres de la carte
    /// </summary>
    public void Init()
    {
        m_halfEdges = new List<HalfEdge>();
    }

    /// <returns>Retourne le nombre de brins</returns>
    public int HalfEdgeCount()
    {
        return m_halfEdges.Count;
    }

    /// <summary>
    /// Ajoute un brin dégénéré à la carte
    /// </summary>
    /// <param name="position">Plongement correspondant à la position du brin</param>
    public void AddIsolatedDart(Vector3 position)
    {
        m_halfEdges.Add(new HalfEdge(position)); 
    }

    /// <summary>
    /// Boucle jusqu'à trouver le brin correspondant à la position
    /// </summary>
    /// <param name="pointIndex">Position pour comparer avec les plongements des brins</param>
    /// <returns>Retourne le premier brin de la liste ayant pour plongement la position passée en paramètre</returns>
    private HalfEdge FirstDartByPoint(Vector3 pointIndex)
    {
        for(int i = 0; i < m_halfEdges.Count; i++)
        {
            if (m_halfEdges[i].Position == pointIndex)
                return m_halfEdges[i];
        }

        return null;
    }

    // Retourne le brin du point suivant
    public HalfEdge NextDartOnPoint(HalfEdge dart)
    {
        return dart.Opposite.Next;
    }

    // Calcul l'angle sur l'axe des Y : retourne l'angle en degré
    private float ComputeAngle(Vector3 from, Vector3 to)
    {
        float angle = Vector3.SignedAngle(new Vector3(from.x, 0f, from.z), new Vector3(to.x, 0f, to.z), Vector3.up);
        return (360f + angle) % 360;
    }

    // Calcul les angles par rapport à l'horizontal pour trouver entre quel et quel brin nous devons mettre le nouveau brin : retourne le futur précédent (de pos1 vers pos2)
    private HalfEdge ComputePreviousDart(HalfEdge firstPrevious, Vector3 pos1, Vector3 pos2)
    {
        HalfEdge previous = firstPrevious; // Index du brin qui précèdera le prin allant de pos1 à pos2
        HalfEdge currentIndex = previous; // Index étant en train d'être étudié
        HalfEdge firstIndex = previous; // premier index étudié

        // On calcul l'angle du vecteur par rapport à l'horizontal pour pouvoir le classer par rapport aux autres brins issus du point pos1
        float minAngle = 360;
        Debug.Log("haallo");
        do
        {
            float currentAngle = ComputeAngle(currentIndex.Opposite.Position - currentIndex.Position, pos2 - pos1);

            if(minAngle > currentAngle)
            {
                minAngle = currentAngle;
                previous = currentIndex;
            }

            currentIndex = NextDartOnPoint(currentIndex);
        } while (firstIndex != currentIndex);

        //Debug.Log("Tronçon prefinal : " + previous.Position + " vers " + previous.Opposite.Position);
        //Debug.Log("Tronçon Final : " + previous.Opposite.ToString());
        return previous.Opposite;
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
        HalfEdge firstPreviousA = FirstDartByPoint(A);
        HalfEdge firstPreviousB = FirstDartByPoint(B);
        //Debug.Log("fpA, fpB : " + firstPreviousA + " " + firstPreviousB);
        
       
        if (!firstPreviousA.IsDegenerated() && !firstPreviousB.IsDegenerated()) // si aucuns des points ne sont dégénérés
        {
            HalfEdge previousA = ComputePreviousDart(firstPreviousA, A, B);
            HalfEdge previousB = ComputePreviousDart(firstPreviousB, B, A);

            AddEdge(previousA, previousB, A, B);
        }
        else if (!firstPreviousA.IsDegenerated() && firstPreviousB.IsDegenerated()) // si le point B est dégénéré
        {
            HalfEdge previousA = ComputePreviousDart(firstPreviousA, A, B);
            LinkDegeneratedDart(previousA, firstPreviousB);
        }
        else if (firstPreviousA.IsDegenerated() && !firstPreviousB.IsDegenerated()) // si le point A est dégénéré
        {
            HalfEdge previousB = ComputePreviousDart(firstPreviousB, B, A);
            LinkDegeneratedDart(previousB, firstPreviousA);
        }
        else
        {
            LinkTwoDegeneratedDarts(firstPreviousA, firstPreviousB);
        }
            // TODO - faire la condition des deux points dégénérés
    }


    // retourne la liste de points d'une face
    private List<Vector3> ComputePointsFace(HalfEdge firstEdge, List<HalfEdge> halfEdges)
    {
        HalfEdge currentIndex = firstEdge;
        List<Vector3> points = new List<Vector3>();

        points.Add(firstEdge.Position);
        string face = "Face : ";
        do
        {
            currentIndex = currentIndex.Next;
            points.Add(currentIndex.Position);
            halfEdges.Remove(currentIndex);
            face += currentIndex.ToString() + " ";
        } while (currentIndex != firstEdge);
        Debug.Log(face);
        return points;
    }

    // Calcul le point d'une face où se trouve le plus grand angle : retourne son brin
    private HalfEdge ComputeMaxAngle(HalfEdge start)
    {
        float maxAngle = 0f;
        HalfEdge currentDart = start;
        HalfEdge maxAngleDart = start;

        do
        {
            float currentAngle = ComputeAngle(currentDart.Next.Position - currentDart.Position, currentDart.Previous.Position - currentDart.Position);
            if (maxAngle < currentAngle)
            {
                maxAngle = currentAngle;
                maxAngleDart = currentDart;
            }
        } while (start != currentDart);

        return maxAngleDart;
    }

    // Triangule en utilisant la méthode des oreilles : retourne une liste d'entiers correspondant à la triangulation
    private int[] Triangulate(List<HalfEdge> face)
    {
        List<int> triangulation = new List<int>();

        HalfEdge hub = ComputeMaxAngle(face[0]);
        int indexHub = face.IndexOf(hub);

        foreach(HalfEdge dart in face)
        {
            if(dart.Next != hub && dart != hub)
            {
                triangulation.Add(indexHub);
                triangulation.Add(face.IndexOf(dart));
                triangulation.Add(face.IndexOf(dart.Next));
            }
        }

        return triangulation.ToArray();
    }

    // Retourne le tableau de position des points la face
    private Vector3[] PointsPositionInFaces(List<HalfEdge> face, bool isHorizontal)
    {
        Vector3[] points = new Vector3[face.Count];

        for(int i = 0; i < face.Count; i++)
            if(isHorizontal)
                points[i] = new Vector3(face[i].Position.x, 0f, face[i].Position.z);

        return points;
    }

    // Extrude en utilisant le probuilder mesh
    public void Extrude(List<HalfEdge> face)
    {
        Vector3[] facePoints = PointsPositionInFaces(face, true);
        int[] triangles = Triangulate(face);

        WingedEdgeMap.PrintArray(triangles);

        ProBuilderMesh poly = ProBuilderMesh.Create(facePoints, new Face[] { new Face(triangles) } );

        poly.Extrude(poly.faces, ExtrudeMethod.FaceNormal, 4f);
        poly.Refresh();
        poly.GetComponent<MeshRenderer>().material = mat;
    }

    // Retourne la list de brins d'une face
    private List<HalfEdge> ComputeFace(HalfEdge firstDart)
    {
        List<HalfEdge> face = new List<HalfEdge>();
        HalfEdge current = firstDart;

        do
        {
            face.Add(current);
            current = current.Next;
        } while (current != firstDart);

        return face;
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
        List<List<Vector3>> facesList = new List<List<Vector3>>(); // Liste des faces

        while(copy.Count > 0)
        {
            facesList.Add(ComputePointsFace(copy[0], copy));
        }

        Debug.Log("NB Faces : " + facesList.Count);

        GL.Begin(GL.LINES);

        for(int i = 0; i < facesList.Count; i++)
        {
            for(int j = 1; j < facesList[i].Count; j++)
            {
                GL.Vertex3(facesList[i][j - 1].x, facesList[i][j - 1].y, facesList[i][j - 1].z);
                GL.Vertex3(facesList[i][j].x, facesList[i][j].y, facesList[i][j].z);
            }
        }

        GL.End();
        GL.PopMatrix();

    }

    public void Demo()
    {
        Init();

        // Face 1
        HalfEdge dart0 = new HalfEdge(new Vector3(0f, 0f, 0f));
        m_halfEdges.Add(dart0);

        HalfEdge dart1 = new HalfEdge(new Vector3(0f, 0f, 10f));
        m_halfEdges.Add(dart1);

        HalfEdge dart2 = new HalfEdge(new Vector3(10f, 0f, 0f));
        m_halfEdges.Add(dart2);

        HalfEdge dart3 = new HalfEdge(new Vector3(10f, 0f, 10f));
        m_halfEdges.Add(dart3);



        Debug.Log("1");
        LinkTwoPoints(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 10f));
        Debug.Log("2");
        LinkTwoPoints(new Vector3(0f, 0f, 10f), new Vector3(10f, 0f, 10f));
        Debug.Log("3");
        LinkTwoPoints(new Vector3(10f, 0f, 10f), new Vector3(10f, 0f, 0f));
        Debug.Log("4");
        LinkTwoPoints(new Vector3(10f, 0f, 0f), new Vector3(0f, 0f, 0f));
        Debug.Log("5");
        LinkTwoPoints(new Vector3(10f, 0f, 10f), new Vector3(0f, 0f, 0f));

        Extrude(ComputeFace(m_halfEdges[8]));

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