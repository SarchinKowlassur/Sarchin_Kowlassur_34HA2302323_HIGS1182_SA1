using UnityEngine;

/// <summary>
/// Collectible power-up that grants the player multi-projectile firing for a limited duration.
/// Drifts across the screen like other collectibles and cleans up when off-screen.
/// </summary>
public class PowerUp : MonoBehaviour
{
    [Header("Power-Up Settings")]
    [SerializeField] private float powerUpDuration = 5f;
    [SerializeField] private float moveSpeed = 5f; // Matches side-scrolling world movement
    [SerializeField] private float destroyXOffset = 25f;

    private Transform mainCameraTransform;

    private void Start()
    {
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        // Drift leftward alongside the background/world movement
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);

        // Auto-cleanup when off-screen to the left
        if (mainCameraTransform != null && transform.position.x < mainCameraTransform.position.x - destroyXOffset)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player collected the power-up
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ActivateSpreadShot(powerUpDuration);
            }

            Debug.Log("Power-Up Collected: Multi-Projectile Activated!");

            // Destroy the power-up pickup item from the scene
            Destroy(gameObject);
        }
    }
}