using UnityEngine;

public class MoveCar : MonoBehaviour
{
    [SerializeField] private float velocidad = 5.0f;
    [SerializeField] private bool moverHaciaDerecha = true;

    void Update()
    {
        float direccion = moverHaciaDerecha ? -1.0f : 1.0f;
        float movimiento = velocidad * direccion * Time.deltaTime;
        transform.Translate(new Vector3(movimiento, 0, 0));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Destructible"))
        {
            Destroy(collision.gameObject);
        }
    }
}

