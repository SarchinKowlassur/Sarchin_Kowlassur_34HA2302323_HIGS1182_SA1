using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns power-up collectibles periodically ahead of the player in the side-scrolling window.
/// </summary>
public class PowerUpSpawner : MonoBehaviour
{
    [Header("Prefab & Timing")]
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private float minSpawnInterval = 20f; // Minimum time between power-up spawns
    [SerializeField] private float maxSpawnInterval = 40f; // Maximum time between power-up spawns
    [SerializeField] private float initialDelay = 15f;     // Delay before the first power-up can appear

    [Header("Spawn Bounds")]
    [SerializeField] private float spawnXDistance = 25f;
    [SerializeField] private Vector2 spawnYRange = new Vector2(-4f, 4f);

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            // Pick a random interval between min and max for varied spacing
            float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(randomInterval);

            if (GameManager.Instance == null || !GameManager.Instance.IsGameOver)
            {
                SpawnPowerUp();
            }
        }
    }

    private void SpawnPowerUp()
    {
        if (powerUpPrefab == null) return;

        float spawnX = mainCamera != null ? mainCamera.transform.position.x + spawnXDistance : transform.position.x + spawnXDistance;
        float spawnY = Random.Range(spawnYRange.x, spawnYRange.y);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

        Instantiate(powerUpPrefab, spawnPos, Quaternion.identity);
        Debug.Log("Power-Up Spawned at: " + spawnPos);
    }
}