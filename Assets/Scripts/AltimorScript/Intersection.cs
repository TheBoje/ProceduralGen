using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    private Vector3 m_position;
    private List<Intersection> m_neighbours;

    public Intersection(Vector3 position, List<Intersection> neighbours)
    {
        m_position = position;
        m_neighbours = neighbours;
    }

    public void AddNeighbour(Intersection neighbour)
    {
        m_neighbours.Add(neighbour);
    }

    public Vector3 position
    {
        get { return m_position; }
    }

    public List<Intersection> neighbours
    {
        get { return m_neighbours; }
        set { m_neighbours = value; }
    }
}
