using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour
{
    [Header("Wheels")]
    public WheelCollider frontLeftCol;
    public WheelCollider frontRightCol;
    public WheelCollider rearLeftCol;
    public WheelCollider rearRightCol;

    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    [Header("AI Settings")]
    public Transform[] waypoints;  // Normal driving
    public float driveSpeed = 8f;  // 👈 Adjustable in Inspector

    private NavMeshAgent agent;
    private Transform target;
    private bool isAtShop = false;

    void Start()
    {
        Debug.Log("CarAI started");
        agent = GetComponent<NavMeshAgent>();

        // Apply custom speed
        agent.speed = driveSpeed;
        agent.acceleration = 10f;  // You can tweak this too
        agent.angularSpeed = 120f; // Turn speed

        GoToRandomWaypoint();
    }

    void Update()
    {
        UpdateWheelVisuals(frontLeftCol, frontLeftMesh);
        UpdateWheelVisuals(frontRightCol, frontRightMesh);
        UpdateWheelVisuals(rearLeftCol, rearLeftMesh);
        UpdateWheelVisuals(rearRightCol, rearRightMesh);

        if (isAtShop && agent.remainingDistance <= 2f)
        {
            agent.isStopped = true; // Stop at shop
        }
    }

    void UpdateWheelVisuals(WheelCollider col, Transform mesh)
    {
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);

        // 1. Set the position from the WheelCollider
        mesh.position = pos;

        // 2. Adjust the rotation
        // The rot from the WheelCollider is in world space.
        // We multiply it by an offset rotation to correct the mesh's initial orientation
        // and potentially reverse the rotation direction if needed.

        // Correct initial orientation: Rotate 90 degrees around the local X-axis.
        // Use -90f or 90f depending on your model's import orientation.
        Quaternion meshRotationOffset = Quaternion.Euler(0f, 0f, 0f);

        // If the rotation is currently on the X-axis (tilting left/right) and you want it on Z (rolling)
        // you likely need a 90-degree rotation on the mesh's local X axis.
        // Let's assume your mesh needs a 90-degree turn around its **local X-axis**.
        // The final rotation should be the WheelCollider's world rotation multiplied by the offset.

        // Try this offset first: Rotates the mesh 90 degrees around its local X-axis to align the rolling axis with Z.
        // If the wheel rolls backward, change the 90f to -90f or add a 180-degree Z rotation.
        Quaternion initialAlignment = Quaternion.Euler(0f, 0f, 0f);

        // Assuming the visual mesh needs to be rotated 90 degrees around its **local X-axis**
        // to align its forward rolling with the WheelCollider's rotation.
        // And possibly a 180-degree rotation around the Z-axis to correct the direction.
        initialAlignment = Quaternion.Euler(90f, 0f, 0f); // Corrects the initial "flat" orientation

        // If the wheels are rotating in the wrong direction (decreasing Z rotation when moving forward):
        // Add a 180-degree rotation around the Z-axis to flip the rolling direction.
        Quaternion flipDirection = Quaternion.Euler(0f, 0f, 180f);

        // Apply the WheelCollider's world rotation, then the initial mesh alignment, and finally the direction flip
        mesh.rotation = rot * initialAlignment * flipDirection;
    }

    public void GoToRandomWaypoint()
    {
        if (waypoints.Length == 0) return;
        target = waypoints[Random.Range(0, waypoints.Length)];
        agent.SetDestination(target.position);
        agent.isStopped = false;
    }

    public void GoToShop(Transform shopPoint)
    {
        target = shopPoint;
        agent.SetDestination(shopPoint.position);
        isAtShop = true;
        agent.isStopped = false;
    }

    public void LeaveShop()
    {
        isAtShop = false;
        GoToRandomWaypoint();
    }
}
