using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

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
        public Vector3[] vertices;
        public Circle circumscribed;
    }

    // Calcul le cercle circonscrit du triangle A, B, C
    private Circle ComputeCircumscribed(Vector3 a, Vector3 b, Vector3 c)
    {
        Circle circle;

        circle.center = Vector3.zero;
        circle.radius = (circle.center - a).magnitude;

        return circle;
    }

    // Créer le triangle A, B, C ainsi que sont cercle circonscrit
    private Triangle BuildTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        Triangle tri;

        tri.vertices = new Vector3[3];

        tri.vertices[0] = a;
        tri.vertices[1] = b;
        tri.vertices[2] = c;

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
            return BuildTriangle(points[0], points[1], points[2]);
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
                    if(S[i].A == S[j].A && S[i].B == S[j].B || S[i].B == S[j].A && S[i].A == S[j].B)
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
}