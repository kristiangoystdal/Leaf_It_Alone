using UnityEngine;

public class IdentifyLeafes : MonoBehaviour
{
    public float maxDistance = 10f; // How far from camera to check
    public float areaRadius = 1f; // Radius of detection zone
    public LayerMask leafLayer; // Assign this to "Leafes" in Inspector
    public Camera playerCamera;

    public void DeleteLeavesInCenterArea()
    {
        // Get a world point in the center of view
        Vector3 centerPoint = playerCamera.ViewportToWorldPoint(
            new Vector3(0.5f, 0.5f, maxDistance / 2)
        );

        // OverlapSphere requires colliders
        Collider[] hits = Physics.OverlapSphere(centerPoint, areaRadius, leafLayer);

        foreach (Collider hit in hits)
        {
            Destroy(hit.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        if (playerCamera == null)
            return;
        Vector3 centerPoint = playerCamera.ViewportToWorldPoint(
            new Vector3(0.5f, 0.5f, maxDistance / 2)
        );
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerPoint, areaRadius);
    }
}
