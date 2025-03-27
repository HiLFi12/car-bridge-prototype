using System.Collections.Generic;
using UnityEngine;

public class BridgeBuilder : MonoBehaviour
{
    [Header("Terrenos")]
    [SerializeField] private Transform terreno1;
    [SerializeField] private Transform terreno2;
    
    [Header("Construcción")]
    [SerializeField] private GameObject cuboPuentePrefab;
    [SerializeField] private float tamañoCubo = 1f;
    [SerializeField] private float ajusteAltura = 0.0f; 
    [SerializeField] private bool usarCurvaAltura = false; 
    [SerializeField] [Range(0, 1)] private float alturaMaximaCurva = 0.2f; 
    
    [Header("Eventos")]
    [SerializeField] private bool notificarAlManager = true;
    [SerializeField] private int indicePuente = 0; 
    
    private List<Vector3> posicionesCubos = new List<Vector3>();
    private List<bool> cuboColocado = new List<bool>();
    private int totalCubosNecesarios;
    private bool puenteCompleto = false;
    private BridgeVisualizer visualizador;
    
    private void Awake()
    {
        visualizador = GetComponent<BridgeVisualizer>();
        if (visualizador == null && gameObject.activeInHierarchy)
        {
            visualizador = gameObject.AddComponent<BridgeVisualizer>();
        }
    }
    
    private void Start()
    {
        if (terreno1 == null || terreno2 == null)
        {
            Debug.LogError("Faltan referencias a los terrenos en BridgeBuilder");
            return;
        }
        
        CalcularPosicionesCubos();
        ActualizarVisualizador();
    }
    
    private void CalcularPosicionesCubos()
    {
        posicionesCubos.Clear();
        cuboColocado.Clear();
        
        Vector3 pos1 = terreno1.position;
        Vector3 pos2 = terreno2.position;
        
        float alturaTerreno1 = pos1.y;
        float alturaTerreno2 = pos2.y;
        
        Vector3 direccionHorizontal = new Vector3(pos2.x - pos1.x, 0, pos2.z - pos1.z);
        float distanciaHorizontal = direccionHorizontal.magnitude;
        
        Vector3 dirHorizontalNormalizada = direccionHorizontal.normalized;
        
        totalCubosNecesarios = Mathf.CeilToInt(distanciaHorizontal / tamañoCubo);
        
        float diferenciaAltura = alturaTerreno2 - alturaTerreno1;
        
        for (int i = 0; i < totalCubosNecesarios; i++)
        {
            float distanciaDesdeInicio = tamañoCubo * (i + 0.5f); 
            float factorProgreso = distanciaDesdeInicio / distanciaHorizontal; 
            
            Vector3 posicionBase = pos1 + dirHorizontalNormalizada * distanciaDesdeInicio;
            
            float altura;
            
            if (usarCurvaAltura)
            {
                float factorCurva = Mathf.Sin(factorProgreso * Mathf.PI); 
                float alturaExtra = factorCurva * alturaMaximaCurva * distanciaHorizontal;
                
                altura = Mathf.Lerp(alturaTerreno1, alturaTerreno2, factorProgreso) + alturaExtra;
            }
            else
            {
                altura = Mathf.Lerp(alturaTerreno1, alturaTerreno2, factorProgreso);
            }
            
            altura += ajusteAltura;
            
            Vector3 posicionCubo = new Vector3(posicionBase.x, altura, posicionBase.z);
            
            posicionesCubos.Add(posicionCubo);
            cuboColocado.Add(false);
        }
        
        Debug.Log($"Se necesitan {totalCubosNecesarios} cubos para completar el puente entre alturas {alturaTerreno1} y {alturaTerreno2}");
    }
    
    private void Update()
    {
        ActualizarVisualizador();
    }
    
