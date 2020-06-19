using System.Collections.Generic;
using UnityEngine;

public class CityHouses
{
    private const int MAX_HEIGHT = 5;
    private const int MIN_HEIGHT = 2;

    private const int MAX_WIDTH = 5;
    private const int MIN_WIDTH = 2;

    private const int MIN_DEPTH = 1;

    // Paramètres d'une maison en ville
    public struct HouseInterface
    {
        // Nombres de composants en hauteur, largeur et profondeur
        public int height;

        public int width;
        public int depth;

        public int doorPos; // Emplacement de la porte par rapport à un côté du bâtiment

        // Présence ou non du mur de gauche et/ou droite
        public bool leftWall;

        public bool rightWall;

        // Rotation de la façade (sur l'axe des y)
        public float angle;
    }

    // Composants de la maison en ville
    public struct HouseComponents
    {
        // Demander à Agnès pour dev une SD pratique et opti
    }

    // Structure d'une maison en ville
    public struct CityHouse
    {
        public HouseInterface hIntereface;
        public HouseComponents hComponents;
    }

    // Définie un type structuré CityHouse
    public CityHouse DefCityHouse(int height, int width, int depth, int doorPos, bool leftWall, bool rightWall, float angle)
    {
        HouseInterface inter;
        HouseComponents comp;
        CityHouse c;

        // ---- DEFINITION DE L'INTERFACE DE LA MAISON ---- //
        inter.height = height;
        inter.width = width;
        inter.depth = depth;

        inter.doorPos = doorPos;

        inter.leftWall = leftWall;
        inter.rightWall = rightWall;

        inter.angle = angle;

        // ---- DEFINITION DE LA MAISON ---- //
        c.hIntereface = inter;
        c.hComponents = comp;

        return c;
    }

    // Initialise une maison en donnant les paramètre // Les paramètres angle, maxWidth et maxDepth sont calculés en fonction de la route ainsi que du quartier
    public CityHouse InitCityHouse(List<CityHouse> houses, int maxWidth, int maxDepth, float angle)
    {
        int height, width, depth, doorPos;
        bool leftWall, rightWall;
        float Angle;

        if (houses.Count > 0)
        {
            CityHouse previousHouse = houses[houses.Count - 1];

            // Les maisons seront créée de la gauche vers la droite
            if (previousHouse.hIntereface.rightWall)
            {
                leftWall = false;
                height = Random.Range(MIN_HEIGHT, previousHouse.hIntereface.height);
                depth = Random.Range(MIN_DEPTH, previousHouse.hIntereface.depth);
            }
            else
            {
                height = Random.Range(MIN_HEIGHT, MAX_HEIGHT);
                depth = Random.Range(MIN_DEPTH, maxDepth);

                leftWall = (height > previousHouse.hIntereface.height || depth > previousHouse.hIntereface.depth);
            }
        }
        else
        {
            height = Random.Range(MIN_HEIGHT, MAX_HEIGHT);
            depth = Random.Range(MIN_DEPTH, maxDepth);
            leftWall = true;
        }

        rightWall = (Random.value > 0.5f);

        width = Random.Range(MIN_WIDTH, maxWidth);

        doorPos = Random.Range(0, width); // La position varie entre 0 et width - 1 (on commence à compter à partir de 0)

        return DefCityHouse(height, width, depth, doorPos, leftWall, rightWall, angle);
    }

    // Calcul la profondeur que peut atteindre la maison
    public int ComputeMaxDepth()
    {
        return 3;
    }

    // Créer les maisons (avec le type CityHouse) sur un côté d'une route
    public void GenerateRoadHouses(float roadLen, float roadAngle)
    {
        List<CityHouse> houses = new List<CityHouse>();

        int maxDepth = ComputeMaxDepth();

        int spaceLeft = (int)roadLen;

        while (spaceLeft > 0)
        {
            if (MAX_WIDTH > spaceLeft)
                houses.Add(InitCityHouse(houses, spaceLeft, maxDepth, roadAngle));
            else
                houses.Add(InitCityHouse(houses, MAX_WIDTH, maxDepth, roadAngle));

            spaceLeft -= houses[houses.Count - 1].hIntereface.width;
        }
    }
}