using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    
    [SerializeField] private List<Transform> m_anchorsPoints; // Liste des points d'ancrage
    private int m_nbAnchorsPoints;                          // Nombre de points d'ancrage


    // Start is called before the first frame update
    public void Start()
    {
        //m_anchorsPoints = new List<Transform>();

        m_nbAnchorsPoints = m_anchorsPoints.Count;
        //Debug.Log("NOM : " + gameObject.name + " | nbAnchors : " + m_nbAnchorsPoints);
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
    public GameObject AddRoom(int indOrigin, int indAdding, GameObject room)
    {
        Room roomScript = room.GetComponent<Room>();
        if( (indOrigin < 0 || indOrigin > m_nbAnchorsPoints - 1) || (indAdding < 0 || indAdding > roomScript.NbAnchorsPoints - 1) )
        {
            Debug.Log("Room.AddRoom() - Invalid index (indOrigin, m_nbAnchorsPoints, indAdding, roomScript.NbAnchorsPoints) : " + indOrigin + ", " + m_nbAnchorsPoints + ", " + indAdding + ", " + roomScript.NbAnchorsPoints);
            return null;
        }
        else
        {
            
            GameObject addedRoom = Instantiate(room, m_anchorsPoints[indOrigin].position, transform.rotation); //Euler(transform.rotation.x, transform.rotation.y - 90, transform.rotation.z)

            Vector3 rotation = addedRoom.GetComponent<Room>().AnchorsPoints[indAdding].GetComponent<AnchorPoint>().RotateRoom(m_anchorsPoints[indOrigin].GetComponent<AnchorPoint>().AnchorSide);
            addedRoom.transform.Rotate(rotation);


            foreach(Transform anchorPoint in addedRoom.GetComponent<Room>().AnchorsPoints)
            {
                anchorPoint.GetComponent<AnchorPoint>().RotateAnchor(rotation);
            }

            Vector3 anchorPos = addedRoom.GetComponent<Room>().AnchorsPoints[indAdding].position;
            Vector3 addedRoomPos = addedRoom.transform.position;

            Vector3 vectDeplacement = addedRoomPos - anchorPos;

            addedRoom.transform.position = addedRoom.transform.position + vectDeplacement;

            // On enlève les ancres de la liste
            RemInList(indOrigin);
            addedRoom.GetComponent<Room>().RemInList(indAdding);

            //Destroy(m_anchorsPoints[indOrigin]);
            //Destroy(addedRoom.GetComponent<Room>().AnchorsPoints[indAdding]);

            return addedRoom;
            
        }
    }

    // Enlève l'objet de la liste dont l'indice est passé en paramètre
    public void RemInList(int index)
    {
        if(m_nbAnchorsPoints == 0)
        {
            Debug.Log("Room.RemInList - Empty List");
        }
        else if(m_nbAnchorsPoints < 0)
        {
            Debug.Log("Room.RemInList - Problem : m_nbAnchorsPoints < 0");
        }
        else if(index > m_nbAnchorsPoints - 1 || index < 0)
        {
            Debug.Log("Room.RemInList - Index out of range : " + index);
        }
        else
        {
            m_anchorsPoints.RemoveAt(index);
            m_nbAnchorsPoints--;
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
