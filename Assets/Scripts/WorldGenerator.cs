using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("Tile Settings")]
    public GameObject[] groundPrefabs; // Assign Ground_01, etc.
    public int width = 5;
    public int height = 5;
    public float tileSize = 1f;

    private HashSet<Vector2Int> tilePositions = new HashSet<Vector2Int>();
    private List<Vector3> edgePositions = new List<Vector3>();

    void Start()
    {
        GenerateGround();
    }

    void GenerateGround()
    {
        if (groundPrefabs == null || groundPrefabs.Length == 0)
        {
            Debug.LogError("Ground prefabs not assigned.");
            return;
        }

        for (int x = -width; x < width; x++)
        {
            for (int z = -height; z < height; z++)
            {
                Vector2Int gridPos = new Vector2Int(x, z);
                tilePositions.Add(gridPos);

                // Weighted random selection
                GameObject tilePrefab;
                float randomValue = Random.value;
                if (randomValue < 0.5f || x == width - 1 || z == height - 1)
                    tilePrefab = groundPrefabs[0];
                else if (randomValue < 0.8f)
                    tilePrefab = groundPrefabs[2];
                else
                    tilePrefab = groundPrefabs[1];

                Quaternion rotation = Quaternion.Euler(0, 90 * Random.Range(0, 4), 0);
                GameObject tile = Instantiate(tilePrefab, Vector3.zero, rotation, this.transform);

                // Position with ground alignment
                MeshFilter mf = tile.GetComponentInChildren<MeshFilter>();
                if (mf == null || mf.sharedMesh == null)
                {
                    Debug.LogWarning("No MeshFilter or mesh found on tile.");
                    continue;
                }

                Bounds bounds = mf.sharedMesh.bounds;
                float yOffset = bounds.min.y;
                Vector3 position = new Vector3(x * tileSize, -yOffset, z * tileSize);
                tile.transform.position = position;

                DipMeshEdges(mf);
            }
        }
    }

    void DipMeshEdges(MeshFilter mf)
    {
        // This is a writable instance copy — safe to edit
        Mesh mesh = mf.mesh;

        Vector3[] verts = mesh.vertices;
        Bounds bounds = mesh.bounds;
        float dipAmount = 0.02f;

        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 v = verts[i];

            bool isEdge =
                Mathf.Approximately(v.x, bounds.min.x)
                || Mathf.Approximately(v.x, bounds.max.x)
                || Mathf.Approximately(v.z, bounds.min.z)
                || Mathf.Approximately(v.z, bounds.max.z);

            if (isEdge)
            {
                // Force any edge vertex to be <= 0, then dip slightly
                v.y = Mathf.Min(v.y, 0f) - dipAmount;
                verts[i] = v;
            }
        }

        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mf.mesh = mesh;
    }
}
