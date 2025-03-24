using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;

    [Header("Salto y Gravedad")]
    [SerializeField] private float fuerzaSalto = 8f;
    [SerializeField] private float gravedad = 9.81f;

    private CharacterController controller;
    private Vector3 velocidadVertical;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Movimiento();
    }

    void Movimiento()
    {
        float movX = Input.GetAxis("Horizontal"); // A/D o Flechas Izq/Der
        float movZ = Input.GetAxis("Vertical");   // W/S o Flechas Arr/Abajo

        Vector3 movimiento = transform.right * movX + transform.forward * movZ;
        controller.Move(movimiento * velocidad * Time.deltaTime);

        // Aplicar gravedad
        if (controller.isGrounded)
        {
            velocidadVertical.y = -0.5f; // Pequeña fuerza hacia abajo para evitar bugs
            if (Input.GetButtonDown("Jump"))
            {
                velocidadVertical.y = fuerzaSalto;
            }
        }
        else
        {
            velocidadVertical.y -= gravedad * Time.deltaTime;
        }

        controller.Move(velocidadVertical * Time.deltaTime);
    }
}
