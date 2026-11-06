using UnityEngine;

/// <summary>
/// Controls enemy drone AI logic and player tracking without interfering with death collisions.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float detectionRange = 20f;

    [Header("Cleanup Bounds")]
    [SerializeField] private float destroyXOffset = 25f;

    private Transform playerTransform;
    private Transform mainCameraTransform;

    private void Start()
    {
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= detectionRange)
            {
                // Move towards the player 
                Vector3 direction = (playerTransform.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;

                // Locked base orientation
                transform.rotation = Quaternion.Euler(180f, 90f, 270f);
            }
            else
            {
                // Default leftward drift when player is out of range, holding steady
                transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
                transform.rotation = Quaternion.Euler(180f, 90f, 270f);
            }
        }

        if (mainCameraTransform != null && transform.position.x < mainCameraTransform.position.x - destroyXOffset)
        {
            Destroy(gameObject);
        }
    }
}