using UnityEngine;

public class BridgeVisualizer : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BridgeBuilder bridgeBuilder;
    [SerializeField] private GameObject marcadorPrefab;
    
    [Header("Configuración")]
    [SerializeField] private Color colorMarcador = new Color(0, 1, 0, 0.3f);
    [SerializeField] private float velocidadPulsacion = 1.5f;
    [SerializeField] private float escalaPulsacion = 0.2f;
    
    private GameObject marcadorActual;
    private Transform jugador;
    private bool mostrarMarcador = false;
    
    void Start()
    {
        if (bridgeBuilder == null)
        {
            bridgeBuilder = GetComponent<BridgeBuilder>();
            
            if (bridgeBuilder == null)
            {
                bridgeBuilder = FindObjectOfType<BridgeBuilder>();
                if (bridgeBuilder == null)
                {
                    Debug.LogError("No se encontró ningún BridgeBuilder");
                    enabled = false;
                    return;
                }
            }
        }
        
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (jugador == null)
        {
            Debug.LogWarning("No se encontró al jugador. Es posible que el visualizador no funcione correctamente.");
        }
        
        CrearMarcador();
    }
    
    void Update()
    {
        PlayerInventory playerInventory = jugador?.GetComponent<PlayerInventory>();
        
        bool deberiaVerMarcador = playerInventory != null && 
                                  playerInventory.TieneCubo() && 
                                  bridgeBuilder.PuedeColocarCubo();
        
        if (deberiaVerMarcador != mostrarMarcador)
        {
            mostrarMarcador = deberiaVerMarcador;
            
            if (marcadorActual != null)
            {
                marcadorActual.SetActive(mostrarMarcador);
            }
        }
        
        if (mostrarMarcador && marcadorActual != null)
        {
            float pulsacion = Mathf.Sin(Time.time * velocidadPulsacion) * escalaPulsacion + 1.0f;
            marcadorActual.transform.localScale = Vector3.one * pulsacion;
        }
    }
    
    private void CrearMarcador()
    {
        if (marcadorPrefab != null)
        {
            marcadorActual = Instantiate(marcadorPrefab, transform.position, Quaternion.identity);
            marcadorActual.SetActive(false);
            
            Renderer renderer = marcadorActual.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = colorMarcador;
            }
        }
        else
        {
            marcadorActual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marcadorActual.transform.localScale = new Vector3(0.9f, 0.2f, 0.9f);
            marcadorActual.GetComponent<Collider>().enabled = false;
            
            Renderer renderer = marcadorActual.GetComponent<Renderer>();
            Material material = new Material(Shader.Find("Transparent/Diffuse"));
            material.color = colorMarcador;
            renderer.material = material;
            
            marcadorActual.SetActive(false);
        }
    }
    
    public void ActualizarPosicionMarcador(Vector3 posicion)
    {
        if (marcadorActual != null)
        {
            marcadorActual.transform.position = posicion;
        }
    }
} 