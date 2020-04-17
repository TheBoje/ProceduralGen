using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** A PLACER SUR UN GAMEOBJECT VIDE **/

public class House : MonoBehaviour
{
    private const int MAX_FLOORS = 5;
    private const int MAX_ROOMS = 10;
    private const float OFFSET_FIELD = 2f;

    private int m_nbRooms; // nombre de pièces
    private int m_nbFloors; // nombre d'étages
    private List<GameObject> m_rooms; // pièces de la maison
    private Vector3 m_field; // taille du terrain ( > 0)

    [SerializeField] private List<GameObject> m_ableRooms;
    private int m_nbAbleRooms;

    private int m_maxFloors;
    [SerializeField] private int m_maxRooms; // nombre maximale de pièces dans la maison


    // Start is called before the first frame update
    public void Start()
    {
        m_rooms = new List<GameObject>();
        m_nbRooms = 0;
        m_nbFloors = 1; //  WIP -> rajouter BOTTOM et TOP pour les ancrages

        m_field = new Vector3(0f, 0f, 0f);

        m_nbAbleRooms = m_ableRooms.Count;

        m_maxFloors = Random.Range(1, MAX_FLOORS);
        //m_maxRooms = Random.Range(1, MAX_ROOMS);

        Debug.Log(m_maxRooms);

        CreateHouse();
        ComputeField();
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

    //Calcul le terrain de la maison 
    private void ComputeField()
    {
        float max_x = 0f, min_x = 0f;
        float max_z = 0f, min_z = 0f;

        foreach(GameObject room in m_rooms)
        {
            Vector3 roomPos = room.transform.position;
            Vector3 roomScale = room.transform.localScale;

            // On calcule les maximums et minimums atteint par la maison
            max_x = (max_x < (roomPos.x + 0.5f * roomScale.x)) ? (roomPos.x + 0.5f * roomScale.x) : max_x;
            min_x = (min_x > (roomPos.x + (-0.5f * roomScale.x))) ? (roomPos.x + (-0.5f * roomScale.x)) : min_x;
           
            max_z = (max_z < (roomPos.z + 0.5f * roomScale.z)) ? (roomPos.z + 0.5f * roomScale.z) : max_z;
            min_z = (min_z > (roomPos.z + (-0.5f * roomScale.z))) ? (roomPos.z + (-0.5f * roomScale.z)) : max_z;
        }

        

        //m_field.x = Mathf.Abs(max_x - min_x) + OFFSET_FIELD;
        //m_field.z = Mathf.Abs(max_z - min_z) + OFFSET_FIELD;

        // calcul d'un terrain carré (pour simplifier) 
        m_field.x = Mathf.Abs(max_x - min_x) + OFFSET_FIELD;
        m_field.z = Mathf.Abs(max_z - min_z) + OFFSET_FIELD;

    }

    // GETTER
    public Vector3 Field
    {
        get { return m_field; }
    }

}
