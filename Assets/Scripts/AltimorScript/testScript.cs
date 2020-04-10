using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public GameObject roomOrigin;
    public GameObject roomAdded;

    // Start is called before the first frame update
    void Start()
    {
        roomOrigin.GetComponent<Room>().AddRoom(0, 0, roomAdded);
    }

    public void OnTriggerStay(Collider other)
    {
        Debug.Log("Je suis " + transform.name + "et je touche " + other.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
