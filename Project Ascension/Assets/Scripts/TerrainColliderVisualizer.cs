using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class TerrainColliderVisualizer : MonoBehaviour
{
    private Terrain terrain;
    private TerrainData terrainData;

    [Header("Visualizer Settings")]
    public Color colliderColor = Color.green;
    public float gizmoHeightOffset = 0.1f; // Slightly offset to avoid Z-fighting with terrain

    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
    }

    void OnDrawGizmos()
    {
        if (terrain == null) terrain = GetComponent<Terrain>();
        if (terrainData == null && terrain != null) terrainData = terrain.terrainData;
        if (terrainData == null) return;

        Gizmos.color = colliderColor;

        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;
        Vector3 terrainPosition = terrain.GetPosition();
        Vector3 terrainSize = terrainData.size;

        float invHeightmapWidth = 1f / heightmapWidth;
        float invHeightmapHeight = 1f / heightmapHeight;

        // Pre-compute the x and z scaling factors
        float xScale = terrainSize.x * invHeightmapWidth;
        float zScale = terrainSize.z * invHeightmapHeight;

        for (int x = 0; x < heightmapWidth - 1; x++)
        {
            for (int z = 0; z < heightmapHeight - 1; z++)
            {
                // Get the heights for the current point and the adjacent points
                float height = terrainData.GetHeight(x, z);
                float nextHeightX = terrainData.GetHeight(x + 1, z);
                float nextHeightZ = terrainData.GetHeight(x, z + 1);

                // Calculate the world positions for the terrain points
                Vector3 point1 = new Vector3(
                    x * xScale,
                    height,
                    z * zScale
                ) + terrainPosition + Vector3.up * gizmoHeightOffset;

                Vector3 point2 = new Vector3(
                    (x + 1) * xScale,
                    nextHeightX,
                    z * zScale
                ) + terrainPosition + Vector3.up * gizmoHeightOffset;

                Vector3 point3 = new Vector3(
                    x * xScale,
                    nextHeightZ,
                    (z + 1) * zScale
                ) + terrainPosition + Vector3.up * gizmoHeightOffset;

                // Draw lines between the points to form the grid
                Gizmos.DrawLine(point1, point2);
                Gizmos.DrawLine(point1, point3);
            }
        }
    }
}