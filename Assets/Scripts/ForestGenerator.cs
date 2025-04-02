using UnityEngine;

public class ForestGenerator : MonoBehaviour
{
    [Header("Tree Prefabs")]
    public GameObject[] treePrefabs;

    [Header("Border Settings")]
    public int borderThickness = 10; // Total border rows (first 5 noise, next 5 solid)
    public int noiseRows = 5; // How many rows use Perlin noise
    public float treeSpacing = 1.5f; // Tree spacing
    public float treeDensity = 0.6f; // For noise zone only
    public float noiseScale = 0.2f;
    public float jitterAmount = 0.3f; // Small offset for filled rows
    public int seed = 0;

    void Start()
    {
        GenerateForestBorder();
    }

    void GenerateForestBorder()
    {
        WorldGenerator worldGen = GetComponent<WorldGenerator>();
        if (worldGen == null)
        {
            Debug.LogError("WorldGenerator component not found.");
            return;
        }

        if (seed == 0)
            seed = Random.Range(1, 100000);

        int width = worldGen.width;
        int height = worldGen.height;
        float tileSize = worldGen.tileSize;

        float innerMinX = -width * tileSize;
        float innerMaxX = (width - 1) * tileSize;
        float innerMinZ = -height * tileSize;
        float innerMaxZ = (height - 1) * tileSize;

        float outerMinX = innerMinX - borderThickness * tileSize;
        float outerMaxX = innerMaxX + borderThickness * tileSize;
        float outerMinZ = innerMinZ - borderThickness * tileSize;
        float outerMaxZ = innerMaxZ + borderThickness * tileSize;

        for (float x = outerMinX; x <= outerMaxX; x += treeSpacing)
        {
            for (float z = outerMinZ; z <= outerMaxZ; z += treeSpacing)
            {
                // Skip if inside playable area
                bool insidePlayableX = x >= innerMinX && x <= innerMaxX;
                bool insidePlayableZ = z >= innerMinZ && z <= innerMaxZ;
                if (insidePlayableX && insidePlayableZ)
                    continue;

                float distToPlayableX = Mathf.Min(
                    Mathf.Abs(x - innerMinX),
                    Mathf.Abs(x - innerMaxX)
                );
                float distToPlayableZ = Mathf.Min(
                    Mathf.Abs(z - innerMinZ),
                    Mathf.Abs(z - innerMaxZ)
                );
                float distToBoard = Mathf.Min(distToPlayableX, distToPlayableZ);

                // Determine zone: noise zone or filled zone
                if (distToBoard > noiseRows * tileSize)
                {
                    // Zone 1: noise-based
                    float nx = (x + seed) * noiseScale;
                    float nz = (z + seed) * noiseScale;
                    float noiseValue = Mathf.PerlinNoise(nx, nz);

                    if (noiseValue < treeDensity)
                    {
                        SpawnTree(new Vector3(x, 0f, z));
                    }
                }
                else
                {
                    // Zone 2: filled with jitter
                    float jitterX = Random.Range(-jitterAmount, jitterAmount);
                    float jitterZ = Random.Range(-jitterAmount, jitterAmount);
                    SpawnTree(new Vector3(x + jitterX, 0f, z + jitterZ));
                }
            }
        }
    }

    void SpawnTree(Vector3 position)
    {
        if (treePrefabs.Length == 0)
            return;

        GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        GameObject tree = Instantiate(prefab, position, rotation, this.transform);
        tree.transform.localScale *= Random.Range(0.9f, 1.1f);
    }
}
