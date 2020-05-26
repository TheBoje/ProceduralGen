using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{

    //private List<Intersection> m_neighbours;
    //private List<bool> m_joined;

    // type structuré contenant les voisins
    public struct Neighbour
    {

        public Neighbour(Vector2Int coord, bool join)
        {
            joined = join;
            coords = coord;

        }

        public void SetJoined(bool val)
        {
            joined = val;
        }

        public bool joined { get; set; }
        public Vector2Int coords { get; set; }

    }

    private List<Neighbour> m_neighbours;
    private Vector3 m_position;
    private Vector2Int m_coordonates;

    // CONSTRUCTEUR 
    public Intersection(Vector3 position, Vector2Int coords)
    {
        m_position = position;
        m_coordonates = coords;

        //m_neighbours = new List<Neighbour>();
    }

    public void InitNeighbours()
    {
        m_neighbours = new List<Neighbour>();
    }

    public void AddNeighbour(int i, int j)
    {
        m_neighbours.Add(new Neighbour(new Vector2Int(i, j), false));
    }

    // Regarde si l'intersection inter est déjà voisine
    public bool ContainsIntersection(Intersection intersection)
    {
        foreach(Neighbour n in m_neighbours)
        {
            if (n.coords == intersection.Coords)
                return true;
        }

        return false;
    }

    public bool IsJoined(Intersection intersection)
    {
        foreach(Neighbour n in m_neighbours)
        {
            if(n.coords == intersection.Coords)
            {
                return n.joined;
            }
        }
        return false;
    }

    public void SetJoined(Intersection intersection, bool val)
    {
        foreach(Neighbour n in m_neighbours)
        {
            if(n.coords == intersection.Coords)
            {
                n.SetJoined(val);
            }
        }
    }

    public int IndexOfInter(Intersection inter)
    {
        for(int i = 0; i < m_neighbours.Count; i++)
        {
            if (m_neighbours[i].coords == inter.Coords)
                return i;
        }
        return -1;
    }

    // Vérifie si le voisin créer un triangle
    /*private bool IsTriangle(int nbEdge, Intersection neighbour)
    {
        if (nbEdge < 0)
            return false;
        else if (this == neighbour && nbEdge == 0)
            return true;
        else
        {
            foreach(Neighbour n in neighbour.Neighbours)
            {
                if (IsTriangle(nbEdge - 1, n.inter))
                    return true;
            }
            return false;
        }
    }

    // Supprime les voisins formant des triangles
    public void DelTriangle()
    {
        List<Neighbour> copy = new List<Neighbour>(m_neighbours);

        foreach(Neighbour n in copy)
        {
            if(IsTriangle(2, n.inter))
            {
                int indexInter = m_neighbours.IndexOf(n);
                int indexNeig = n.inter.IndexOfInter(this);

                m_neighbours.RemoveAt(indexInter);
                n.inter.Neighbours.RemoveAt(indexNeig);
            }
        }
    }*/

    public Vector3 position
    {
        get { return m_position; }
        set { m_position = value; }
    }

    public List<Neighbour> Neighbours
    {
        get { return m_neighbours; }
        set { m_neighbours = value; }
    }

    public Vector2Int Coords
    {
        get { return m_coordonates; }
        set { m_coordonates = value; }
    }
}
