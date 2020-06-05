using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.AI;

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

    // Créer le premier triangle pour lancer la triangulisation
    private Triangle ComputeFirstTriangle(List<Vector3> points)
    {


        if(points.Count < 3)
        {
            throw new Exception("Not enought points for triangulation");
        }
        else
        {
            return BuildTriangle(new Vector3(-2.5f, 0f, -0.5f), new Vector3(0.5f, 0f, 2.5f), new Vector3(5f, 0f, -0.5f));
        }
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
                if(i != j)
                {
                    if((S[i].A == S[j].A && S[i].B == S[j].B) || (S[i].B == S[j].A && S[i].A == S[j].B))
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

        foreach(Triangle tri in triangles)
        {
            foreach(Segment seg in GetTriangleSegments(tri))
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

        foreach(Triangle tri in triangles)
        {
            if(Mathf.Pow(point.x - tri.circumscribed.center.x, 2) + Mathf.Pow(point.z - tri.circumscribed.center.z, 2) < Mathf.Pow(tri.circumscribed.radius, 2))
            {
                triangleContainers.Add(tri);
            }
        }

        return triangleContainers;
    }

    // Ajoute un point à l'ensemble des points existants
    private void AddPoint(Vector3 point, List<Triangle> triangles)
    {
        //Debug.Log("Avant L'ajout de point : " + triangles.Count);
        // Suppression des triangles dont le cercle circonscrit contient p
        List<Triangle> trianglesContainers = FindTrianglesContainers(point, triangles);


        foreach(Triangle tri in trianglesContainers)
        {
            triangles.Remove(tri);
        }

        // Création de nouveaux triangles vérifiant la condition de Delaunay
        List<Segment> segList = GetBorder(trianglesContainers);

        foreach(Segment seg in segList)
        {
            Triangle tri = BuildTriangle(seg.A, seg.B, point);
            triangles.Add(tri);
        }
        //Debug.Log("Apres L'ajout de point : " + triangles.Count);
        //return triangles;
    }

    public List<Triangle> Triangulate(List<Vector3> points)
    {
        Triangle initTriangle = ComputeFirstTriangle(points);

        List<Triangle> triangles = new List<Triangle>();
        triangles.Add(initTriangle);

        foreach(Vector3 point in points)
        {
            AddPoint(point, triangles);
        }

        List<Triangle> toRemoved = new List<Triangle>();
        
        foreach(Vector3 vertex in initTriangle.vertices)
        {
            foreach(Triangle tri in triangles)
            {
                if(tri.vertices.Contains(vertex))
                {
                    toRemoved.Add(tri);
                }
            }
        }

        foreach(Triangle tri in toRemoved)
        {
            triangles.Remove(tri);
        }
        
        return triangles;
    }

    
}

/// <summary>
/// http://wiki.unity3d.com/index.php?title=Triangulator&_ga=2.172830115.1260159605.1591280256-872917778.1591280256
/// </summary>

public class Triangulator
{
    private List<Vector3> m_points = new List<Vector3>();

    public Triangulator(Vector3[] points)
    {
        m_points = new List<Vector3>(points);
    }

    public int[] Triangulate()
    {
        List<int> indices = new List<int>();

        int n = m_points.Count;
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (Area() > 0)
        {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (Snip(u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        //indices.Reverse();
        return indices.ToArray();
    }

    private float Area()
    {
        int n = m_points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector3 pval = m_points[p];
            Vector3 qval = m_points[q];
            A += pval.x * qval.z - qval.x * pval.z;
        }
        return (A * 0.5f);
    }

    private bool Snip(int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector3 A = m_points[V[u]];
        Vector3 B = m_points[V[v]];
        Vector3 C = m_points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.z - A.z)) - ((B.z - A.z) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector3 P = m_points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
    {
        float ax, az, bx, bz, cx, cz, apx, apz, bpx, bpz, cpx, cpz;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; az = C.z - B.z;
        bx = A.x - C.x; bz = A.z - C.z;
        cx = B.x - A.x; cz = B.z - A.z;
        apx = P.x - A.x; apz = P.z - A.z;
        bpx = P.x - B.x; bpz = P.z - B.z;
        cpx = P.x - C.x; cpz = P.z - C.z;

        aCROSSbp = ax * bpz - az * bpx;
        cCROSSap = cx * apz - cz * apx;
        bCROSScp = bx * cpz - bz * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}