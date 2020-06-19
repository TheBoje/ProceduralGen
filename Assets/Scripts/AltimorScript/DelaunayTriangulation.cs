using System.Collections.Generic;
using UnityEngine;

// Algorithme provenant de https://yahiko.developpez.com/tutoriels/triangulation-delaunay-incrementale/

public class DelaunayTriangulation
{
    public struct Segment
    {
        public Vector3 A;
        public Vector3 B;
    }

    public struct Circle
    {
        public Vector3 center;
        public float radius;
    }

    public struct Triangle
    {
        public List<Vector3> vertices;
        public Circle circumscribed;
    }

    private float ComputeDeterminent(Vector3 u, Vector3 v)
    {
        return u.x * v.z - u.z * v.x;
    }

    // Vérifie si le point est dans le triangle (soit P un point et ABC un triangle, P est dans ABC si det(PA, PB), det(PB, PC) et det(PC, PA) sont du même signe (on ignore la composante en Y)
    private bool IsInTriangle(Triangle triangle, Vector3 point)
    {
        Vector3 PA = triangle.vertices[0] - point;
        Vector3 PB = triangle.vertices[1] - point;
        Vector3 PC = triangle.vertices[2] - point;

        float detPAPB = ComputeDeterminent(PA, PB);
        float detPBPC = ComputeDeterminent(PB, PC);
        float detPCPA = ComputeDeterminent(PC, PA);

        return (detPAPB >= 0 && detPBPC >= 0 && detPCPA >= 0) || (detPAPB < 0 && detPBPC < 0 && detPCPA < 0);
    }

    // Agrandit le triangle
    private List<Vector3> ResizeTriangle(Triangle triangle, float k)
    {
        List<Vector3> trianglePoints = triangle.vertices;

        trianglePoints[0] += new Vector3(-k, 0f, -k);
        trianglePoints[1] += new Vector3(0f, 0f, k);
        trianglePoints[2] += new Vector3(k, 0f, -k);

        return trianglePoints;
    }

    // Construit le premier super triangle
    private Triangle BuildFirtsSuperTriangle(List<Vector3> points)
    {
        float maxX = points[0].x;
        float minX = points[0].x;

        float maxZ = points[0].z;
        float minZ = points[0].z;

        foreach (Vector3 point in points)
        {
            maxX = (point.x > maxX) ? point.x : maxX;
            minX = (point.x < minX) ? point.x : minX;

            maxZ = (point.x > maxX) ? point.z : maxZ;
            minZ = (point.x < minZ) ? point.z : minZ;
        }

        Triangle triangle = new Triangle();

        triangle.vertices = new List<Vector3>();
        triangle.vertices.Add(new Vector3(minX, 0f, minZ));
        triangle.vertices.Add(new Vector3((maxX + minX) / 2, 0f, maxZ));
        triangle.vertices.Add(new Vector3(maxX, 0f, minZ));

        triangle.circumscribed = ComputeCircumscribed(triangle.vertices[0], triangle.vertices[1], triangle.vertices[2]);

        return triangle;
    }

    // Construit le super triangle
    private Triangle ComputeSuperTriangle(List<Vector3> points)
    {
        Triangle superTriangle = BuildFirtsSuperTriangle(points);

        bool allPointsInTriangle = false;
        List<Vector3> pointsIn = new List<Vector3>();

        while (!allPointsInTriangle)
        {
            foreach (Vector3 point in points)
            {
                if (superTriangle.vertices.Contains(point) || !IsInTriangle(superTriangle, point))
                {
                    superTriangle.vertices = ResizeTriangle(superTriangle, 2f);
                    superTriangle.circumscribed = ComputeCircumscribed(superTriangle.vertices[0], superTriangle.vertices[1], superTriangle.vertices[2]);
                }
                else
                {
                    if (!pointsIn.Contains(point))
                        pointsIn.Add(point);
                }
            }

            if (pointsIn.Count >= points.Count)
                allPointsInTriangle = true;
        }
        Debug.Log("1 : " + superTriangle.vertices[0] + " 2 : " + superTriangle.vertices[1] + "3 : " + superTriangle.vertices[2]);
        return superTriangle;
    }

