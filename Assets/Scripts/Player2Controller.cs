using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Controller : MonoBehaviour
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
        float movX = Input.GetAxis("Horizontal2");
        float movZ = Input.GetAxis("Vertical2");

        Vector3 movimiento = (transform.right * movX + transform.forward * movZ).normalized;
        controller.Move(movimiento * velocidad * Time.deltaTime);

        if (controller.isGrounded)
        {
            velocidadVertical.y = -0.5f;
            if (Input.GetButtonDown("Jump2"))
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
