using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class District : MonoBehaviour
{

    private List<GameObject> m_houses;

    public void Start()
    {
        m_houses = new List<GameObject>();
    }

    
    // Calcule la distance à laquelle le centre de la deuxième maison doit être placé
    public Vector3 ComputeNewCenterOfHouse(GameObject house, GameObject addedHouse)
    {
        Vector3 fieldHouse = house.GetComponent<House>().Field;
        Vector3 fieldAddedHouse = addedHouse.GetComponent<House>().Field;

        return new Vector3(house.transform.position.x, 0f, house.transform.position.z + (fieldHouse.z / 2f) + (fieldAddedHouse.z / 2f));
    }

}
