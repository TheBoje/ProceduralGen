using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    
    [SerializeField] private List<Transform> m_anchorsPoints; // Liste des points d'ancrage
    private int m_nbAnchorsPoints;                          // Nombre de points d'ancrage


    // Start is called before the first frame update
    void Start()
    {
        //m_anchorsPoints = new List<Transform>();

        m_nbAnchorsPoints = m_anchorsPoints.Count;
        Debug.Log(m_nbAnchorsPoints);
    }

    // Place tous les points d'ancrage dans la liste
    private void InitAnchorsPoints()
    {
        foreach(Transform child in transform)
        {
            if(child.CompareTag("Anchor"))
            {
                if(child == null)
                {
                    Debug.Log("NULL");
                }
                else
                {
                    m_anchorsPoints.Add(child);
                }
            }
        }
    }

    // Ajoute une pièce sur le n-ième point d'ancrage (passé en paramètre)
    public void AddRoom(int indOrigin, int indAdding, GameObject room)
    {
        Room roomScript = room.GetComponent<Room>();
        Debug.Log(m_nbAnchorsPoints);
        Debug.Log(roomScript.NbAnchorsPoints);
        if( (indOrigin < 0 || indOrigin > m_nbAnchorsPoints - 1) || (indAdding < 0 || indAdding > roomScript.NbAnchorsPoints) )
        {
            Debug.Log("Room.AddRoom() - Invalid index");
        }
        else
        {
            Instantiate(room, m_anchorsPoints[indOrigin].position + roomScript.AnchorsPoints[indAdding].position, Quaternion.identity);
        }
    }

    // GETTER
    public List<Transform> AnchorsPoints
    {
        get { return m_anchorsPoints; }
    }

    public int NbAnchorsPoints
    {
        get { return m_nbAnchorsPoints; }
    }
}
