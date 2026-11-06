using UnityEngine;

/// <summary>
/// Scales difficulty over time, ramping from 1x up to a maximum of 5x over a 60-second period.
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    [Header("Scaling Settings")]
    // Ramps multiplier from 1x up to 5x across 60 seconds
    // (0.0667 * 60 = 4.0 increase, totaling 5.0x at 60s)
    [SerializeField] private float difficultyIncreaseRate = 0.0667f;

    public float CurrentDifficultyMultiplier { get; private set; } = 1f;

    private void Update()
    {
        // Stop scaling if the game is over or paused
        if (GameManager.Instance != null && (GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused)) return;

        CurrentDifficultyMultiplier += difficultyIncreaseRate * Time.deltaTime;
        CurrentDifficultyMultiplier = Mathf.Min(CurrentDifficultyMultiplier, 5f); // Hard cap at 5x max difficulty
    }
}