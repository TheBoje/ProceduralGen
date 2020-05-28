using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    private Vector3 m_crossroad1;
    private Vector3 m_crossroad2;
    private Vector3 m_vectRoad;


    // Initialise les deux variables correspondant aux deux intersections
    public void Init(Vector3 cr1, Vector3 cr2)
    {
        m_crossroad1 = cr1;
        m_crossroad2 = cr2;
    }

    // Calcule la position du milieu des deux intersections
    private void ComputeMiddle(Vector3 pos1, Vector3 pos2)
    {
        Vector3 middle = new Vector3(
            (pos1.x + pos2.x) / 2,
            (pos1.y + pos2.y) / 2,
            (pos1.z + pos2.z) / 2
            );

        transform.position = middle;
    }

    // Calcul le vecteur directeur de la route
    private void ComputeVectRoad()
    {

        ComputeMiddle(m_crossroad1, m_crossroad2);

        m_vectRoad = m_crossroad2 - m_crossroad1;
        //Debug.Log("Vect : " + m_vectRoad);

    }

    // Calcul l'orientation de la route
    private void ComputeAngleRoad()
    {
        transform.LookAt(m_crossroad1);
    }

    public void SetRoad()
    {
        ComputeVectRoad();
        ComputeAngleRoad();
        transform.localScale = new Vector3(0.1f, 0.1f, m_vectRoad.magnitude / 10f); // Transforme la route pour que sa longueur soit la norme du vecteur directeur
    }

}
