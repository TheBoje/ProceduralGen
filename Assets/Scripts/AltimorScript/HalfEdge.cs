﻿using UnityEngine;

public class HalfEdge
{
    public enum TypeFace
    {
        BUILDING,
        PARK,
        ROAD,
        NONE
    }

    private HalfEdge m_next;         // Index de la demi arête suivante
    private HalfEdge m_previous;     // Index de la demi arête précédente
    private HalfEdge m_opposite;     // Index de la demi arête opposée
    private Vector3 m_position;     // Index du plongement correspondant à la position (Vector3)
    private Vector3 m_vect;
    private TypeFace m_type;

    // Constructeurs
    public HalfEdge(Vector3 position, TypeFace type = TypeFace.NONE)
    {
        m_next = this;
        m_previous = this;
        m_opposite = this;
        m_position = position;
        m_type = type;
    }

    public HalfEdge(HalfEdge next, HalfEdge previous, HalfEdge opposite, Vector3 position, TypeFace type = TypeFace.NONE)
    {
        m_next = next;
        m_previous = previous;
        m_opposite = opposite;
        m_position = position;
        m_type = type;
    }

    // Met à jour le vecteur directeur du brin
    public void RefreshVect()
    {
        m_vect = m_next.Position - m_position;
    }

    // Ecrase les attributs
    public void SetHalfEdge(HalfEdge next, HalfEdge previous, HalfEdge opposite, bool updateVect = true)
    {
        m_next = next;
        m_previous = previous;
        m_opposite = opposite;

        if (updateVect)
            this.RefreshVect();
    }

    // Retourne vrai si le brin est dégénéré
    public bool IsDegenerated()
    {
        return (m_next == this && this == m_previous);
    }

    public override string ToString()
    {
        return "De " + m_position + " vers " + m_next.Position;
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

    public TypeFace Type
    {
        get { return m_type; }
        set { m_type = value; }
    }

    public Vector3 Vect
    {
        get { return m_vect; }
    }
}