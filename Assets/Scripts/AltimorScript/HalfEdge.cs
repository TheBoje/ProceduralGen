using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfEdge
{
    private HalfEdge m_next;         // Index de la demi arête suivante
    private HalfEdge m_previous;     // Index de la demi arête précédente
    private HalfEdge m_opposite;     // Index de la demi arête opposée
    private Vector3 m_position;     // Index du plongement correspondant à la position (Vector3)

    // Constructeurs
    public HalfEdge(Vector3 position)
    {
        m_next = this;
        m_previous = this;
        m_opposite = this;
        m_position = position;
    }

    public HalfEdge(HalfEdge next, HalfEdge previous, HalfEdge opposite, Vector3 position)
    {
        m_next = next;
        m_previous = previous;
        m_opposite = opposite;
        m_position = position;
    }


    // Ecrase les attributs
    public void SetHalfEdge(HalfEdge next, HalfEdge previous, HalfEdge opposite)
    {
        m_next = next;
        m_previous = previous;
        m_opposite = opposite;
    }

    // Retourne vrai si le brin est dégénéré
    public bool IsDegenerated(HalfEdge index)
    {
        return (m_next == index && index == m_previous);
    }

    public HalfEdge Next
    {
        get { return m_next; }
        set { m_next = value; }
    }

    public HalfEdge Previous
    {
        get { return m_previous; }
        set { m_previous = value; }
    }

    public HalfEdge Opposite
    {
        get { return m_opposite; }
        set { m_opposite = value; }
    }

    public Vector3 Position
    {
        get { return m_position; }
        set { m_position = value; }
    }

}
