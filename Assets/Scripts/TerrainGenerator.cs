#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class TerrainGenerator : EditorWindow
{
    Terrain terrain;

    [MenuItem("Tools/Generate Perlin Terrain")]
    public static void ShowWindow()
    {
        GetWindow<TerrainGenerator>("Perlin Terrain Generator");
    }

    void OnGUI()
    {
        terrain = (Terrain)EditorGUILayout.ObjectField("Terrain", terrain, typeof(Terrain), true);

        if (GUILayout.Button("Generate"))
        {
            if (terrain != null)
            {
                GenerateTerrain();
            }
        }
    }

    void GenerateTerrain()
    {
        int width = terrain.terrainData.heightmapResolution;
        int height = terrain.terrainData.heightmapResolution;

        float[,] heights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = (Mathf.PerlinNoise(x * 0.02f, y * 0.02f) - 0.5f) * 0.1f;

            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
    }
}
#endif