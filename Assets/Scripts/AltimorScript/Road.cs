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

    private void SetInTheMiddle(Vector3 pos1, Vector3 pos2)
    {
        Vector3 middle = new Vector3(
            (pos1.x + pos2.x) / 2,
            (pos1.y + pos2.y) / 2,
            (pos1.z + pos2.z) / 2
            );

        transform.position = middle;
    }

    // Calcul la longueur de la route
    private Vector3 ComputeVectRoad()
    {
        Vector3 pos1 = crossroad1.transform.position;
        Vector3 pos2 = crossroad2.transform.position;

        SetInTheMiddle(pos1, pos2);

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
