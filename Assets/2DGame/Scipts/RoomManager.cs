using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private void Start()
    {
        // Retrieve the last door ID
        string lastDoorID = PlayerPrefs.GetString("LastDoorID", "");

        if (!string.IsNullOrEmpty(lastDoorID))
        {
            // Find the door with the matching ID
            RoomInteractable[] doors = FindObjectsOfType<RoomInteractable>();
            foreach (var door in doors)
            {
                if (door.doorID == lastDoorID)
                {
                    // Set the player's x position to the door's x position, keeping y and z unchanged
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                    {
                        Vector3 playerPosition = player.transform.position;
                        player.transform.position = new Vector3(door.transform.position.x, playerPosition.y, playerPosition.z);
                    }
                    break;
                }
            }
        }
    }
}
