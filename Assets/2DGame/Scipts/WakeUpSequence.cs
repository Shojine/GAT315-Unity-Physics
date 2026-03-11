using System.Collections;
using UnityEngine;

/// <summary>
/// Plays a sequence of dialogue lines when the scene starts, simulating the player waking up.
/// Attach to any GameObject in the opening scene. Disable player input during the sequence.
/// </summary>
public class WakeUpSequence : MonoBehaviour
{
    [SerializeField] private string[] lines = new string[]
    {
        "...",
        "Waking up...",
        "I need to get out of here."
    };

    [SerializeField] private string skipIfHasKey; // Skip the sequence if the player already has this key
    [SerializeField] private AudioClip crashSound; // Played before the second line
    [SerializeField] private float delayBetweenLines = 0.5f; // Gap between one line finishing and the next starting
    [SerializeField] private float initialDelay = 1f;        // Pause before the first line

    private DialogueManager dialogueManager;
    private AudioSource audioSource;
    private InputListener[] playerInputListeners;

    private void Start()
    {
        dialogueManager = FindFirstObjectByType<DialogueManager>();
        audioSource = GetComponent<AudioSource>();

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerInputListeners = player.GetComponents<InputListener>();

        bool skip = !string.IsNullOrEmpty(skipIfHasKey) && InventoryManager.Instance.HasKey(skipIfHasKey);
        if (dialogueManager != null && !skip)
            StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        SetPlayerInput(false);

        yield return new WaitForSeconds(initialDelay);

        for (int i = 0; i < lines.Length; i++)
        {
            // Play crash sound before the second line (index 1)
            if (i == 1 && crashSound != null && audioSource != null)
                audioSource.PlayOneShot(crashSound);

            dialogueManager.ShowDialogue(lines[i]);

            // Wait for typing + display duration + gap before next line
            float lineLength = lines[i].Length * dialogueManager.CharacterDelay;
            float totalWait = lineLength + dialogueManager.DisplayDuration + delayBetweenLines;
            yield return new WaitForSeconds(totalWait);
        }

        SetPlayerInput(true);
    }

    private void SetPlayerInput(bool enabled)
    {
        if (playerInputListeners == null) return;
        foreach (var l in playerInputListeners) l.enabled = enabled;
    }
}
