using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    private GameObject m_crossroad1;
    private GameObject m_crossroad2;
    private Vector3 m_vectRoad;

    public void InitCrossroads(GameObject cr1, GameObject cr2)
    {
        m_crossroad1 = cr1;
        m_crossroad2 = cr2;
    }

    private void ComputeMiddle(Vector3 pos1, Vector3 pos2)
    {
        Vector3 middle = new Vector3(
            (pos1.x + pos2.x) / 2,
            (pos1.y + pos2.y) / 2,
            (pos1.z + pos2.z) / 2
            );

        transform.position = middle;
    }

    // Calcul la longueur de la route
    private void ComputeVectRoad()
    {
        Vector3 pos1 = m_crossroad1.transform.position;
        Vector3 pos2 = m_crossroad2.transform.position;

        ComputeMiddle(pos1, pos2);

        m_vectRoad = pos2 - pos1;
        Debug.Log("Vect : " + m_vectRoad);

    }

    // Calcul l'orientation de la route
    private void ComputeAngleRoad()
    {
        Vector3 rotation = new Vector3(0f, 0f, 0f);

        rotation.x = Mathf.Atan(m_vectRoad.z / m_vectRoad.y);
        rotation.y = Mathf.Atan(m_vectRoad.x / m_vectRoad.z);
        rotation.z = Mathf.Atan(m_vectRoad.y / m_vectRoad.x);

        transform.rotation = Quaternion.Euler(m_vectRoad);
    }

    public void SetRoad()
    {
        ComputeVectRoad();
        //ComputeAngleRoad();
        transform.localScale = new Vector3(m_vectRoad.magnitude / 10f, 0.1f, 0.1f);
    }
    

}
