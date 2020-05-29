using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town : MonoBehaviour
{

    [SerializeField] private GameObject m_district;

    private void InstantiateDistrict(GameObject road)
    {
        float mult = 5f;

        Instantiate(m_district, road.transform.position + Vector3.left * mult, Quaternion.identity);
        Instantiate(m_district, road.transform.position + Vector3.right * mult, Quaternion.identity);
    }

    public void BuildTown()
    {
        foreach(Transform road in gameObject.transform)
        {
            if(road.name == "Road")
            {
                InstantiateDistrict(road.gameObject);
            }
        }
    }


}
