using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    
    [SerializeField] private List<Vector3> m_anchorsPoints; // Liste des points d'ancrage
    private int m_nbAnchorsPoints;                          // Nombre de points d'ancrage


    // Start is called before the first frame update
    void Start()
    {
        m_nbAnchorsPoints = m_anchorsPoints.Count;
    }

    // GETTER
    public List<Vector3> anchorsPoints
    {
        get { return m_anchorsPoints; }
    }

    public int nbAnchorsPoints
    {
        get { return m_nbAnchorsPoints; }
    }
}
