using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Security.Cryptography;
using Random = UnityEngine.Random;


public class TransformInfo
{
    public Vector3 position;
    public Quaternion rotation;
}

public class LSystemScript : MonoBehaviour
{
    [SerializeField] private int iterations = 2;
    [SerializeField] private GameObject Cylinder;
    [SerializeField] private GameObject Plane;
    [SerializeField] private float length = 10f;
    [SerializeField] private float angle = 90f;
    [SerializeField] private float initialeSize = 10f;
 

    //Entrez l'axiom du L-system
    public const string axiom = "F";


    
    private Stack<TransformInfo> transformStack;
    private Dictionary<char, string> rules;
    private string currentString = string.Empty;
    private Vector3 actualAngle;
    
    //initiation des couleurs du tronc et des feuilles
    private Color barkColor = Color.black;
    private Color leavesColor = Color.green;
    private Renderer rend;
    private int green = 0;

    void Start()
    {
        transformStack = new Stack<TransformInfo>();

        rules = new Dictionary<char, string>
        {
            //tree C, n=4, angle = 22.5, axiom = F
            {'F', "FF-[-F+F+FL]+[+F-F-FL]" }

            //{'F', "FF-[-F+F+F]+[+F-F-F]" }

            //dragon curve, angle 90, axiom FX
            //{'X', "X+YF+"},
            //{'Y', "-FX-Y"}

            //{'X', "F[+X]F[-X]+X" },
            //{'F', "FF" }

            //{'F',"[F-F]+[F-F]+[F-F]+[F-F]" }
            //{'F',"FF-F-F-F-F-F+F" },

            //{'F',"F[-&<F][<++&F]||F[--&>F][+&F]" },
            //{'F',"F[&+F]F[->F][->F][&F]" },

            //{'A', "[&FFFA]////[&FFFA]////[&FFFA]" }

            //{'A', "B-F+CFC+F-D&F∧D-F+&&CFC+F+B//" },
            //{'B', "A&F∧CFB∧F∧D∧∧-F-D∧|F∧B|FC∧F∧A//" },
            //{'C', "|D∧|F∧B-F+C∧F∧A&&FA&F∧C+F+B∧F∧D//" },
            //{'D', "|CFB-F+B|FA&F∧A&&FB-F+B|FC//" }
            //{'F', "FF" }

            //{'1',"2+[1+]--//[1]++1" },
            //{'2',"F3[//&&4][//^^4]F3" },
            //{'3',"3F3" }
        };
        Generate();
    }

    private void Generate()
    {
        currentString = axiom;

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < iterations; i++)
        {
            foreach (char c in currentString)
            {
                sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
            }
            
            currentString = sb.ToString();
            sb = new StringBuilder();
        }

        foreach (char c in currentString)
        {
            var randomFloat = Random.Range(-10.0f, 10.0f);
            switch (c)
            {
                //on créer une branche
                case 'F':
                    Vector3 initialPosition = transform.position;
                    transform.Translate(Vector3.up * initialeSize);

                    GameObject treeSegment = Instantiate(Cylinder);

                    treeSegment.transform.localPosition = initialPosition;
                    treeSegment.transform.localScale = Vector3.zero;    
                    treeSegment.transform.localScale += (Vector3.right * initialeSize/2);    
                    treeSegment.transform.localScale += (Vector3.forward * initialeSize/2);    
                    
                    treeSegment.transform.localScale += (Vector3.up * initialeSize/2);
                    treeSegment.transform.localRotation = transform.rotation;

                    rend = treeSegment.GetComponent<Renderer>();
                    barkColor.g = green;
                    barkColor.b = green;
                    rend.material.color = barkColor;

                    //var treeColor = treeSegment.GetComponent<Renderer>();
                    //treeColor.material.SetColor("Bark", Color.blue);
                    //treeSegment.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
                    // treeSegment.GetComponent<LineRenderer>().SetPosition(1, transform.position);
                    break;

                    //on créer une feuille
                case 'L':
                    Vector3 leafPosition = transform.position;
                    transform.Translate(Vector3.up * initialeSize);

                    GameObject leafSegment = Instantiate(Plane);

                    leafSegment.transform.localPosition = leafPosition;
                    leafSegment.transform.localScale = Vector3.zero;
                    leafSegment.transform.localScale += (Vector3.right * initialeSize / 10);
                    leafSegment.transform.localScale += (Vector3.forward * initialeSize / 10);

                    leafSegment.transform.localScale += (Vector3.up * initialeSize / 10);
                    leafSegment.transform.localRotation = transform.rotation;

                    rend = leafSegment.GetComponent<Renderer>();
                    leavesColor.g = green;
                    rend.material.color = leavesColor;
                    break;

                case 'X':
                      break; 
                case 'Y':
                      break; 
                case '1':
                      break; 
                case '2':
                      break; 
                case '3':
                      break; 
                case '4':
                      break; 
                case '5':
                      break;
                             case 'A':
                                   break;  
                //          case 'B':
                //                 break;               
                //        case 'C':
                //            break;  
                //        case 'D':
                //           break;

                case '+':
                    transform.Rotate(Vector3.forward * (angle+randomFloat));
                      break;

               case '-':
                    transform.Rotate(Vector3.back * (angle + randomFloat));
                    break;

               case '^':
                    transform.Rotate(Vector3.up * (angle + randomFloat));
                    break;

               case '&':
                    transform.Rotate(Vector3.down * (angle + randomFloat));
                    break;

                case '\\': //case \
                    transform.Rotate(Vector3.left * (angle + randomFloat));
                    break;

                case '/':
                    transform.Rotate(Vector3.right * (angle + randomFloat));
                    break;
                
                case '|':
                    transform.Rotate(Vector3.forward * -(angle + randomFloat));
                    break;

                case '[':
                    transformStack.Push(new TransformInfo()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    });
                    initialeSize /= 1.5f ;
                    if (green <= 254)
                        green += 1;
                      break;

                case ']':
                    TransformInfo ti = transformStack.Pop();
                    transform.position = ti.position;
                    transform.rotation = ti.rotation;
                    initialeSize *= 1.5f;
                    if (green > 1)
                        green -= 1;
                    break;

                default:
                    throw new InvalidOperationException("invalid L Tree operation");
            }
            
        }
    }
}
