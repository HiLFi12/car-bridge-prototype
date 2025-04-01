using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InteractableObject : MonoBehaviour, IInteractable
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

    [Header("Inputs")]
    [SerializeField] private KeyCode teclaInteraccionJugador1 = KeyCode.E;
    [SerializeField] private KeyCode teclaInteraccionJugador2 = KeyCode.U;

    [SerializeField] private int cubeOrder = 1;
    private bool enRango = false;
    private PlayerInventory playerInventory;
    private Player2Inventory player2Inventory;
    private bool esJugador2 = false;
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
        ActivarInteraccion();
    }

    void Update()
    {
        if (!enRango || !objetoDisponible) return;

        KeyCode teclaPulsar = esJugador2 ? teclaInteraccionJugador2 : teclaInteraccionJugador1;
        
        if (Input.GetKeyDown(teclaPulsar))
        {
            ProcesarInteraccion();
        }
    }

    public void ProcesarInteraccion()
    {
        if (esJugador2)
        {
            if (player2Inventory == null) return;

            if (!player2Inventory.TieneCubo())
            {
                EntregarCuboAlJugador2();
            }
            else
            {
                MostrarMensaje("Ya tienes un cubo", true);
            }
        }
        else
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

    private void EntregarCuboAlJugador2()
    {
        if (cuboBlancoPrefab == null || player2Inventory == null) return;

        if (spawnPoint != null)
        {
            player2Inventory.RecogerCubo(cuboBlancoPrefab);
        }
        else
        {
            player2Inventory.RecogerCubo(cuboBlancoPrefab);
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

    public void MostrarMensaje(string texto, bool temporal = false)
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
        // Comprobar si el objeto tiene PlayerInventory o Player2Inventory para identificar jugadores
        PlayerInventory playerInv = other.GetComponent<PlayerInventory>();
        Player2Inventory player2Inv = other.GetComponent<Player2Inventory>();
        
        if (playerInv != null || player2Inv != null)
        {
            enRango = true;
            
            // Determinar qué tipo de jugador es
            if (playerInv != null)
            {
                playerInventory = playerInv;
                esJugador2 = false;
            }
            else
            {
                player2Inventory = player2Inv;
                esJugador2 = true;
            }

            if (objetoDisponible)
            {
                KeyCode teclaMostrar = esJugador2 ? teclaInteraccionJugador2 : teclaInteraccionJugador1;
                
                if ((esJugador2 && player2Inventory != null && player2Inventory.TieneCubo()) ||
                   (!esJugador2 && playerInventory != null && playerInventory.TieneCubo()))
                {
                    MostrarMensaje("Ya tienes un cubo");
                }
                else
                {
                    MostrarMensaje($"Presiona {teclaMostrar} para recoger");
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
        // Comprobar si el objeto tiene PlayerInventory o Player2Inventory para identificar jugadores
        if (other.GetComponent<PlayerInventory>() != null || other.GetComponent<Player2Inventory>() != null)
        {
            enRango = false;

            if (mensajeUI != null)
            {
                mensajeUI.SetActive(false);
            }
            
            if (esJugador2)
            {
                player2Inventory = null;
            }
            else
            {
                playerInventory = null;
            }
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
            KeyCode teclaMostrar = esJugador2 ? teclaInteraccionJugador2 : teclaInteraccionJugador1;
            MostrarMensaje($"Presiona {teclaMostrar} para recoger");
        }
    }

    public bool PuedeInteractuar()
    {
        return objetoDisponible && cuboBlancoPrefab != null;
    }
}