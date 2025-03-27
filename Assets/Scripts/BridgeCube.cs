using UnityEngine;

public class BridgeCube : MonoBehaviour
{
    [SerializeField] private float duracionAnimacion = 0.5f;
    [SerializeField] private float alturaSalto = 0.5f;
    
    private Vector3 escalaNormal;
    private bool animando = false;
    
    void Start()
    {
        escalaNormal = transform.localScale;
        
        transform.localScale = Vector3.zero;
        
        AnimarAparicion();
    }
    
    private void AnimarAparicion()
    {
        if (animando) return;
        
        StartCoroutine(AnimarEscala(Vector3.zero, escalaNormal, duracionAnimacion));
    }
    
    private System.Collections.IEnumerator AnimarEscala(Vector3 escalaInicial, Vector3 escalaFinal, float duracion)
    {
        animando = true;
        float tiempoInicio = Time.time;
        
        while (Time.time < tiempoInicio + duracion)
        {
            float t = (Time.time - tiempoInicio) / duracion;
            
            t = Mathf.SmoothStep(0, 1, t);
            
            transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t);
            
            float factorSalto = Mathf.Sin(t * Mathf.PI) * alturaSalto;
            transform.position = new Vector3(
                transform.position.x, 
                transform.position.y + factorSalto * Time.deltaTime, 
                transform.position.z
            );
            
            yield return null;
        }
        
        transform.localScale = escalaFinal;
        animando = false;
    }
} 