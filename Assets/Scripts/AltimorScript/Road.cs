using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{

    [SerializeField] private GameObject crossroad1;
    [SerializeField] private GameObject crossroad2;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Calcul la longueur de la route
    private Vector3 ComputeVectRoad()
    {
        Vector3 pos1 = crossroad1.transform.position;
        Vector3 pos2 = crossroad2.transform.position;

        return pos2 - pos1;

    }

    // Calcul l'orientation de la route
    private Vector3 ComputeAngleRoad(Vector3 vect)
    {
        Vector3 rotation = new Vector3();

        rotation.x = Mathf.Atan(vect.z / vect.y);
        rotation.y = Mathf.Atan(vect.x / vect.z);
        rotation.z = Mathf.Atan(vect.y / vect.x);

        return rotation;
    }

}
