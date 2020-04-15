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

    [SerializeField] private List<GameObject> m_ableRooms;
    private int m_nbAbleRooms;

    private int m_maxFloors;
    [SerializeField] private int m_maxRooms;


    // Start is called before the first frame update
    public void Start()
    {
        m_rooms = new List<GameObject>();
        m_nbRooms = 0;
        m_nbFloors = 1; //  WIP -> rajouter BOTTOM et TOP pour les ancrages

        m_nbAbleRooms = m_ableRooms.Count;

        m_maxFloors = Random.Range(1, MAX_FLOORS);
        //m_maxRooms = Random.Range(1, MAX_ROOMS);

        Debug.Log(m_maxRooms);
    }

    // Ajoute une pièce à la maison à l'étage passé en paramètre TODO -> Gérer les étages
    public void AddRoom(int floor, GameObject room)
    {
        if(m_nbRooms == 0)
        {
            GameObject addedRoom = Instantiate(room, transform.position, Quaternion.identity);

            if (addedRoom != null)
            {
                addedRoom.GetComponent<Room>().SetParent(gameObject);
                m_nbRooms++;
                m_rooms.Add(addedRoom);
            }
            else
            {
                Debug.Log("House.AddRoom - null addedRoom ");
            }
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
            int indRoom = Random.Range(0, m_nbRooms - 1);

            // On récupère les scripts des objets
            Room originRoom = m_rooms[indRoom].GetComponent<Room>();
            Room addingRoom = room.GetComponent<Room>();

            originRoom.Start();
            addingRoom.Start();

            // On récupère l'identifiant d'une ancre aléatoirement
            int indAnchorOrigin = Random.Range(0, originRoom.NbAnchorsPoints - 1);
            int indAnchorAdding = Random.Range(0, addingRoom.NbAnchorsPoints - 1);

            GameObject addedRoom = originRoom.AddRoom(indAnchorOrigin, indAnchorAdding, room);
            if(addedRoom != null)
            {
                addedRoom.GetComponent<Room>().SetParent(gameObject);
                m_nbRooms++;
                m_rooms.Add(addedRoom);
            }
            else
            {
                Debug.Log("House.AddRoom - null addedRoom ");
            }
            
        }
    }

    IEnumerator Cor()
    {
        yield return new WaitForSeconds(2);
    }

    // Créer la maison avec un nombre nbRooms de pièces
    public void CreateHouse()
    {
        
        Debug.Log(m_maxRooms);
        for(int i = 0; i < m_maxRooms ; i++)
        {
            StartCoroutine(Cor());
            int id = Random.Range(0, m_nbAbleRooms);
            Debug.Log("ID : " + id);
            AddRoom(m_nbFloors, m_ableRooms[id]);
        }
    }

}
