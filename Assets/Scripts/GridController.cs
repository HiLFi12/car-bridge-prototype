using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public GameObject Prefab;

    public int x_ColumnLenght;
    public int z_RowLenght;

    public float x_SpaceBetweenPrefab;
    public float z_SpaceBetweenPrefab;


    void Start()
    {
        for (int i = 0; i < x_ColumnLenght*z_RowLenght; i++)
        {
            Instantiate(Prefab, new Vector3(x_ColumnLenght + (x_SpaceBetweenPrefab * (i % x_ColumnLenght)), 0, z_SpaceBetweenPrefab + (z_SpaceBetweenPrefab * (i / x_ColumnLenght))), Quaternion.identity);
        }
    }
}
