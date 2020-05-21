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

    // Vérifie si le voisin créer un triangle
    private bool IsTriangle(int nbEdge, Intersection neighbour)
    {
        if (nbEdge < 0)
            return false;
        else if (this == neighbour && nbEdge == 0)
            return true;
        else
        {
            foreach(Intersection n in neighbour.neighbours)
            {
                if (IsTriangle(nbEdge - 1, n))
                    return true;
            }
            return false;
        }
    }

    // Supprime les voisins formant des triangles
    public void DelTriangle()
    {
        foreach(Intersection n in m_neighbours)
        {
            if(IsTriangle(2, n))
            {
                m_neighbours.Remove(n);
                n.neighbours.Remove(this);
            }
        }
    }

    public Vector3 position
    {
        get { return m_position; }
        set { m_position = value; }
    }

    public List<Intersection> neighbours
    {
        get { return m_neighbours; }
        set { m_neighbours = value; }
    }
}
