using UnityEngine;

public class CarrotPlacement : MonoBehaviour
{
    public GameObject carrotPrefab;
    public Terrain terrain;
    public int numberOfCarrots = 50;
    public float placementRadius = 0.2f;
    public int maxAttemptsCollide = 5;
    public int maxAttemptsFlat = 300;
    public float maxSlopeAngle = 5f;
    public LayerMask foliageLayerMask;
    public float buryDepth = 1.2f;

    void Start()
    {
        int placed = 0;
        int iteration = 0;

        int highestAttemptsCollide = 1;
        int highestAttemptsFlat = 1;


        while (iteration < numberOfCarrots)
        {
            iteration++;
            int attemptsFlat = 0;
            Vector3 randomPosition = new Vector3(0, 0, 0);
            while (attemptsFlat < maxAttemptsFlat)
            {
                attemptsFlat++;
                randomPosition = GetRandomPointOnTerrain();

                if (IsSurfaceFlat(randomPosition))
                {
                    if (attemptsFlat > highestAttemptsFlat)
                        highestAttemptsFlat = attemptsFlat;
                    break;
                }
            }
            if (attemptsFlat == maxAttemptsFlat && attemptsFlat > highestAttemptsFlat)
            {
                highestAttemptsFlat = attemptsFlat;
                continue;
            }
            int attemptsCollide = 0;
            while (attemptsCollide < maxAttemptsCollide)
            {
                attemptsCollide++;
                if (Physics.OverlapSphere(randomPosition, placementRadius, foliageLayerMask).Length == 0)
                {
                    if (attemptsCollide > highestAttemptsCollide)
                        highestAttemptsCollide = attemptsCollide;
                    placed++;
                    break;
                }
            }
            if (attemptsCollide == maxAttemptsCollide && attemptsCollide > highestAttemptsCollide)
                highestAttemptsCollide = attemptsCollide;


            Instantiate(carrotPrefab, randomPosition, Quaternion.Euler(-90, Random.Range(0, 360), 0));
        }

        Debug.Log($"Placed {placed} carrots. Max retry due to flatness: {highestAttemptsFlat}/{maxAttemptsFlat}; due to collision: {highestAttemptsCollide}/{maxAttemptsCollide}.");
    }

    Vector3 GetRandomPointOnTerrain()
    {
        float x = Random.Range(terrain.transform.position.x, terrain.terrainData.size.x + terrain.transform.position.x);
        float z = Random.Range(terrain.transform.position.z, terrain.terrainData.size.z + terrain.transform.position.z);
        float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrain.transform.position.y - buryDepth;
        return new Vector3(x, y, z);
    }

    bool IsSurfaceFlat(Vector3 position)
    {
        Vector3 terrainLocalPos = position - terrain.transform.position;
        Vector3 normal = terrain.terrainData.GetInterpolatedNormal(
            terrainLocalPos.x / terrain.terrainData.size.x,
            terrainLocalPos.z / terrain.terrainData.size.z
        );

        float angle = Vector3.Angle(normal, Vector3.up);
        return angle <= maxSlopeAngle;
    }

}
