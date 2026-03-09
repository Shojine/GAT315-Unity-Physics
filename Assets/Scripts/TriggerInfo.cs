using UnityEngine;

public class TriggerInfo : MonoBehaviour
{
    Material material;
    Color color;
    void Start()
    {
        material = gameObject.GetComponent<Renderer>().material;
        color = material.color;

    }

    private void OnTriggernEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            material.color = Color.green;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            material.color = Color.red;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        material.color = color;
    }
}
