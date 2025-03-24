using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InteractableObject : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject cuboBlancoPrefab; 
    [SerializeField] private Transform spawnPoint; 

    [Header("Interfaz")]
    [SerializeField] private GameObject mensajeUI; 
    [SerializeField] private TMPro.TextMeshProUGUI textoMensajeUI;
    [SerializeField] private float tiempoMensajeTemporal = 2.0f;

    [Header("Efectos")]
    [SerializeField] private AudioClip sonidoRecoger; 
    [SerializeField] private ParticleSystem efectoRecoger; 

    private bool enRango = false;
    private PlayerInventory playerInventory;
    private bool objetoDisponible = true;
    private Coroutine mensajeTemporalCoroutine;

    public delegate void InteraccionEventHandler(InteractableObject objeto);
    public event InteraccionEventHandler OnInteraccionDisponible;
    public event InteraccionEventHandler OnInteraccionCompletada;

    void Start()
    {
    
        if (mensajeUI == null)
        {
            Debug.LogWarning("MensajeUI no asignado en " + gameObject.name);
        }
        else
        {
            mensajeUI.SetActive(false);
        }

        if (cuboBlancoPrefab == null)
        {
            Debug.LogError("CuboBlancoPrefab no asignado en " + gameObject.name);
            objetoDisponible = false;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("SpawnPoint no asignado, se usará la posición del objeto");
            spawnPoint = transform;
        }
    }

    void Update()
    {
        if (enRango && objetoDisponible && Input.GetKeyDown(KeyCode.E))
        {
            ProcesarInteraccion();
        }
    }

    private void ProcesarInteraccion()
    {
        if (playerInventory == null) return;

        if (!playerInventory.TieneCubo())
        {
            EntregarCuboAlJugador();
        }
        else
        {
            MostrarMensaje("Ya tienes un cubo", true);
        }
    }

    private void EntregarCuboAlJugador()
    {
        if (cuboBlancoPrefab == null || playerInventory == null) return;

        if (spawnPoint != null)
        {
            playerInventory.RecogerCubo(cuboBlancoPrefab);
        }
        else
        {
            playerInventory.RecogerCubo(cuboBlancoPrefab);
        }

        if (mensajeUI != null)
        {
            mensajeUI.SetActive(false);
        }

        ReproducirEfectos();

        if (OnInteraccionCompletada != null)
        {
            OnInteraccionCompletada(this);
        }
    }

    private void ReproducirEfectos()
    {
        if (sonidoRecoger != null)
        {
            AudioSource.PlayClipAtPoint(sonidoRecoger, transform.position);
        }

        if (efectoRecoger != null)
        {
            efectoRecoger.Play();
        }
    }

    private void MostrarMensaje(string texto, bool temporal = false)
    {
        if (mensajeUI == null) return;

        if (textoMensajeUI != null)
        {
            textoMensajeUI.text = texto;
        }

        mensajeUI.SetActive(true);

        if (temporal)
        {
            if (mensajeTemporalCoroutine != null)
            {
                StopCoroutine(mensajeTemporalCoroutine);
            }

            mensajeTemporalCoroutine = StartCoroutine(OcultarMensajeDespuesDeTiempo(tiempoMensajeTemporal));
        }
    }

    private IEnumerator OcultarMensajeDespuesDeTiempo(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);

        if (mensajeUI != null)
        {
            mensajeUI.SetActive(false);
        }

        mensajeTemporalCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enRango = true;
            playerInventory = other.GetComponent<PlayerInventory>();

            if (objetoDisponible)
            {
                if (playerInventory != null && playerInventory.TieneCubo())
                {
                    MostrarMensaje("Ya tienes un cubo");
                }
                else
                {
                    MostrarMensaje("Presiona E para recoger");
                }
            }

            if (OnInteraccionDisponible != null)
            {
                OnInteraccionDisponible(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enRango = false;

            if (mensajeUI != null)
            {
                mensajeUI.SetActive(false);
            }
            playerInventory = null;
        }
    }

    public void DesactivarInteraccion()
    {
        objetoDisponible = false;

        if (mensajeUI != null)
        {
            mensajeUI.SetActive(false);
        }
    }

    public void ActivarInteraccion()
    {
        objetoDisponible = true;

        if (enRango && mensajeUI != null)
        {
            MostrarMensaje("Presiona E para recoger");
        }
    }
    public bool PuedeInteractuar()
    {
        return objetoDisponible && cuboBlancoPrefab != null;
    }
}