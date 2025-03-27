using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour
{
    [Header("Configuración del Auto")]
    [SerializeField] private GameObject carPrefab;       
    [SerializeField] private float spawnInterval = 3f;   
    [SerializeField] private bool moveRight = true;      

    [Header("Punto de Spawn")]
    [SerializeField] private Transform spawnPoint;       

    void Start()
    {
        StartCoroutine(SpawnCarRoutine());
    }

    private IEnumerator SpawnCarRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnCar();
        }
    }

    private void SpawnCar()
    {
        if (carPrefab == null)
        {
            Debug.LogError("No hay prefab asignado para el auto!");
            return;
        }

        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;

        GameObject newCar = Instantiate(carPrefab, spawnPosition, Quaternion.identity);
        MoveCar moveCarScript = newCar.GetComponent<MoveCar>();

        if (moveCarScript != null)
        {
            moveCarScript.moverHaciaDerecha = moveRight; 
        }
    }
}
