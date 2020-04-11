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

    public void SetParent(GameObject newParent)
    {
        gameObject.transform.parent = newParent.transform;

        //Debug.Log("gameObject's Parent: " + gameObject.transform.parent.name);
    }

    // Ajoute une pièce sur le n-ième point d'ancrage (passé en paramètre)
    public void AddRoom(int indOrigin, int indAdding, GameObject room)
    {
        Room roomScript = room.GetComponent<Room>();
        if( (indOrigin < 0 || indOrigin > m_nbAnchorsPoints - 1) || (indAdding < 0 || indAdding > roomScript.NbAnchorsPoints) )
        {
            Debug.Log("Room.AddRoom() - Invalid index");
        }
        else
        {
            GameObject addedRoom = Instantiate(room, m_anchorsPoints[indOrigin].position, transform.rotation); //Euler(transform.rotation.x, transform.rotation.y - 90, transform.rotation.z)

            addedRoom.GetComponent<Room>().AnchorsPoints[indAdding].GetComponent<AnchorPoint>().RotateRoom(m_anchorsPoints[indOrigin].GetComponent<AnchorPoint>().AnchorSide);

            Vector3 anchorPos = addedRoom.GetComponent<Room>().AnchorsPoints[indAdding].position;
            Vector3 addedRoomPos = addedRoom.transform.position;


            Vector3 vectDeplacement = new Vector3(addedRoomPos.x - anchorPos.x,
                                                  addedRoomPos.y - anchorPos.y,
                                                  addedRoomPos.z - anchorPos.z);


            addedRoom.transform.position = addedRoom.transform.position + vectDeplacement;
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
