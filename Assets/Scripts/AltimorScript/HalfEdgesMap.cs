using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
    private const float MAGNITUDE_INTERSECTION = 4f;


    static Material lineMaterial; // utilisé pour afficher le debug
    public Material mat;
    public bool drawDarts = false;


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
        m_halfEdges.Add(new HalfEdge(m_halfEdges.Count, position)); 
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

    /// <summary>
    /// Le suivant de l'opposé
    /// </summary>
    /// <param name="dart">Brin actuel</param>
    /// <returns>Retourne le brin du point suivant</returns>
    public HalfEdge NextDartOnPoint(HalfEdge dart)
    {
        return dart.Opposite.Next;
    }

    /// <summary>
    /// Calcul l'angle orienté dans le sens trigonométrique sur l'axe des Y
    /// </summary>
    /// <param name="from">Vecteur de départ</param>
    /// <param name="to">Vecteur d'arrivée</param>
    /// <returns>Retourne l'angle entre les deux vecteur</returns>
    private float ComputeAngle(Vector3 from, Vector3 to)
    {
        float angle = Vector3.SignedAngle(new Vector3(from.x, 0f, from.z), new Vector3(to.x, 0f, to.z), Vector3.up);
        return (360f + angle) % 360;
    }

    /// <summary>
    /// Calcul le brin opposé du précédent en fonction de l'angle que les deux points passé en paramètre forment entre eux
    /// </summary>
    /// <param name="firstPrevious">Premier brin analysé du point</param>
    /// <param name="pos1">Position du point de départ</param>
    /// <param name="pos2">Position du point d'arrivée</param>
    /// <returns>Retourne l'opposé du brin précédent trouvé</returns>
    private HalfEdge ComputePreviousDart(HalfEdge firstPrevious, Vector3 pos1, Vector3 pos2)
    {
        HalfEdge previous = firstPrevious; // Index du brin qui précèdera le prin allant de pos1 à pos2
        HalfEdge currentIndex = previous; // Index étant en train d'être étudié
        HalfEdge firstIndex = previous; // premier index étudié

        // On calcul l'angle du vecteur par rapport à l'horizontal pour pouvoir le classer par rapport aux autres brins issus du point pos1
        float minAngle = 360;

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
        HalfEdge AB = new HalfEdge(m_halfEdges.Count, A);

        // De B vers A
        HalfEdge BA = new HalfEdge(m_halfEdges.Count + 1, B);

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

    // Insère une face dans une arête
    public void CreateFaceFromEdge(HalfEdge dart)
    {
        HalfEdge newDart = new HalfEdge(m_halfEdges.Count, dart.Opposite.Position, HalfEdge.TypeFace.ROAD);
        HalfEdge newDartOpposite = new HalfEdge(m_halfEdges.Count + 1, dart.Position, HalfEdge.TypeFace.ROAD);

        newDart.SetHalfEdge(newDartOpposite, newDartOpposite, dart);
        newDartOpposite.SetHalfEdge(newDart, newDart, dart.Opposite);

        m_halfEdges.Add(newDart);
        m_halfEdges.Add(newDartOpposite);

        dart.Opposite.Opposite = newDartOpposite;
        dart.Opposite = newDart;
    }

    // Ajoute des faces pour les routes
    public void FillEsdgesWithRoads()
    {
        List<HalfEdge> copy = new List<HalfEdge>(m_halfEdges);

        while (copy.Count > 0)
        {
            copy.Remove(copy[0].Opposite);
            CreateFaceFromEdge(m_halfEdges[m_halfEdges.IndexOf(copy[0])]);
            copy.Remove(copy[0]);
        
        }
    }

    // Calcul la nouvelle position du point de l'intersection
    private Vector3 ComputeNewIntersection(Vector3 from, Vector3 to, Vector3 intersection, float magnitude)
    {
        float angle = ComputeAngle(from, to);
        angle /= 2f;

        return new Vector3(Mathf.Cos(angle) * magnitude, intersection.y, Mathf.Sin(angle) * magnitude);
    }

    // Calcul les nouveaux points d'une intersection
    private void ComputePointsOnIntersection(Vector3 intersection)
    {
        HalfEdge firsDart = FirstDartByPoint(intersection);
        HalfEdge currentDart = firsDart;

        do
        {
            if(currentDart.Next == currentDart.Previous)
            {
                
            }
            else
            {

            }

            currentDart = NextDartOnPoint(currentDart);
        } while (currentDart != firsDart);
    }

    // Calcul des nouveaux points de toutes les intersections
    public void ComputeAllIntersectionPoints(Vector3?[,] grid, int row, int col)
    {
        for(int i = 0; i < row; i++)
        {
            for(int j = 0; j < col; j++)
            {
                if(grid[i, j] != null)
                {
                    ComputePointsOnIntersection((Vector3)grid[i, j]);
                }
            }
        }
    }

    public void ComputeAllIntersectionPoints(List<Vector3> points)
    {
        
        foreach(Vector3 point in points)
            ComputePointsOnIntersection(point);

    }





    /** 
     * 
     * Fonctions s'occupant de l'extrusion
     *
     **/

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

    // Retourne la hauteur la plus petite
    private float MinHeight(List<HalfEdge> darts)
    {
        float min = darts[0].Position.y;

        foreach(HalfEdge dart in darts)
        {
            min = (dart.Position.y < min) ? dart.Position.y : min;
        }

        return min;
    }


    // Retourne le tableau de position des points la face
    private Vector3[] PointsPositionInFaces(List<HalfEdge> face, bool isHorizontal = true)
    {
        Vector3[] points = new Vector3[face.Count];
        
        
        for(int i = 0; i < face.Count; i++)
            if(isHorizontal)
            {
                float minHeight = MinHeight(face);
                points[i] = new Vector3(face[i].Position.x, minHeight, face[i].Position.z);
            }
                
            else
                points[i] = face[i].Position;

        return points;
    }

    // Extrude en utilisant le probuilder mesh
    public void Extrude(List<HalfEdge> face, float height)
    {
        Vector3[] facePoints = PointsPositionInFaces(face);
        int[] triangles = Triangulate(face);

        WingedEdgeMap.PrintArray(triangles);

        if (Vector3.Cross(facePoints[triangles[1]] - facePoints[triangles[0]], facePoints[triangles[2]] - facePoints[triangles[1]]).y <= 0f)
            Array.Reverse(triangles);

        WingedEdgeMap.PrintArray(triangles);
        ProBuilderMesh poly = ProBuilderMesh.Create(facePoints, new Face[] { new Face(triangles) });



        poly.Extrude(poly.faces, ExtrudeMethod.FaceNormal, height);
        poly.ToMesh();
        MeshRenderer mr = poly.GetComponent<MeshRenderer>();

        mr.material = mat;
        poly.Refresh();
    }

    // Retourne la list de brins d'une face
    private List<HalfEdge> ComputeFace(HalfEdge firstDart, List<HalfEdge> dartsList)
    {
        List<HalfEdge> face = new List<HalfEdge>();
        HalfEdge current = firstDart;

        HalfEdge.TypeFace type = (HalfEdge.TypeFace)UnityEngine.Random.Range((int)HalfEdge.TypeFace.BUILDING, (int)HalfEdge.TypeFace.PARK + 1);

        do
        {
            current.Type = type;
            face.Add(current);
            dartsList.Remove(current);
            current = current.Next;
        } while (current != firstDart);

        return face;
    }

    // Retourne la liste des faces
    private List<List<HalfEdge>> ComputeFaces()
    {
        List<List<HalfEdge>> faces = new List<List<HalfEdge>>();
        List<HalfEdge> copy = new List<HalfEdge>(m_halfEdges);

        while(copy.Count > 0)
        {
            faces.Add(ComputeFace(copy[0], copy));
        }

        int indexMaxLength = 0;
        int maxLen = 0;

        for(int i = 0; i < faces.Count; i++)
        {
            if(maxLen < faces[i].Count)
            {
                indexMaxLength = i;
                maxLen = faces[i].Count;
            }
        }

        faces.RemoveAt(indexMaxLength);

        return faces;
    }

    // Extrude toutes les faces d'une hauteur compris entre les deux param
    public void ExtrudeAllFaces(float minHeight, float maxHeight)
    {
        List<List<HalfEdge>> faces = ComputeFaces();

        foreach(List<HalfEdge> face in faces)
        {
            switch(face[0].Type)
            {
                case HalfEdge.TypeFace.BUILDING :
                    Extrude(face, UnityEngine.Random.Range(minHeight, maxHeight));
                    break;
            }
        }
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
        if(drawDarts)
        {
            CreateLineMaterial();
            lineMaterial.SetPass(0);
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);

            List<HalfEdge> copy = new List<HalfEdge>(m_halfEdges);
            List<List<Vector3>> facesList = new List<List<Vector3>>(); // Liste des faces

            while (copy.Count > 0)
            {
                facesList.Add(ComputePointsFace(copy[0], copy));
            }

            Debug.Log("NB Faces : " + facesList.Count);

            GL.Begin(GL.LINES);

            for (int i = 0; i < facesList.Count; i++)
            {
                for (int j = 1; j < facesList[i].Count; j++)
                {
                    GL.Vertex3(facesList[i][j - 1].x, facesList[i][j - 1].y, facesList[i][j - 1].z);
                    GL.Vertex3(facesList[i][j].x, facesList[i][j].y, facesList[i][j].z);
                }
            }

            GL.End();
            GL.PopMatrix();
        }
    }

    public void Demo()
    {
        Debug.Log("debut dmo");

        Init();

        List<Vector3> points = new List<Vector3>();
        points.Add(new Vector3(0f, 0f, 0f));
        points.Add(new Vector3(0f, 0f, 10f));
        points.Add(new Vector3(10f, 0f, 0f));
        //points.Add(new Vector3(10f, 0f, 10f));


        // Face 1
        HalfEdge dart0 = new HalfEdge(m_halfEdges.Count, points[0]);
        m_halfEdges.Add(dart0);

        HalfEdge dart1 = new HalfEdge(m_halfEdges.Count, points[1]);
        m_halfEdges.Add(dart1);

        HalfEdge dart2 = new HalfEdge(m_halfEdges.Count, points[2]);
        m_halfEdges.Add(dart2);

        //HalfEdge dart3 = new HalfEdge(m_halfEdges.Count, points[3]);
        //m_halfEdges.Add(dart3);



        Debug.Log("1");
        LinkTwoPoints(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 10f));
        Debug.Log("2");
        LinkTwoPoints(new Vector3(0f, 0f, 10f), new Vector3(10f, 0f, 0f));
        Debug.Log("3");
        //LinkTwoPoints(new Vector3(10f, 0f, 10f), new Vector3(10f, 0f, 0f));
        Debug.Log("4");
        LinkTwoPoints(new Vector3(10f, 0f, 0f), new Vector3(0f, 0f, 0f));
        //Debug.Log("5");
        //LinkTwoPoints(new Vector3(10f, 0f, 10f), new Vector3(0f, 0f, 0f));

        FillEsdgesWithRoads();
        ComputeAllIntersectionPoints(points);
        Debug.Log("fin demo");

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