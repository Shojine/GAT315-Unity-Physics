using UnityEngine;

public class PointEffector : MonoBehaviour
{
    [Range (-10, 10)]
    [SerializeField] float force = 1.0f;

    [Range (0, 100)]
    [SerializeField] float radius = 1.0f;

    private void OnValidate()
    {
        transform.localScale = new Vector3(radius, radius, radius);
    }


    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) return;
       
        if (other.CompareTag("Player"))
        {
                Vector3 direction = (other.transform.position - transform.position).normalized;
                rb.AddForce(direction * force, ForceMode.Impulse);
        }
    }
}
