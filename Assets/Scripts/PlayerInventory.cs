using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Transform mano; 
    [SerializeField] private Transform spawnPuente; 
    [SerializeField] private Transform destinoPuente; 
    [SerializeField] private GameObject cuboPuentePrefab;

    [Header("Materiales")]
    [SerializeField] private Material materialNormal; 
    [SerializeField] private Material materialActivo; 

    private GameObject cuboEnMano;
    private Renderer rendererCubo;
    private bool enBorde = false; 


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
            if (enBorde && materialActivo != null)
            {
                rendererCubo.material = materialActivo;
            }
            else if (!enBorde && materialNormal != null)
            {
                rendererCubo.material = materialNormal;
            }
        }

        if (TieneCubo())
        {

            if (enBorde && Input.GetKeyDown(KeyCode.E))
            {
                ColocarPuente();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                SoltarCubo();
            }
        }
    }

    private void ColocarPuente()
    {
        if (spawnPuente == null || destinoPuente == null || cuboPuentePrefab == null)
        {
            Debug.LogWarning("Faltan referencias para crear el puente");
            return;
        }

        Vector3 puntoA = spawnPuente.position;
        Vector3 puntoB = destinoPuente.position;
        Vector3 centro = (puntoA + puntoB) / 2;

        Vector3 direccion = puntoB - puntoA;
        float distancia = direccion.magnitude;

        GameObject puente = Instantiate(cuboPuentePrefab, centro, Quaternion.identity);

       
        puente.transform.localScale = new Vector3(
            puente.transform.localScale.x * 2,
            puente.transform.localScale.y,
            distancia
        );

        puente.transform.rotation = Quaternion.LookRotation(direccion.normalized);

        if (cuboEnMano != null)
        {
            Destroy(cuboEnMano);
            cuboEnMano = null;
            rendererCubo = null;
        }
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

    private bool PuedeConstruirPuente()
    {
        return TieneCubo() && enBorde && spawnPuente != null && destinoPuente != null;
    }
}