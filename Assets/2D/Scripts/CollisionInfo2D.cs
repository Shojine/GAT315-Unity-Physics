using UnityEngine;

public class CollisionInfo2D : MonoBehaviour
{
    Material material;
    Color color;
    void Start()
    {
        material = gameObject.GetComponent<Renderer>().material;
        color = material.color;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            material.color = Color.green;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            material.color = Color.red;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        material.color = color;
    }
}
