using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town : MonoBehaviour
{

    [SerializeField] private GameObject m_district;

    private void InstantiateDistrict(GameObject road)
    {
        float mult = 30f;

        GameObject district1 = Instantiate(m_district, road.transform.position, road.transform.rotation);
        //GameObject district2 = Instantiate(m_district);

        

        //district1.transform.localPosition += (Vector3.right * mult);

        district1.transform.parent = road.transform;

        //district1.GetComponent<District>().Init();
        //district2.transform.parent = road.transform;

        //district1.transform.Rotate(road.transform.eulerAngles.x * (-1f), 0f, road.transform.eulerAngles.z * (-1f));
        district1.transform.Rotate(Vector3.zero);
        //district2.transform.Rotate(0f, road.transform.eulerAngles.y, 0f);


        district1.transform.localPosition += Vector3.left * mult;
       //district2.transform.position = road.transform.position + Vector3.right * mult;*/

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
