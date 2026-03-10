using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Place on every door in the game.
/// - doorID        : unique name for this door (e.g. "Kitchen_Left")
/// - targetScene   : scene to load when player walks through
/// - targetDoorID  : the doorID of the door to spawn at in the target scene
/// </summary>
public class SceneDoor : MonoBehaviour
{
    [Header("This Door")]
    [SerializeField] private string doorID;

    [Header("Destination")]
    [SerializeField] private string targetScene;
    [SerializeField] private string targetDoorID; // doorID of the door to spawn at in the next scene

    [Header("Spawn Offset")]
    [SerializeField] private Vector2 spawnOffset = new Vector2(0, 0); // Offset from door position when spawning

    private const string SpawnDoorKey = "SpawnDoorID";

    private void Start()
    {
        // If we're the door the player should spawn at, move the player here
        if (PlayerPrefs.GetString(SpawnDoorKey, "") == doorID)
        {
            PlayerPrefs.DeleteKey(SpawnDoorKey);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.transform.position = (Vector2)transform.position + spawnOffset;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerPrefs.SetString(SpawnDoorKey, targetDoorID);
        SceneManager.LoadScene(targetScene);
    }
}
