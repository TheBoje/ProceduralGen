using UnityEngine;

public class HalfEdge
{
    public enum TypeFace
    {
        BUILDING,
        PARK,
        ROAD,
        NONE
    }

    private HalfEdge m_next;        // Index de la demi arête suivante
    private HalfEdge m_previous;    // Index de la demi arête précédente
    private HalfEdge m_opposite;    // Index de la demi arête opposée
    private Vector3 m_position;     // Index du plongement correspondant à la position (Vector3)
    private Vector3 m_vect;         // Vecteur entre la position du brin et celle de son suivant
    private TypeFace m_type;        // type de la face auquel le brin est relié
    private int index;              // Index ne servant que au debug

    // Constructeurs
    public HalfEdge(int ind, Vector3 position, TypeFace type = TypeFace.NONE)
    {
        m_next = this;
        m_previous = this;
        m_opposite = this;
        m_position = position;
        m_type = type;
        index = ind;
    }

    public HalfEdge(HalfEdge next, HalfEdge previous, HalfEdge opposite, Vector3 position, TypeFace type = TypeFace.NONE)
    {
        m_next = next;
        m_previous = previous;
        m_opposite = opposite;
        m_position = position;
        m_type = type;
    }

    /// <summary>
    /// Met à jour le vecteur du brin
    /// </summary>
    public void RefreshVect()
    {
        m_vect = m_next.Position - m_position;
    }


    /// <summary>
    /// Change les valeurs du brin
    /// </summary>
    /// <param name="next">Nouveau brin suivant</param>
    /// <param name="previous">Nouveau brin précédent</param>
    /// <param name="opposite">Nouveau brin opposé</param>
    /// <param name="updateVect">Booléen mettant à jour le vecteur si celui est vrai et non sinon (vrai automatiquement si non précisé)</param>
    public void SetHalfEdge(HalfEdge next, HalfEdge previous, HalfEdge opposite, bool updateVect = true)
    {
        m_next = next;
        m_previous = previous;
        m_opposite = opposite;

        if (updateVect)
            this.RefreshVect();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Retourne vrai si le brin est dégénéré</returns>
    public bool IsDegenerated()
    {
        return (m_next == this && this == m_previous);
    }

    public override string ToString()
    {
        return "Index : " + index + " De " + m_position + " vers " + m_next.Position;
    }


    public HalfEdge Next
    {
        get { return m_next; }
        set { m_next = value; }
    }

    public HalfEdge Previous
    {
        get { return m_previous; }
        set { m_previous = value; }
    }

    public HalfEdge Opposite
    {
        get { return m_opposite; }
        set { m_opposite = value; }
    }

    public Vector3 Position
    {
        get { return m_position; }
        set { m_position = value; }
    }

    public TypeFace Type
    {
        get { return m_type; }
        set { m_type = value; }
    }

    public Vector3 Vect
    {
        get { return m_vect; }
    }
}