using UnityEngine;

public class Road : MonoBehaviour
{
    private Vector3 m_crossroad1;
    private Vector3 m_crossroad2;
    private Vector3 m_vectRoad;
    private Vector3 m_normal;

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
        ComputeNormal();
        ComputeAngleRoad();
        transform.localScale = new Vector3(0.1f, 0.1f, m_vectRoad.magnitude / 10f); // Transforme la route pour que sa longueur soit la norme du vecteur directeur
    }

    // Calcul la normal au vecteur directeur de la route sur les x
    public void ComputeNormal()
    {
        GameObject point = new GameObject("point");
        point.transform.parent = transform;
        point.transform.localPosition = Vector3.zero + Vector3.right * 5f;

        m_normal = point.transform.position - transform.position;
        m_normal = m_normal.normalized;

        Destroy(point);
    }

    public Vector3 Vector
    {
        get { return m_vectRoad; }
    }

    public Vector3 NormalVect
    {
        get { return m_normal; }
    }
}