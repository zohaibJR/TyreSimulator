using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    public Transform rayOrigin;    // Empty object in front of player
    public float rayDistance = 5f; // How far the ray goes

    private bool isEmitting = false;   // Track if ray is active
    private float emitDuration = 0.2f; // Ray stays visible for 0.2s
    private float emitTimer = 0f;

    private void Update()
    {
        // Check for input (E key)
        if (Input.GetKeyDown(KeyCode.E))
        {
            CastRay();
        }

        // Countdown timer for gizmo visibility
        if (isEmitting)
        {
            emitTimer -= Time.deltaTime;
            if (emitTimer <= 0f)
            {
                isEmitting = false;
            }
        }
    }

    private void CastRay()
    {
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            Debug.Log("Hit: " + hit.collider.name);
        }
        else
        {
            Debug.Log("No hit");
        }

        // Enable gizmo for short duration
        isEmitting = true;
        emitTimer = emitDuration;
        Debug.Log("Raycast emitted!");
    }

    // Draw green gizmo only while ray is active
    private void OnDrawGizmos()
    {
        if (isEmitting && rayOrigin != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(rayOrigin.position, rayOrigin.forward * rayDistance);
        }
    }
}
