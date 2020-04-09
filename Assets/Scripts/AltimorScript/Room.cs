using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    
    [SerializeField] private List<Vector3> m_anchorsPoints; // Liste des points d'ancrage
    private int m_nbAnchorsPoints;                          // Nombre de points d'ancrage
    [SerializeField] private Transform room;


    // Start is called before the first frame update
    void Start()
    {
        m_nbAnchorsPoints = m_anchorsPoints.Count;
        Vector3 roomAPoint = room.GetComponent<Room>().anchorsPoints[0];
        Instantiate(room, new Vector3(
            m_anchorsPoints[0].x * transform.localScale.x - roomAPoint.x * room.transform.localScale.x,
            m_anchorsPoints[0].y * transform.localScale.y - roomAPoint.y * room.transform.localScale.y,
            m_anchorsPoints[0].z * transform.localScale.z - roomAPoint.z * room.transform.localScale.z), Quaternion.identity); //(m_anchorsPoints[0] * transform.localScale) - room.GetComponent<Room>().anchorsPoints[0]
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
