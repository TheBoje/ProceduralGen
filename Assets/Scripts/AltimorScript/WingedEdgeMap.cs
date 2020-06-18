using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class WingedEdgeMap
{
    public Material mat;

    public int[] GetIndexesInOrder(List<DelaunayTriangulation.Triangle> triangles, List<Vector3> points)
    {
        List<int> indexes = new List<int>();

        foreach(DelaunayTriangulation.Triangle tri in triangles)
        {
            foreach(Vector3 point in tri.vertices)
            {
                indexes.Add(points.IndexOf(point));
            }
        }

        return indexes.ToArray();
    }

    private void PrintArray(int[] arr)
    {
        string str = "";
        foreach(int i in arr)
        {
            str += " " + i;
        }
        Debug.Log(str);
    }


    /*private void Start()
    {
        Vector3[] points = new Vector3[] {
                            new Vector3(0f, 0f, 0f), // 0
                            new Vector3(1f, 0f, 0f),
                            new Vector3(0f, 0f, 1f),
                            new Vector3(1f, 0f, 1f),
                            new Vector3(1f, 0f, 2f)
                        };

        DelaunayTriangulation delaunayTriangulation = new DelaunayTriangulation();
        List<DelaunayTriangulation.Triangle> triangles = delaunayTriangulation.Triangulate(points.ToList());
        Debug.Log(triangles.Count);
        int[] index = GetIndexesInOrder(triangles, points.ToList());
        
        PrintArray(index);

        //Debug.Log(GetIndexesInOrder(triangles, points.ToList()));

        ProBuilderMesh quad = ProBuilderMesh.Create(points,
            new Face[] { new Face(index)
        });
        
        
        quad.Extrude(quad.faces, ExtrudeMethod.FaceNormal, 4f);


        
        quad.Refresh();

        quad.ToMesh();
        quad.GetComponent<MeshRenderer>().material = mat;


        List<WingedEdge> wList = WingedEdge.GetWingedEdges(quad);

        Debug.Log("Faces : " + quad.faceCount);
        Debug.Log("Edges : " + quad.edgeCount);
        
        Debug.Log("finish");
    }*/



}
