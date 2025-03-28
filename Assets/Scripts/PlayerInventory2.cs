    using System.Collections;
using UnityEngine;

public class Player2Inventory : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Transform mano; 
    [SerializeField] private GameObject cuboPuentePrefab;

    [Header("Materiales")]
    [SerializeField] private Material materialNormal; 
    [SerializeField] private Material materialActivo;

    [Header("UI")]
    [SerializeField] private GameObject mensajeUI;
    [SerializeField] private TMPro.TextMeshProUGUI textoMensajeUI;
    [SerializeField] private float tiempoMensaje = 2.0f;    

    private GameObject cuboEnMano;
    private Renderer rendererCubo;
    private bool enBorde = false; 
    private BridgeBuilder bridgeBuilder;

    void Start()
    {
        bridgeBuilder = FindObjectOfType<BridgeBuilder>();
        if (bridgeBuilder == null)
        {
            Debug.LogWarning("No se encontró BridgeBuilder en la escena");
        }
    }

    public bool TieneCubo()
    {
        return cuboEnMano != null;
    }

    public void RecogerCubo(GameObject cuboPrefab)
    {
        if (cuboEnMano == null && cuboPrefab != null)
        {
            cuboEnMano = Instantiate(cuboPrefab, mano.position, mano.rotation);
            cuboEnMano.transform.SetParent(mano);

            rendererCubo = cuboEnMano.GetComponent<Renderer>();
            if (rendererCubo != null && materialNormal != null)
            {
                rendererCubo.material = materialNormal;
            }
        }
    }

    public void SoltarCubo()
    {
        if (cuboEnMano != null)
        {
            cuboEnMano.transform.SetParent(null);

            Rigidbody rb = cuboEnMano.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            if (rb != null)
            {
                rb.AddForce(transform.forward * 2f, ForceMode.Impulse);
            }

            cuboEnMano = null;
            rendererCubo = null;
        }
    }

    void Update()
    {
        if (TieneCubo() && rendererCubo != null)
        {
            bool puedeColocar = enBorde && (bridgeBuilder != null && bridgeBuilder.PuedeColocarCubo());
            
            if (puedeColocar && materialActivo != null)
            {
                rendererCubo.material = materialActivo;
            }
            else if (!puedeColocar && materialNormal != null)
            {
                rendererCubo.material = materialNormal;
            }
        }

        if (TieneCubo())
        {
            if (enBorde && Input.GetKeyDown(KeyCode.U)) // Cambia la tecla para el segundo jugador
            {
                ColocarCubo();
            }

            if (Input.GetKeyDown(KeyCode.I)) // Cambia la tecla para el segundo jugador
            {
                SoltarCubo();
            }
        }
    }

    private void ColocarCubo()
    {
        if (bridgeBuilder == null)
        {
            Debug.LogWarning("No hay BridgeBuilder disponible");
            return;
        }

        if (!bridgeBuilder.PuedeColocarCubo())
        {
            MostrarMensaje("No puedes colocar un cubo aquí", true);
            return;
        }

        if (bridgeBuilder.IntentarColocarCubo(transform))
        {
            if (cuboEnMano != null)
            {
                Destroy(cuboEnMano);
                cuboEnMano = null;
                rendererCubo = null;
            }
            
            float progreso = bridgeBuilder.ObtenerProgreso();
            MostrarMensaje($"Progreso del puente: {progreso:P0}", true);
            
            if (bridgeBuilder.PuenteCompleto())
            {
                MostrarMensaje("¡Puente completado!", true);
            }
        }
    }

    public void MostrarMensaje(string texto, bool temporal = false)
    {
        if (mensajeUI != null && textoMensajeUI != null)
        {
            textoMensajeUI.text = texto;
            mensajeUI.SetActive(true);

            if (temporal)
            {
                StartCoroutine(OcultarMensaje(tiempoMensaje));
            }
        }
    }

    private IEnumerator OcultarMensaje(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        mensajeUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Borde"))
        {
            enBorde = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Borde"))
        {
            enBorde = false;
        }
    }
}