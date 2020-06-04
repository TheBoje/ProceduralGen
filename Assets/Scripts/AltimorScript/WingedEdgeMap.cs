using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class WingedEdgeMap : MonoBehaviour
{
    public Material mat;

    private void Start()
    {

        ProBuilderMesh quad = ProBuilderMesh.Create(
            new Vector3[] {
                new Vector3(0f, 0f, 0f),
                new Vector3(1f, 0f, 0f),
                new Vector3(0f, 1f, 0f),
                new Vector3(1f, 1f, 0f),
                new Vector3(1f, 2f, 0f)
            },
            new Face[] { new Face(new int[] { 0, 1, 2, 1, 3, 2, 2, 3, 4 } )
        });

        quad.Extrude(quad.faces, ExtrudeMethod.FaceNormal, 4f);
        quad.Refresh();

        quad.ToMesh();
        quad.GetComponent<MeshRenderer>().material = mat;


        List<WingedEdge> wList = WingedEdge.GetWingedEdges(quad);

        Debug.Log("Faces : " + quad.faceCount);
        Debug.Log("Edges : " + quad.edgeCount);
    }



}
