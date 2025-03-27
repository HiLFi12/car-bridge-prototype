using System.Collections.Generic;
using UnityEngine;

public class BridgeManager : MonoBehaviour
{
    public static BridgeManager Instance;

    [Header("Referencias")]
    [SerializeField] private List<BridgeBuilder> puentes = new List<BridgeBuilder>();
    
    private HashSet<int> puentesCompletados = new HashSet<int>();

    private HashSet<int> placedOrders = new HashSet<int>(); 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (puentes.Count == 0)
        {
            BridgeBuilder[] buildersEnEscena = FindObjectsOfType<BridgeBuilder>();
            puentes.AddRange(buildersEnEscena);
            
            Debug.Log($"Se encontraron {puentes.Count} constructores de puentes en la escena");
        }
        
    }

    public void RegisterPlacedOrder(int order)
    {
        placedOrders.Add(order);
    }

    public bool IsPreviousOrderPlaced(int order)
    {
        return order == 1 || placedOrders.Contains(order - 1);
    }
    
    public void RegistrarPuenteCompletado(int indicePuente)
    {
        if (indicePuente >= 0 && indicePuente < puentes.Count)
        {
            puentesCompletados.Add(indicePuente);
            
            if (puentesCompletados.Count == puentes.Count)
            {
                OnTodosPuentesCompletados();
            }
        }
    }
    
    public bool TodosPuentesCompletados()
    {
        return puentesCompletados.Count == puentes.Count && puentes.Count > 0;
    }
    
    private void OnTodosPuentesCompletados()
    {
        Debug.Log("¡Todos los puentes han sido completados!");

    }
}