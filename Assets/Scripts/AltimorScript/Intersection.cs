﻿using System.Collections.Generic;
using UnityEngine;

public class Intersection
{
    //private List<Intersection> m_neighbours;
    //private List<bool> m_joined;

    // type structuré contenant les voisins
    public struct Neighbour
    {
        public bool joined;         // Booléen vérifiant si l'intersection est liée à ce voisin
        public Vector2Int coords;   // Coordonnées dans le tableau d'intersections
        public Vector3 positionOnIntersection;
    }

    private List<Neighbour> m_neighbours;   // Liste des voisins des l'intersection
    private Vector3 m_position;             // Position dans l'espace de l'intersection
    private Vector2Int m_coordonates;       // Coordonnées dans le tableau
    private int m_indexInMap;

    // CONSTRUCTEUR
    public Intersection(Vector3 position, Vector2Int coords)
    {
        m_position = position;
        m_coordonates = coords;

        m_neighbours = new List<Neighbour>();
    }

    // Calcul les côté de l'intersection sur lequel le voisin viendra se ranger
    private Vector3 ComputeIntersectionSide(Vector3 neighbourPos)
    {
        Vector3 Vect = neighbourPos - this.m_position;
        float multiplier = 0.5f;

        if (Mathf.Abs(Vect.x) > Mathf.Abs(Vect.z))
        {
            if (Vect.x < 0)
                return this.m_position + Vector3.left * multiplier;
            else
                return this.m_position + Vector3.right * multiplier;
        }
        else
        {
            if (Vect.z < 0)
                return this.m_position + Vector3.back * multiplier;
            else
                return this.m_position + Vector3.forward * multiplier;
        }
    }

    // Ajoute un voisin à l'intersection en prenant ses coordonnées dans le tableau en paramètre
    public void AddNeighbour(int i, int j, Vector3 neighbourPos)
    {
        Neighbour n;

        n.joined = false;
        n.coords = new Vector2Int(i, j);
        n.positionOnIntersection = ComputeIntersectionSide(neighbourPos);

        m_neighbours.Add(n);
    }

    // Regarde si l'intersection passée en paramètre est déjà une voisine
    public bool ContainsIntersection(Intersection intersection)
    {
        foreach (Neighbour n in m_neighbours)
        {
            if (n.coords == intersection.Coords)
                return true;
        }

        return false;
    }

    // Vérifie si l'intersection passée en paramètre et déjà jointe
    public bool IsJoined(Intersection intersection)
    {
        foreach (Neighbour n in m_neighbours)
        {
            if (n.coords == intersection.Coords)
            {
                return n.joined;
            }
        }
        return false;
    }

    // Change la valeur booléenne "joined" dans la liste des voisins correspondant à l'intersection passée en paramètre
    public void SetJoined(Intersection intersection, bool val)
    {
        for (int i = 0; i < m_neighbours.Count; i++)
        {
            if (m_neighbours[i].coords == intersection.Coords)
            {
                Neighbour n = m_neighbours[i];
                n.joined = true;
                m_neighbours[i] = n;
                return;
            }
        }
        Debug.Log("Intersection not in neighbours list");
    }

    // Retourne l'index de l'intersection passé en paramètre dans la liste des voisins
    public int IndexOfInter(Intersection inter)
    {
        for (int i = 0; i < m_neighbours.Count; i++)
        {
            if (m_neighbours[i].coords == inter.Coords)
                return i;
        }
        return -1;
    }

    // Vérifie si le chemin emprunté par le voisins "neighbour" créer un triangle (RECURSIF)
    private bool IsTriangle(int nbEdge, Intersection neighbour, Intersection[,] intersections)
    {
        if (nbEdge < 0)
            return false;
        else if (this == neighbour && nbEdge == 0)
            return true;
        else
        {
            // Parcours les voisins du voisin passé en paramètre
            foreach (Neighbour n in neighbour.Neighbours)
            {
                if (IsTriangle(nbEdge - 1, intersections[n.coords.x, n.coords.y], intersections))
                    return true;
            }
            return false;
        }
    }

    // Supprime les voisins formant des triangles
    public void DelTriangle(Intersection[,] intersections)
    {
        List<Neighbour> copy = new List<Neighbour>(m_neighbours);

        foreach (Neighbour n in copy)
        {
            if (IsTriangle(2, intersections[n.coords.x, n.coords.y], intersections))
            {
                int indexInter = m_neighbours.IndexOf(n);
                int indexNeig = intersections[n.coords.x, n.coords.y].IndexOfInter(this);

                m_neighbours.RemoveAt(indexInter);
                intersections[n.coords.x, n.coords.y].Neighbours.RemoveAt(indexNeig);
            }
        }
    }

    // Créer une plane au niveau de l'intersection (le futur parent est mit en argument)
    public void GenerateIntersection(Transform parent)
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position = m_position;
        plane.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
        plane.name = "Intersection";
        plane.transform.parent = parent;
    }

    // GETTER ---- SETTER

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

    public int IndexInMap
    {
        get { return m_indexInMap; }
        set { m_indexInMap = value; }
    }
}