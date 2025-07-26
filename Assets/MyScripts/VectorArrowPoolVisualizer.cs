using UnityEngine;
using System.Collections.Generic;

public class VectorArrowPoolVisualizer : MonoBehaviour
{

    public CFD_DataSource dataSource;
    public GameObject vectorArrowPrefab;
    public Gradient velocityGradient;
    public int poolSize = 10000;
    public float vectorLengthMultiplier = 0.5f;
    public Vector3 uniformScale = new Vector3(0.01f, 0.01f, 1f);

    private List<GameObject> pool = new List<GameObject>();
    private int poolIndex = 0;

    void Start() => dataSource.OnDataLoaded += VisualizeVectors;

    void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = Instantiate(vectorArrowPrefab, Vector3.zero, Quaternion.identity, transform);
            arrow.SetActive(false);
            pool.Add(arrow);
        }
    }

    void VisualizeVectors()
    {
        Debug.Log("VisualizeVectors disparado!");
        Debug.Log($"Total de pontos carregados: {dataSource.DataPoints.Count}");

        if (pool.Count == 0) CreatePool();

        poolIndex = 0;
        float minV = dataSource.MinVelocity;
        float maxV = dataSource.MaxVelocity;
        float vRange = Mathf.Max(maxV - minV, 0.001f);

        foreach (var point in dataSource.DataPoints)
        {
            if (poolIndex >= pool.Count) break;

            GameObject arrow = pool[poolIndex++];
            Vector3 velocity = point.velocityVector;
            float magnitude = velocity.magnitude;

            if (velocity.magnitude < 1e-6f)
            {
                Debug.LogWarning($"Vetor nulo em {point.position}");
                continue;
            }


            arrow.transform.position = point.position;
            arrow.transform.rotation = Quaternion.LookRotation(velocity);
            arrow.transform.localScale = new Vector3(uniformScale.x, uniformScale.y, magnitude * vectorLengthMultiplier);

            //var renderer = arrow.GetComponent<Renderer>();
            Renderer renderer = arrow.GetComponent<Renderer>();
            if (renderer == null) renderer = arrow.GetComponentInChildren<Renderer>();

            if (renderer != null)
            {
                Color color = velocityGradient.Evaluate((magnitude - minV) / vRange);
                renderer.material.color = color;
            }

            arrow.SetActive(true);
        }

        // Desativa o restante dos objetos da pool
        for (int i = poolIndex; i < pool.Count; i++)
        {
            pool[i].SetActive(false);
        }
    }
}
