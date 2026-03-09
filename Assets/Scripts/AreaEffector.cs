using UnityEngine;

// Ensures that a BoxCollider component is added to any GameObject this script is attached to
[RequireComponent(typeof(BoxCollider))]
public class AreaEffector : MonoBehaviour
{
	// Force value between -100 and 100. Positive pushes in the forward direction, negative pulls in the opposite direction
	[SerializeField, Range(-100, 100)] float force = 10f;

	// Determines how the force is applied (Force, Acceleration, Impulse, or VelocityChange)
	[SerializeField] ForceMode forceMode = ForceMode.Force;

	// Determines which layers will be affected by the effector
	[SerializeField] LayerMask affectedLayers = Physics.AllLayers;

	// Reference to the box collider that determines the effect area
	private BoxCollider boxCollider;

	void Awake()
	{
		// Cache the BoxCollider component and configure it
		boxCollider = GetComponent<BoxCollider>();
		boxCollider.isTrigger = true;  // Set as trigger to detect objects without physical collision
		boxCollider.size = Vector3.one; // Set default size to 1x1x1
	}

	// Called every frame for each collider staying inside the trigger
	private void OnTriggerStay(Collider other)
	{
		// Get the rigidbody attached to the collider
		Rigidbody rb = other.attachedRigidbody;

		// Skip if no rigidbody or if the object's layer isn't in the affected layers
		if (rb == null || (affectedLayers & (1 << other.gameObject.layer)) == 0) return;

		// The force direction is always forward from the effector (based on the GameObject's forward direction)
		Vector3 forceVector = transform.forward * force;

		// Apply the force to the rigidbody using the specified force mode
		rb.AddForce(forceVector, forceMode);
	}
}