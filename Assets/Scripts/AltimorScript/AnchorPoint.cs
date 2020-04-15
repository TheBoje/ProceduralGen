using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorPoint : MonoBehaviour
{

    public enum side { NORTH, EAST, SOUTH, WEST, BOTTOM, TOP}

    [SerializeField] side m_anchorSide; // à automatiser par la suite en fonction de la position

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Tourne le parent ou non (Room) en fonction de la face qui lui est présenté WIP : ajouter le haut et le bas
    public void RotateRoom(side otherAnchors)
    {

        Transform parent = transform.parent;
        Vector3 rot = new Vector3(0f, 90f, 0f);

        if(m_anchorSide == otherAnchors)
        {
            parent.Rotate(new Vector3(0f, 180f, 0f));
            m_anchorSide = (side)(((int)m_anchorSide + 2) % 4);
        }
        else
        {
            float multiplier = 0.0f;

            if (4 + ((int)m_anchorSide - 1) % 4 == (int)otherAnchors) // vérifie le côté gauche
            {
                multiplier = -1f;
                m_anchorSide = (side)(4 + ((int)m_anchorSide + 1) % 4);
            }
            else if (4 + ((int)m_anchorSide + 1) % 4 == (int)otherAnchors) // vérifie le côté droit
            {
                multiplier = 1f;
                m_anchorSide = (side)( 4 + ((int)m_anchorSide - 1) % 4);
            }
                
            parent.Rotate(rot * multiplier);
        }
    }

    // GETTER
    public side AnchorSide
    {
        get { return m_anchorSide; }
    }
}
