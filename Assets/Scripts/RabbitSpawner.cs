using UnityEngine;

public class RabbitSpawner : MonoBehaviour
{
    public GameObject rabbitPrefab;
    public int rabbitCount = 10;
    public Terrain terrain;

    void Start()
    {
        for (int i = 0; i < rabbitCount; i++)
        {
            Vector3 spawnPosition = GetRandomPointOnTerrain();
            Instantiate(rabbitPrefab, spawnPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomPointOnTerrain()
    {
        float x = Random.Range(terrain.transform.position.x, terrain.terrainData.size.x + terrain.transform.position.x);
        float z = Random.Range(terrain.transform.position.z, terrain.terrainData.size.z + terrain.transform.position.z);
        float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrain.transform.position.y;
        return new Vector3(x, y, z);
    }
}
