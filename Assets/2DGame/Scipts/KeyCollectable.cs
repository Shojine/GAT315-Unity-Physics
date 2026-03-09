using UnityEngine;

public class KeyCollectible : MonoBehaviour
{
    [SerializeField] private string keyID; // Unique ID for this key

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Add the key to the player's inventory
            var characterController = other.GetComponent<CharacterController2D>();
            if (characterController != null)
            {
                characterController.AddKey(keyID);
                Destroy(gameObject); // Remove the key from the scene
            }
        }
    }
}
