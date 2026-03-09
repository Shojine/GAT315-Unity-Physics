using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    [SerializeField] float jumpHeight = 1.0f;
    [SerializeField] LayerMask layerMask = Physics.AllLayers;

    Rigidbody rb;
    Vector3 force;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Vector3.zero;
        direction.x =Input.GetAxis("Horizontal");
        direction.z = Input.GetAxis("Vertical");

        force = direction * speed;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * jumpHeight), ForceMode.Impulse);
        }

        //var colliders = Physics.OverlapSphere(transform.position, 2, layerMask);

        //foreach (var collider in colliders)
        //{
        //    Destroy(collider.gameObject);
        //}

        //Debug.DrawRay(transform.position, transform.forward * 5, Color.red);
        //if (Physics.Raycast(transform.position, transform.forward,out RaycastHit hit, 5, layerMask))
        //{
        //    Destroy(hit.collider.gameObject);
        //}
    }


    void FixedUpdate()
    { 
        rb.AddForce(force);
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2);
    }
}

