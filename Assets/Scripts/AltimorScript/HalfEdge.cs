﻿using System.Collections;
using System.Collections.Generic;

public class HalfEdge
{
    private int m_index;        // Index de la demi arête 
    private int m_next;         // Index de la demi arête suivante
    private int m_previous;     // Index de la demi arête précédente
    private int m_opposite;     // Index de la demi arête opposée
    private int m_position;     // Index du plongement correspondant à la position (Vector3)

    // Constructeurs
    public HalfEdge(int index, int indexPosition)
    {
        m_index = index;
        m_next = index;
        m_previous = index;
        m_opposite = index;
        m_position = indexPosition;
    }

    public HalfEdge(int index, int next, int previous, int opposite, int indexPosition)
    {
        m_index = index;
        m_next = next;
        m_previous = previous;
        m_opposite = opposite;
        m_position = indexPosition;
    }

    // Lie de demi-arêtes
    public void LinkStrandToNext(int strandIndex)
    {
        m_next = strandIndex;

        if (m_index == m_previous)
            m_previous = strandIndex;
    }

    public void LinkStrandToPrevious(int strandIndex)
    {
        m_previous = strandIndex;

        if (m_index == m_next)
            m_next = strandIndex;
    }

    // Ecrase les attributs
    public void SetHalfEdge(int next, int previous, int opposite)
    {
        m_next = next;
        m_previous = previous;
        m_opposite = opposite;
    }

    // PROPRIETES
    public int Next
    {
        get { return m_next; }
        set { m_next = value; }
    }

    public int Previous
    {
        get { return m_previous; }
        set { m_previous = value; }
    }

    public int Opposite
    {
        get { return m_opposite; }
        set { m_opposite = value; }
    }

    public int Position
    {
        get { return m_position; }
        set { m_position = value; }
    }
}
