   using UnityEngine;

   public class PlayerStartPosition : MonoBehaviour
   {
       private void Start()
       {
           // Retrieve the stored position
           float startX = PlayerPrefs.GetFloat("StartPosX", transform.position.x);
           float startY = PlayerPrefs.GetFloat("StartPosY", transform.position.y);

           // Set the player's position
           transform.position = new Vector2(startX, startY);
       }
   }
   