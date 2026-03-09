using UnityEngine;

public class CabinetInteractable : MonoBehaviour
{
    [SerializeField] private string keyID; // The ID of the key hidden in the cabinet
    [SerializeField] private string searchMessage = "You found a key!"; // Message to display when the key is found
    [SerializeField] private string emptyMessage = "The cabinet is empty."; // Message to display if the cabinet has already been searched
    [SerializeField] private AudioClip searchSound; // Sound to play when searching the cabinet

    private bool isPlayerInRange = false;
    private bool hasKey = false; // Whether the cabinet still contains the key
    private DialogueManager dialogueManager;
    private AudioSource audioSource;

    private void Start()
    {
        if(!string.IsNullOrEmpty(keyID)) hasKey = true;
        dialogueManager = FindObjectOfType<DialogueManager>(); // Find the DialogueManager in the scene
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
    }

    public void SearchCabinet()
    {
        if (isPlayerInRange)
        {
            Debug.Log("Player is in range. Searching cabinet...");

            // Play the search sound
            if (audioSource != null && searchSound != null)
            {
                Debug.Log("Playing search sound...");
                audioSource.PlayOneShot(searchSound);
            }
            else
            {
                Debug.LogWarning("AudioSource or searchSound is missing.");
            }

            if (hasKey)
            {
                Debug.Log("Cabinet contains a key. Adding key to inventory...");

                // Add the key to the player's inventory
                var player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController2D>();
                if (player != null)
                {
                    player.AddKey(keyID);
                    Debug.Log($"Key '{keyID}' added to player's inventory.");
                }
                else
                {
                    Debug.LogError("Player GameObject or CharacterController2D script is missing.");
                }

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

                hasKey = false; // Mark the cabinet as empty
            }
            else
            {
                Debug.Log("Cabinet is empty.");

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
