using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorPoint : MonoBehaviour
{

    public enum side { NORTH, EAST, SOUTH, WEST}

    [SerializeField] side m_anchorSide; // à automatiser par la suite en fonction de la position

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Tourne le parent ou non (Room) en fonction de la face qui lui est présenté WIP : ajouter le haut et le bas
    public Vector3 RotateRoom(side otherAnchors)
    {

        Transform parent = transform.parent;
        Vector3 rot = new Vector3(0f, 90f, 0f);

        if(m_anchorSide == otherAnchors)
        {
            return new Vector3(0f, 180f, 0f);
            m_anchorSide = (side)(((int)m_anchorSide + 2) % 4);
        }
        else
        {
            float multiplier = 0.0f;

            if (((int)m_anchorSide - 1) % 4 == (int)otherAnchors) // vérifie le côté gauche
            {
                multiplier = 1f;
                //m_anchorSide = (side)(4 + ((int)m_anchorSide + 1) % 4);
            }
            else if (((int)m_anchorSide + 1) % 4 == (int)otherAnchors) // vérifie le côté droit
            {
                multiplier = -1f;
                //m_anchorSide = (side)( 4 + ((int)m_anchorSide - 1) % 4);
            }
                
            return (rot * multiplier);
        }
    }

    public void RotateAnchor(Vector3 rot)
    {
        side newSide = (side)((4 + ((int)m_anchorSide + (rot.y / 90f))) % 4);
        //Debug.Log("SIDE -> NEW SIDE = " + m_anchorSide + " -> " + newSide + " " + rot.y);
        m_anchorSide = newSide;
    }

    // GETTER
    public side AnchorSide
    {    
        get { return m_anchorSide; }
        set { m_anchorSide = (side)value; }
    }
}
