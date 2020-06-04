using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class WingedEdgeMap : MonoBehaviour
{
    public Material mat;

    private void Start()
    {

        Vector3[] points = new Vector3[] {
                            new Vector3(0f, 0f, 0f),
                            new Vector3(1f, 0f, 0f),
                            new Vector3(0f, 0f, 1f),
                            new Vector3(1f, 0f, 1f),
                            new Vector3(1f, 0f, 2f)
                        };

        Triangulator triangulator = new Triangulator(points);

        ProBuilderMesh quad = ProBuilderMesh.Create(points,
            new Face[] { new Face(triangulator.Triangulate())
        });

        

        //quad.Extrude(quad.faces, ExtrudeMethod.FaceNormal, -4f);

        quad.Refresh();

        quad.ToMesh();
        quad.GetComponent<MeshRenderer>().material = mat;


        List<WingedEdge> wList = WingedEdge.GetWingedEdges(quad);

        Debug.Log("Faces : " + quad.faceCount);
        Debug.Log("Edges : " + quad.edgeCount);

        Debug.Log("finish");
    }



}
