using UnityEngine;

#pragma warning disable 0108

public class CameraScript : MonoBehaviour
{
    [Range(0.2f, 10f)]
    public float mouseSensitivity = 1f;

    public bool mouseLocked = true;

    [SerializeField]
    [Range(0f, 1f)]
    private float lerpCoef = 0.15f; // [0, 1]

    private float maxYAngle = 80f;
    private float speed = 12f;

    private float mouseWhell;
    private Vector3 move;
    private Vector2 currentRotation;

    public Transform transform;

    private void FixedUpdate()
    {
        // DEPLACEMENT
        // Recupere les inputs du clavier "ZQSD" (voir Edit>Project Settings>Input Manager>Axes)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        // Applique les inputs a la camera avec lissage (proportionnel a lerpCoef [0, 1])
        move = transform.right * x * speed + transform.forward * z * speed;
        transform.position = Vector3.Lerp(transform.position, transform.position + move, lerpCoef);

        // ROTATION
        // Quand clique-gauche n'est pas enfoncé
        if (Input.GetMouseButton(0) == false)
        {
            // rotation de la caméra en fonction des inputs de la souris (voir Edit>Project Settings>Input Manager>Axes)
            currentRotation.x += Input.GetAxis("Mouse X") * mouseSensitivity * 50f * Time.deltaTime;
            currentRotation.y -= Input.GetAxis("Mouse Y") * mouseSensitivity * 50f * Time.deltaTime;
            // Angle modulo 360 pour eviter de gimball lock
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            // Limitation du degré de liberté (pour ne pas pouvoir regarder parfaitement en haut et en bas)
            currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
            // application de la rotation
            Camera.main.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        }

        // ELEVATION
        // Recuperation de l'input de la molette (voir Edit>Project Settings>Input Manager>Axes)
        mouseWhell = Input.GetAxis("Mouse ScrollWheel");
        if (mouseWhell != 0)
        {
            // Calcul de la nouvelle position de la camera (application d'un vecteur vertical)
            Vector3 newPos = transform.position + Vector3.up * mouseWhell * 10f;
            transform.position = Vector3.Lerp(transform.position, newPos, lerpCoef);
        }
    }

    private void Start()
    {
        // Rend le curseur invisible et centrée au milieu de la fenetre. ECHAP pour faire réaparaitre le curseur. Modifiable via l'inspector sur la Camera>CameraScript
        if (mouseLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}