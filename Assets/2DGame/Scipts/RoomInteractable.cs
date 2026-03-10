using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(AudioSource))]
public class RoomInteractable : MonoBehaviour
{
    [SerializeField] public string roomName;     // The name of the room to load
    [SerializeField] public string doorID;       // A unique identifier for this door
    [SerializeField] public string targetDoorID; // The doorID to spawn at in the destination scene
    [SerializeField] private Vector2 spawnOffset = new Vector2(1f, 0f); // Offset from door when spawning player
    [SerializeField] public bool isLocked = false; // Whether the door is locked
    [SerializeField] public string requiredKeyID;  // The ID of the key required to unlock the door
    [SerializeField] public string lockedMessage = "The door is locked."; // Message to display when locked
    [SerializeField] private AudioClip lockedSound; // Sound to play when the door is locked
    [SerializeField] private AudioClip unlockedSound; // Sound to play when the door is unlocked

    private bool isPlayerInRange = false;
    private GameObject player;
    private DialogueManager dialogueManager;
    private AudioSource audioSource;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        dialogueManager = FindObjectOfType<DialogueManager>(); // Find the DialogueManager in the scene
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component

        // If the player came through this door, spawn them here
        if (PlayerPrefs.GetString("LastDoorID", "") == doorID)
        {
            PlayerPrefs.DeleteKey("LastDoorID");
            if (player != null)
                player.transform.position = new Vector2(transform.position.x + spawnOffset.x, player.transform.position.y);
        }
    }

    public void OnInteract()
    {
        if (!enabled) return;
        if (isPlayerInRange)
        {
            if (isLocked)
            {
                // Check if the player has the required key
                var characterController = player.GetComponent<CharacterController2D>();
                if (characterController != null && characterController.HasKey(requiredKeyID))
                {
                    // Unlock the door if the player has the key
                    isLocked = false;
                    Debug.Log($"Door unlocked with key: {requiredKeyID}");
                    PlaySoundAndEnterRoom(unlockedSound, roomName); // Play the unlocked sound and enter the room
                }
                else
                {
                    if (dialogueManager)
                    {
                        // Show locked message if the player doesn't have the key
                        ShowDialogue(lockedMessage);
                    }
                    PlaySound(lockedSound); // Play the locked sound
                }
            }
            else
            {
                if(roomName == "Win")
                {
                    Destroy(FindFirstObjectByType<InventoryManager>().gameObject);
                }
                PlaySoundAndEnterRoom(unlockedSound, roomName); // Play the unlocked sound and enter the room
            }
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerInRange = false;
    }

    private void PlaySoundAndEnterRoom(AudioClip clip, string room)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip); // Play the sound
            StartCoroutine(WaitForSoundAndEnterRoom(clip.length, room)); // Wait for the sound to finish before entering the room
        }
        else
        {
            EnterRoom(room); // If no sound is provided, enter the room immediately
        }
    }

    private System.Collections.IEnumerator WaitForSoundAndEnterRoom(float delay, string room)
    {
        yield return new WaitForSeconds(delay); // Wait for the sound to finish
        EnterRoom(room); // Enter the room
    }

    private void EnterRoom(string room)
    {
        // Store the destination door ID so the next scene knows where to spawn the player
        PlayerPrefs.SetString("LastDoorID", targetDoorID);

        // Load the new scene
        SceneManager.LoadScene(room);

        Debug.Log($"Entering room: {room} through door: {doorID}");
    }

    private void ShowDialogue(string message)
    {
        if (dialogueManager != null)
        {
            dialogueManager.ShowDialogue(message); // Call the DialogueManager to display the message
        }
        else
        {
            Debug.LogError("DialogueManager reference is missing.");
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip); // Play the specified sound
        }
    }
}
