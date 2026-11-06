using UnityEngine;

/// <summary>
/// Controls collectible scrap movement, floating animation, audio, and collection logic.
/// </summary>
public class Collectible : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField] private float rotateSpeed = 90f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatAmplitude = 0.3f;

    [Header("Score Value")]
    [SerializeField] private int scoreValue = 1;

    [Header("Cleanup Bounds")]
    [SerializeField] private float destroyXOffset = 25f;

    private Vector3 startPos;
    private Transform mainCameraTransform;
    private bool isCollected = false; // Prevents double collection

    private void Start()
    {
        startPos = transform.position;

        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        // Continuous 3D rotation
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

        // Gentle vertical floating animation
        float newY = startPos.y + (Mathf.Sin(Time.time * floatSpeed) * floatAmplitude);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (mainCameraTransform != null && transform.position.x < mainCameraTransform.position.x - destroyXOffset)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Guard clause: stop if already collected or not player
        if (isCollected || !other.CompareTag("Player")) return;

        isCollected = true;
        Debug.Log("Scrap collected by Player!");

        // Play Scrap Pickup SFX via AudioManager
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScrapPickup();
        }

        // Award points
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        Destroy(gameObject);
    }
}