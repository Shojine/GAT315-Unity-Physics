using UnityEngine;

public class CabinetInteractable : MonoBehaviour
{
    [SerializeField] private string keyID; // The ID of the key hidden in the cabinet
    [SerializeField] private string searchMessage = "You found a key!"; // Message to display when the key is found
    [SerializeField] private string emptyMessage = "The cabinet is empty."; // Message to display if the cabinet has already been searched
    [SerializeField] private AudioClip searchSound; // Sound to play when searching the cabinet
    [SerializeField] private AudioClip keySound;

    [Header("Required Tool")]
    [SerializeField] private string requiredToolName; // Player must have this tool to open the cabinet
    [SerializeField] private string lockedMessage = "I cannot open this."; // Message shown when player lacks the required tool
    [SerializeField] private AudioClip lockedSound; // Sound to play when the cabinet is locked

    [Header("Tool Pickup (optional)")]
    [SerializeField] private string toolName; // Tool added to inventory when the cabinet is searched

    private bool isPlayerInRange = false;
    private bool hasKey = false; // Whether the cabinet still contains a key or tool
    private DialogueManager dialogueManager;
    private AudioSource audioSource;
    [SerializeField] private GameObject audioPlayer;

    private void Start()
    {
        if (!string.IsNullOrEmpty(keyID) || !string.IsNullOrEmpty(toolName)) hasKey = true;
        dialogueManager = FindObjectOfType<DialogueManager>(); // Find the DialogueManager in the scene
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
    }

    public void SearchCabinet()
    {
        if (isPlayerInRange)
        {
            // Check if a required tool is needed and the player doesn't have it
            if (!string.IsNullOrEmpty(requiredToolName) && !InventoryManager.Instance.HasKey(requiredToolName))
            {
                if (dialogueManager != null)
                    dialogueManager.ShowDialogue(lockedMessage);
                audioSource.PlayOneShot(lockedSound);
                return;
            }

            Debug.Log("Player is in range. Searching cabinet...");

            Debug.Log($"Cabinet has item: {hasKey}");

            if (hasKey)
            {
                audioSource.PlayOneShot(keySound);

                // Add key or tool to inventory
                if (!string.IsNullOrEmpty(keyID))
                    InventoryManager.Instance.AddKey(keyID);

                if (!string.IsNullOrEmpty(toolName))
                    InventoryManager.Instance.AddKey(toolName);

                // Show the message for finding the key
                if (dialogueManager != null)
                {
                    dialogueManager.ShowDialogue(searchMessage);
                    Debug.Log($"Displayed message: {searchMessage}");
                }
                else
                {
                    Debug.LogError("DialogueManager is missing.");
                }

                this.hasKey = false; // Mark the cabinet as empty

                if(audioPlayer != null)
                {
                    audioPlayer.SetActive(true);
                }
            }
            else
            {
                Debug.Log("Cabinet is empty.");
                audioSource.PlayOneShot(searchSound);
                // Show the message for an empty cabinet
                if (dialogueManager != null)
                {
                    dialogueManager.ShowDialogue(emptyMessage);
                    Debug.Log($"Displayed message: {emptyMessage}");
                }
                else
                {
                    Debug.LogError("DialogueManager is missing.");
                }
            }
        }
        else
        {
            Debug.LogWarning("Player is not in range. Cannot search cabinet.");
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
