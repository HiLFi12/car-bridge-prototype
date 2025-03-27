using UnityEngine;

public class BridgeGrid : MonoBehaviour
{
    public GameObject cubePrefab; 
    public float spacing = 1.1f; 
    public Vector3 startPosition = new Vector3(0, 2, 0); 
    public int bridgeLength = 4; 

    void Start()
    {
        for (int i = 0; i < bridgeLength; i++)
        {
            Vector3 position = startPosition + new Vector3(i * spacing, 0, 0); 
            Instantiate(cubePrefab, position, Quaternion.identity, transform);
        }
    }
}
