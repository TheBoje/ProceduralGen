using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** A PLACER SUR UN GAMEOBJECT VIDE **/

public class House : MonoBehaviour
{
    private const int MAX_FLOORS = 5;
    private const int MAX_ROOMS = 10;

    private int m_nbRooms;
    private int m_nbFloors;
    private List<GameObject> m_rooms;

    private int m_maxFloors;
    private int m_maxRooms;


    // Start is called before the first frame update
    void Start()
    {
        m_rooms = new List<GameObject>();
        m_nbRooms = 0;
        m_nbFloors = 1; //  WIP -> rajouter BOTTOM et TOP pour les ancrages

        m_maxFloors = Random.Range(1, MAX_FLOORS);
        m_maxRooms = Random.Range(1, MAX_ROOMS);
    }

    // Ajoute une pièce à la maison à l'étage passé en paramètre TODO -> Gérer les étages
    public void AddRoom(int floor, GameObject room)
    {
        if(m_nbRooms == 0)
        {
            GameObject addedRoom = Instantiate(room, transform.position, Quaternion.identity);
            m_nbRooms++;
            m_rooms.Add(addedRoom);
        }
        else if(m_nbRooms >= m_maxRooms)
        {
            Debug.Log("House.AddRoom - Max rooms reached");
        }
        else if(floor < 1 || floor > m_maxFloors)
        {
            Debug.Log("House.AddRoom - Invalid floor");
        }
        else
        {
            int indRoom = Random.Range(1, m_nbRooms);

            // On récupère les scripts des objets
            Room originRoom = m_rooms[indRoom].GetComponent<Room>();
            Room addingRoom = room.GetComponent<Room>();

            // On récupère l'identifiant d'une ancre aléatoirement
            int indAnchorOrigin = Random.Range(1, originRoom.NbAnchorsPoints);
            int indAnchorAdding = Random.Range(1, addingRoom.NbAnchorsPoints);

            GameObject addedRoom = originRoom.AddRoom(indAnchorOrigin, indAnchorAdding, room);
            m_nbRooms++;
            m_rooms.Add(addedRoom);
        }
    }

}
