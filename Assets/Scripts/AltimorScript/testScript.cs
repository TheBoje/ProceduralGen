using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_houses;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject house in m_houses)
        {
            house.GetComponent<House>().Start();
            house.GetComponent<House>().CreateHouse();
        }
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }

}
