using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls Main Menu interactions, scene transitions, audio, and application exit.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string gameplaySceneName = "GameplayScene";

    private void Start()
    {
        // Ensure game time is running normally
        Time.timeScale = 1f;

        // Play menu music when entering the Main Menu scene
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMenuMusic();
        }
    }

    // Loads the main gameplay scene. Hook this to the START button.
    public void StartGame()
    {
        Debug.Log("Loading Gameplay Scene: " + gameplaySceneName);
        SceneManager.LoadScene(gameplaySceneName);
    }

    // Quits the application. Hook this to the QUIT button.
    public void QuitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}