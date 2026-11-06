using UnityEngine;

/// <summary>
/// Controls moving laser projectile behavior, impact collisions, 
/// and off-screen cleanup to prevent hitting unspawned/off-screen targets.
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 50f;
    [SerializeField] private float lifeTime = 2f;

    [Header("Screen Padding")]
    [SerializeField] private float rightBoundaryPadding = 0.05f; // Extra margin past the right screen edge before destroy

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        // Backup lifetime destroy in case it somehow misses viewport bounds
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // Move along local forward so spread-shot and angled lasers travel in their rotated direction
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        // Check if projectile has traveled off-screen
        CheckViewportBounds();
    }

    // Destroys the projectile as soon as it leaves the camera's visible viewport.
    private void CheckViewportBounds()
    {
        if (mainCamera == null) return;

        // Convert world position to Viewport coordinates (x: 0 to 1, y: 0 to 1)
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        // Destroy if it passes the right edge (+ padding), left edge, top, or bottom
        if (viewportPos.x > (1.0f + rightBoundaryPadding) || viewportPos.x < -0.1f ||
            viewportPos.y > 1.1f || viewportPos.y < -0.1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Asteroid") || other.CompareTag("Enemy"))
        {
            Debug.Log("Projectile impacted target: " + other.name);

            // Call the death sequence on the hit object (handles mesh hiding + sprite explosion spawning)
            DestructibleObject destructible = other.GetComponentInParent<DestructibleObject>();
            if (destructible != null)
            {
                destructible.TriggerDeathSequence();
            }
            else
            {
                Destroy(other.gameObject); // Fallback destroy if script is missing
            }

            Destroy(gameObject); // Destroy the laser projectile
        }
    }
}