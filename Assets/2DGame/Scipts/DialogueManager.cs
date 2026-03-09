using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    
    [SerializeField] private GameObject dialoguePanel; // The UI panel for dialogue
    [SerializeField] private TextMeshProUGUI dialogueText; // The UI text element for dialogue
    [SerializeField] private float characterDelay = 0.05f; // Delay between each character appearing
    [SerializeField] private float displayDuration = 2f; // Time to display the dialogue before hiding

    
    private Coroutine typingCoroutine;
    private Coroutine hideCoroutine;

    /// <summary>
    /// Displays a dialogue message with a typewriter effect.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public void ShowDialogue(string message)
    {
        // Ensure the dialogue panel is active
        dialoguePanel.SetActive(true);

        // Stop any ongoing typing coroutine
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Stop any ongoing hide coroutine
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        // Start the typewriter effect
        typingCoroutine = StartCoroutine(TypeText(message));

        // Start the hide coroutine to hide the dialogue after the display duration
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    /// <summary>
    /// Hides the dialogue panel.
    /// </summary>
    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);

        // Stop the typing coroutine if it's running
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Stop the hide coroutine if it's running
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
    }

    /// <summary>
    /// Coroutine to display text one character at a time.
    /// </summary>
    /// <param name="message">The message to display.</param>
    private System.Collections.IEnumerator TypeText(string message)
    {
        dialogueText.text = ""; // Clear the text

        foreach (char c in message)
        {
            dialogueText.text += c; // Add one character at a time
            yield return new WaitForSeconds(characterDelay); // Wait before adding the next character
        }
    }

    /// <summary>
    /// Coroutine to hide the dialogue panel after a delay.
    /// </summary>
    private System.Collections.IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        HideDialogue();
    }
}
