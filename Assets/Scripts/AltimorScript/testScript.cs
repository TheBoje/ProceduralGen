using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public GameObject house;

    // Start is called before the first frame update
    void Start()
    {
        house.GetComponent<House>().Start();
        house.GetComponent<House>().CreateHouse();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

}
