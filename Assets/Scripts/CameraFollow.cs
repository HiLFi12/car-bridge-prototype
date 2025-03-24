using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Transform objetivo;                     

    [Header("Posición")]
    [SerializeField] private Vector3 offset = new Vector3(10f, 10f, -10f); 
    [SerializeField] private bool seguirRotacionY = false;          
    [SerializeField] private float alturaMinima = 1.0f;

    [Header("Suavizado")]
    [SerializeField] private bool usarSuavizado = true;             
    [SerializeField] private float velocidadSuavizado = 5.0f;       

    [Header("Limites de Cámara")]
    [SerializeField] private bool usarLimites = false;               
    [SerializeField] private Vector2 limitesX = new Vector2(-50f, 50f); 
    [SerializeField] private Vector2 limitesZ = new Vector2(-50f, 50f); 

    [Header("Colisiones")]
    [SerializeField] private bool detectarColisiones = false;        
    [SerializeField] private float distanciaDeteccion = 5.0f;        
    [SerializeField] private LayerMask capasColision;                

    private Vector3 posicionObjetivo;
    private Vector3 posicionSuavizada;
    private float distanciaAlObjetivo;
    private Vector3 direccionAlObjetivo;

    private void Start()
    {
        if (objetivo == null)
        {
            Debug.LogWarning("CameraFollow: No hay objetivo asignado. Asigna un objetivo en el Inspector.");
            enabled = false; 
            return;
        }

        distanciaAlObjetivo = offset.magnitude;

        posicionObjetivo = CalcularPosicionObjetivo();
        transform.position = posicionObjetivo;

        transform.LookAt(objetivo);
    }

    private void LateUpdate()
    {
        if (objetivo == null) return;

        posicionObjetivo = CalcularPosicionObjetivo();

        if (usarLimites)
        {
            posicionObjetivo.x = Mathf.Clamp(posicionObjetivo.x, limitesX.x, limitesX.y);
            posicionObjetivo.z = Mathf.Clamp(posicionObjetivo.z, limitesZ.x, limitesZ.y);
        }

        if (detectarColisiones)
        {
            direccionAlObjetivo = (objetivo.position - posicionObjetivo).normalized;
            AjustarPorColisiones();
        }

        if (usarSuavizado)
        {
            transform.position = Vector3.Lerp(transform.position, posicionObjetivo,
                velocidadSuavizado * Time.deltaTime);
        }
        else
        {
            transform.position = posicionObjetivo;
        }

        if (transform.position.y < alturaMinima)
        {
            Vector3 pos = transform.position;
            pos.y = alturaMinima;
            transform.position = pos;
        }

        transform.LookAt(objetivo);
    }

    private Vector3 CalcularPosicionObjetivo()
    {
        Vector3 targetPos;

        if (seguirRotacionY)
        {
            float anguloY = objetivo.eulerAngles.y;
            Quaternion rotacion = Quaternion.Euler(0, anguloY, 0);
            Vector3 offsetRotado = rotacion * offset;

            targetPos = objetivo.position + offsetRotado;
        }
        else
        {
            targetPos = objetivo.position + offset;
        }
        return targetPos;
    }

    private void AjustarPorColisiones()
    {
        RaycastHit hit;
        Vector3 inicioRay = objetivo.position;
        Vector3 direccionRay = -direccionAlObjetivo; 

        if (Physics.Raycast(inicioRay, direccionRay, out hit, distanciaAlObjetivo, capasColision))
        {
            float distanciaAjustada = hit.distance * 0.8f; 
            posicionObjetivo = objetivo.position - direccionRay * distanciaAjustada;

            Debug.DrawLine(inicioRay, hit.point, Color.red);
        }
        else
        {
            Debug.DrawLine(inicioRay, inicioRay + direccionRay * distanciaAlObjetivo, Color.green);
        }
    }

    public void CambiarObjetivo(Transform nuevoObjetivo)
    {
        if (nuevoObjetivo != null)
        {
            objetivo = nuevoObjetivo;
        }
    }

    public void AjustarOffset(Vector3 nuevoOffset)
    {
        offset = nuevoOffset;
        distanciaAlObjetivo = offset.magnitude;
    }

    public void AjustarVelocidadSuavizado(float nuevaVelocidad)
    {
        velocidadSuavizado = Mathf.Max(0.1f, nuevaVelocidad);
    }

    private void OnDrawGizmosSelected()
    {
        if (objetivo == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(objetivo.position, 1f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(objetivo.position, objetivo.position + offset);
        Gizmos.DrawWireSphere(objetivo.position + offset, 0.5f);

        if (usarLimites)
        {
            Gizmos.color = Color.red;
            Vector3 center = new Vector3((limitesX.x + limitesX.y) / 2, transform.position.y,
                (limitesZ.x + limitesZ.y) / 2);
            Vector3 size = new Vector3(limitesX.y - limitesX.x, 2, limitesZ.y - limitesZ.x);
            Gizmos.DrawWireCube(center, size);
        }
    }
}