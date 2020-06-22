using System.Collections.Generic;
using UnityEngine;

public class District : MonoBehaviour
{
    private const int MAX_HOUSES = 3;
    private const int MIN_HOUSES = 3;

    private int m_nbHouses;

    private List<GameObject> m_houses;

    [SerializeField] private GameObject m_house = null;

    public void Awake()
    {
        m_houses = new List<GameObject>();
        m_nbHouses = Random.Range(MIN_HOUSES, MAX_HOUSES);
        CreateDistrict();
    }

    // Calcule la distance à laquelle le centre de la deuxième maison doit être placé
    private Vector3 ComputeNewCenterOfHouse(GameObject house, GameObject addedHouse)
    {
        Vector3 fieldHouse = house.GetComponent<House>().Field;
        Vector3 fieldAddedHouse = addedHouse.GetComponent<House>().Field;

        Debug.Log("fieldHouse : " + fieldHouse);
        Debug.Log("fieldAddedHouse : " + fieldAddedHouse);

        Vector3 newPos = new Vector3(house.transform.position.x, transform.position.y, house.transform.position.z + (fieldHouse.z / 2f) + (fieldAddedHouse.z / 2f));

        return newPos;
    }

    // Créer le quartier en y ajoutant les maisons
    private void AddHouse()
    {
        Debug.Log("Adding House ...");
        if (m_houses.Count == 0)
        {
            // On créer la première maison sur la position du quartier
            GameObject addedHouse = Instantiate(m_house, transform.position, Quaternion.identity);
            addedHouse.transform.parent = transform;
            addedHouse.name += " " + m_houses.Count;
            m_houses.Add(addedHouse);
        }
        else
        {
            GameObject addedHouse = Instantiate(m_house, transform.position, Quaternion.identity);
            addedHouse.transform.parent = transform;
            m_houses.Add(addedHouse);
            addedHouse.name += " " + m_houses.Count;

            Vector3 newPos = ComputeNewCenterOfHouse(m_houses[m_houses.Count - 2], addedHouse);

            addedHouse.transform.position = newPos;

            Vector3 side = addedHouse.transform.localPosition;
            side.z = addedHouse.transform.localPosition.z * Mathf.Pow(-1f, m_houses.Count);

            addedHouse.transform.localPosition = side;
        }
    }

    // Créer le quartier
    private void CreateDistrict()
    {
        for (int i = 0; i < m_nbHouses; i++)
        {
            AddHouse();
        }
    }
}