    // Calcul le cercle circonscrit du triangle A, B, C
    private Circle ComputeCircumscribed(Vector3 a, Vector3 b, Vector3 c)
    {
        Circle circle;

        Vector3 v_AB = b - a;
        Vector3 v_BC = c - b;
        Vector3 v_CA = a - c;

        float tanA = Mathf.Tan(Vector3.AngleBetween(v_AB, v_CA * (-1f))); // Angle au sommet A
        float tanB = Mathf.Tan(Vector3.AngleBetween(v_AB * (-1f), v_BC)); // Angle au sommet B
        float tanC = Mathf.Tan(Vector3.AngleBetween(v_BC * (-1f), v_CA)); // Angle au sommet C

        circle.center = (1 / (2 * (tanA + tanB + tanC))) * ((tanB + tanC) * a + (tanA + tanC) * b + (tanA + tanB) * c); // Formule de la position du point d'intersection des médiatrices
        circle.radius = (circle.center - a).magnitude;

        return circle;
    }

    // Créer le triangle A, B, C ainsi que sont cercle circonscrit
    private Triangle BuildTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        Triangle tri;

        tri.vertices = new List<Vector3> { a, b, c };

        tri.circumscribed = ComputeCircumscribed(a, b, c);

        return tri;
    }

    // Retourne un tableau des segments du triangle
    private Segment[] GetTriangleSegments(Triangle tri)
    {
        Segment[] seg = new Segment[3];

        seg[0].A = tri.vertices[0];
        seg[0].B = tri.vertices[1];

        seg[1].A = tri.vertices[1];
        seg[1].B = tri.vertices[2];

        seg[2].A = tri.vertices[2];
        seg[2].B = tri.vertices[0];

        return seg;
    }

    // Supprime les segments en commun
    private List<Segment> DeleteSameSegment(List<Segment> S)
    {
        List<Segment> segList = new List<Segment>();

        for (int i = 0; i < S.Count; i++)
        {
            bool isTwice = false;
            for (int j = 0; j < S.Count; j++)
            {
                if (i != j)
                {
                    if ((S[i].A == S[j].A && S[i].B == S[j].B) || (S[i].B == S[j].A && S[i].A == S[j].B))
                    {
                        isTwice = true;
                        break;
                    }
                }
            }

            if (!isTwice)
                segList.Add(S[i]);
        }

        return segList;
    }

    // Enlève les segments entre les triangles (Obtient la frontière du polygone)
    private List<Segment> GetBorder(List<Triangle> triangles)
    {
        List<Segment> allSegments = new List<Segment>();

        foreach (Triangle tri in triangles)
        {
            foreach (Segment seg in GetTriangleSegments(tri))
            {
                allSegments.Add(seg);
            }
        }

        return DeleteSameSegment(allSegments);
    }

    // Retourne une liste de triangles dans laquelle le point p se trouve dans le cercle circonscrit de chaques éléments de la liste de triangles
    private List<Triangle> FindTrianglesContainers(Vector3 point, List<Triangle> triangles)
    {
        List<Triangle> triangleContainers = new List<Triangle>();

        foreach (Triangle tri in triangles)
        {
            if (Mathf.Pow(point.x - tri.circumscribed.center.x, 2) + Mathf.Pow(point.z - tri.circumscribed.center.z, 2) < Mathf.Pow(tri.circumscribed.radius, 2))
            {
                triangleContainers.Add(tri);
            }
        }

        return triangleContainers;
    }

    // Ajoute un point à l'ensemble des points existants
    private void AddPoint(Vector3 point, List<Triangle> triangles)
    {
        // Suppression des triangles dont le cercle circonscrit contient p
        List<Triangle> trianglesContainers = FindTrianglesContainers(point, triangles);

        foreach (Triangle tri in trianglesContainers)
        {
            triangles.Remove(tri);
        }

        // Création de nouveaux triangles vérifiant la condition de Delaunay
        List<Segment> segList = GetBorder(trianglesContainers);

        foreach (Segment seg in segList)
        {
            Triangle tri = BuildTriangle(seg.A, seg.B, point);
            triangles.Add(tri);
        }
        //return triangles;
    }

    public List<Triangle> Triangulate(List<Vector3> points)
    {
        Triangle initTriangle = ComputeSuperTriangle(points);

        List<Triangle> triangles = new List<Triangle>();
        triangles.Add(initTriangle);

        // On insère chaque points
        foreach (Vector3 point in points)
        {
            AddPoint(point, triangles);
        }

        List<Triangle> toRemoved = new List<Triangle>(triangles);

        // On retire les triangles qui ont un sommet en commum avec le super triangle
        foreach (Vector3 vertex in initTriangle.vertices)
        {
            foreach (Triangle tri in toRemoved)
            {
                if (tri.vertices.Contains(vertex))
                {
                    triangles.Remove(tri);
                }
            }
        }

        return triangles;
    }
}