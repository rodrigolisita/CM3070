using UnityEngine;
using System.Collections.Generic;

public class CFD_SpherePlot : MonoBehaviour
{
    public CFD_DataSource dataSource;
    public GameObject spherePrefab;
    public int poolSize = 10000;

    private List<GameObject> spherePool = new List<GameObject>();
    private int poolIndex = 0;

    void Start()
    {
        dataSource.OnDataLoaded += PlotSpheres;
    }

    void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject sphere = Instantiate(spherePrefab, Vector3.zero, Quaternion.identity, transform);
            sphere.SetActive(false);
            spherePool.Add(sphere);
        }
    }

    void PlotSpheres()
    {
        if (spherePool.Count == 0) CreatePool();

        poolIndex = 0;
        foreach (var point in dataSource.DataPoints)
        {
            if (poolIndex >= spherePool.Count) break;

            GameObject sphere = spherePool[poolIndex++];
            sphere.transform.position = point.position;
            sphere.SetActive(true);
        }

        // Desativa o restante da pool
        for (int i = poolIndex; i < spherePool.Count; i++)
        {
            spherePool[i].SetActive(false);
        }

        Debug.Log($"✅ Instanciadas {poolIndex} esferas na cena.");
    }
}