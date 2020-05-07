﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    private Vector3 m_position;
    private List<Vector3> m_neighbours;

    public Intersection(Vector3 position, List<Vector3> neighbours)
    {
        m_position = position;
        m_neighbours = neighbours;
    }

    public Vector3 position
    {
        get { return m_position; }
    }

    public List<Vector3> neighbours
    {
        get { return m_neighbours; }
        set { m_neighbours = value; }
    }
}