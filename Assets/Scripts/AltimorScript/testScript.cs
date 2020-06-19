using UnityEngine;

public class testScript : MonoBehaviour
{
    [SerializeField] private GameObject cr1;
    [SerializeField] private GameObject cr2;

    private void Start()
    {
        /*
                GameObject planeRoad = GameObject.CreatePrimitive(PrimitiveType.Plane);
                //Road road = new Road(cr1.transform.position, cr2.transform.position);
                road = planeRoad.AddComponent<Road>() as Road;
                //road.InitCrossroads(cr1, cr2);
                road.SetRoad();
                */
    }
}