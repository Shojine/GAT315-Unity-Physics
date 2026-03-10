using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    
    [SerializeField] private GameObject dialoguePanel; // The UI panel for dialogue
    [SerializeField] private TextMeshProUGUI dialogueText;
    public TextMeshProUGUI DialogueText => dialogueText; // The UI text element for dialogue
    [SerializeField] private float characterDelay = 0.05f; // Delay between each character appearing
    [SerializeField] private float displayDuration = 2f; // Time to display the dialogue before hiding

    
    [SerializeField] private Vector2 panelPadding = new Vector2(24f, 16f); // Extra space around the text

    public float CharacterDelay => characterDelay;
    public float DisplayDuration => displayDuration;

    public float totalDisplayTime { get; private set; } = 0f; // Total time the current dialogue has been displayed (including typing time)

    private RectTransform panelRect;
    private Coroutine typingCoroutine;
    private Coroutine hideCoroutine;

    private void Awake()
    {
        panelRect = dialoguePanel.GetComponent<RectTransform>();
    }

    /// <summary>
    /// Displays a dialogue message with a typewriter effect.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public void ShowDialogue(string message)
    {
        totalDisplayTime = displayDuration; // reset for this message

        foreach (char c in message)
        {
            totalDisplayTime += (c == '.') ? characterDelay + 0.5f : characterDelay;
        }

        dialoguePanel.SetActive(true);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message, () =>
        {
            hideCoroutine = StartCoroutine(HideAfterDelay());
        }));
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
    private System.Collections.IEnumerator TypeText(string message, System.Action onComplete = null)
    {
        dialogueText.text = ""; // Clear the text

        foreach (char c in message)
        {
            dialogueText.text += c; // Add one character at a time
            ResizePanel();
            float delay = (c == '.') ? characterDelay + 0.5f : characterDelay;
            yield return new WaitForSeconds(delay);
        }

        onComplete?.Invoke();
    }

    private void ResizePanel()
    {
        if (panelRect == null) return;
        float textWidth = panelRect.sizeDelta.x - panelPadding.x;
        dialogueText.ForceMeshUpdate();
        Vector2 preferred = dialogueText.GetPreferredValues(dialogueText.text, textWidth, 0);
        panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x, preferred.y + panelPadding.y);
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