    private void ActualizarVisualizador()
    {
        if (visualizador == null || puenteCompleto)
            return;
        
        Vector3 posicionSiguiente = Vector3.zero;
        bool encontrada = false;
        
        for (int i = 0; i < cuboColocado.Count; i++)
        {
            if (!cuboColocado[i])
            {
                if (i == 0 || cuboColocado[i - 1])
                {
                    posicionSiguiente = posicionesCubos[i];
                    encontrada = true;
                    break;
                }
            }
        }
        
        if (encontrada)
        {
            visualizador.ActualizarPosicionMarcador(posicionSiguiente);
        }
    }
    
    public bool IntentarColocarCubo(Transform jugador)
    {
        if (posicionesCubos.Count == 0 || puenteCompleto)
            return false;
            
        int indiceSiguienteCubo = -1;
        
        for (int i = 0; i < cuboColocado.Count; i++)
        {
            if (!cuboColocado[i])
            {
                if (i == 0 || cuboColocado[i - 1])
                {
                    indiceSiguienteCubo = i;
                    break;
                }
            }
        }
        
        if (indiceSiguienteCubo == -1)
            return false; // No hay posiciones disponibles o válidas para colocar
            
        Vector3 posicion = posicionesCubos[indiceSiguienteCubo];
        GameObject nuevoCubo = Instantiate(cuboPuentePrefab, posicion, Quaternion.identity);
        
        nuevoCubo.transform.localScale = new Vector3(tamañoCubo, tamañoCubo / 2, tamañoCubo);
        
        Vector3 direccion;
        
        if (indiceSiguienteCubo < posicionesCubos.Count - 1)
        {
            direccion = (posicionesCubos[indiceSiguienteCubo + 1] - posicion);
        }
        else if (indiceSiguienteCubo > 0)
        {
            direccion = (posicion - posicionesCubos[indiceSiguienteCubo - 1]);
        }
        else
        {
            direccion = (terreno2.position - terreno1.position);
        }
        
        direccion.y = 0;
        
        if (direccion.magnitude > 0.01f)
        {
            nuevoCubo.transform.rotation = Quaternion.LookRotation(direccion.normalized);
        }
        
        if (nuevoCubo.GetComponent<BridgeCube>() == null)
        {
            nuevoCubo.AddComponent<BridgeCube>();
        }
        
        cuboColocado[indiceSiguienteCubo] = true;
        
        ActualizarVisualizador();
        
        if (PuenteCompleto() && !puenteCompleto)
        {
            puenteCompleto = true;
            OnPuenteCompletado();
        }
        
        return true;
    }
    
    private void OnPuenteCompletado()
    {
        Debug.Log($"Puente {indicePuente} completado");
        
        if (notificarAlManager && BridgeManager.Instance != null)
        {
            BridgeManager.Instance.RegistrarPuenteCompletado(indicePuente);
        }
    }
    
    public bool PuedeColocarCubo()
    {
        if (puenteCompleto) return false;
        
        for (int i = 0; i < cuboColocado.Count; i++)
        {
            if (!cuboColocado[i])
            {
                if (i == 0 || cuboColocado[i - 1])
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    public bool PuenteCompleto()
    {
        foreach (bool colocado in cuboColocado)
        {
            if (!colocado)
                return false;
        }
        return true;
    }
    
    public float ObtenerProgreso()
    {
        int cubosColocados = 0;
        foreach (bool colocado in cuboColocado)
        {
            if (colocado)
                cubosColocados++;
        }
        
        return (float)cubosColocados / totalCubosNecesarios;
    }
    
    public bool ObtenerPosicionSiguienteCubo(out Vector3 posicion)
    {
        posicion = Vector3.zero;
        
        if (puenteCompleto) return false;
        
        for (int i = 0; i < cuboColocado.Count; i++)
        {
            if (!cuboColocado[i])
            {
                if (i == 0 || cuboColocado[i - 1])
                {
                    posicion = posicionesCubos[i];
                    return true;
                }
            }
        }
        
        return false;
    }
} 