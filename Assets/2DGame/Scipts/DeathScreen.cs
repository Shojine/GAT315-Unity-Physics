using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Shows a death screen overlay and handles returning to the main menu.
/// Attach to a persistent GameObject in the scene.
/// Wire Health.onDeath -> DeathScreen.ShowDeathScreen()
/// </summary>
public class DeathScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject deathPanel;       // The full-screen death UI panel
    [SerializeField] CanvasGroup canvasGroup;     // For fade-in effect
    [SerializeField] float fadeInDuration = 1f;   // How long the fade takes

    [Header("Scene")]
    [SerializeField] string mainMenuScene = "MainMenu"; // Name of the main menu scene

    void Awake()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    /// <summary>
    /// Call this when the player dies. Wire to Health.onDeath in the Inspector.
    /// </summary>
    public void ShowDeathScreen()
    {
        if (deathPanel == null) return;

        deathPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game

        if (canvasGroup != null)
            StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaled so pause doesn't block it
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Loads the main menu scene. Wire to the "Main Menu" button's OnClick.
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Restore time before loading
        SceneManager.LoadScene(mainMenuScene);
    }

    /// <summary>
    /// Reloads the current scene. Wire to the "Retry" button's OnClick.
    /// </summary>
    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
