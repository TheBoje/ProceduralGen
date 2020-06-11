using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class HalfEdgesMap
{
    private List<HalfEdge> m_halfEdges;
    private List<Vector3> m_positions;

    private void AddIsolatedStrand(int indexPosition)
    {
        HalfEdge strand = new HalfEdge(m_halfEdges.Count, indexPosition);
        m_halfEdges.Add(strand);
    }

    public void AddPoint(Vector3 position)
    {
        AddIsolatedStrand(m_positions.Count);
        m_positions.Add(position);
    }


    // Cas précis où on ne relie que deux brins -- Faire une fonction plus générale pour l'ajout de brins
    private void LinkTwoStrands(int indexStrand1, int indexStrand2)
    {
        m_halfEdges[indexStrand1].LinkStrandToNext(indexStrand2);
        m_halfEdges[indexStrand2].LinkStrandToPrevious(indexStrand1);

        // On créer le brin opposé du brin 1
        int oppositeStrandIndex1 = m_halfEdges.Count;
        HalfEdge oppositeStrand1 = new HalfEdge(oppositeStrandIndex1, m_halfEdges[indexStrand2].Position);
        m_halfEdges.Add(oppositeStrand1);

        // On créer le brin opposé du brin 2
        int oppositeStrandIndex2 = m_halfEdges.Count;
        HalfEdge oppositeStrand2 = new HalfEdge(oppositeStrandIndex2, m_halfEdges[indexStrand1].Position);
        m_halfEdges.Add(oppositeStrand2);

        // On insère les brins opposés dans les bons brins
        m_halfEdges[indexStrand1].Opposite = oppositeStrandIndex1;
        m_halfEdges[indexStrand2].Opposite = oppositeStrandIndex2;

        //                                              l'opposé du brin suivant du brin opposé             l'opposé du brin précédent du brin opposé                   le brin opposé
        m_halfEdges[oppositeStrandIndex1].SetHalfEdge(m_halfEdges[m_halfEdges[indexStrand1].Next].Opposite, m_halfEdges[m_halfEdges[indexStrand1].Previous].Opposite, indexStrand1);
        m_halfEdges[oppositeStrandIndex2].SetHalfEdge(m_halfEdges[m_halfEdges[indexStrand2].Next].Opposite, m_halfEdges[m_halfEdges[indexStrand2].Previous].Opposite, indexStrand2);
    }

    private void LinkThreeStrands(int indexMotherStrand, int nextStrand, int previousStrand)
    {
        m_halfEdges[indexMotherStrand].LinkStrandToNext(nextStrand);
        m_halfEdges[indexMotherStrand].LinkStrandToPrevious(previousStrand);

        m_halfEdges[nextStrand].LinkStrandToPrevious(indexMotherStrand);
        m_halfEdges[previousStrand].LinkStrandToNext(indexMotherStrand);

        int oppositeNextStrandIndex = m_halfEdges.Count;
        HalfEdge oppositeNextStrand = new HalfEdge(oppositeNextStrandIndex, 
            m_halfEdges[previousStrand].Opposite, 
            m_halfEdges[nextStrand].Opposite,
            nextStrand, 
            m_halfEdges[nextStrand].Position);

        m_halfEdges.Add(oppositeNextStrand);

        m_halfEdges[m_halfEdges[nextStrand].Opposite].Next = oppositeNextStrandIndex;
        m_halfEdges[m_halfEdges[previousStrand].Opposite].Previous = oppositeNextStrandIndex;
    }

    // lie trois brins les uns aux autres
    public void linkStrand(int indexStrandToLink, int nextStrand, int previousStrand)
    {
        if (nextStrand == previousStrand)
            LinkTwoStrands(indexStrandToLink, nextStrand);
        else
            LinkThreeStrands(indexStrandToLink, nextStrand, previousStrand);
    }

}