using UnityEngine;

/// <summary>
/// A utility class that demonstrates different types of physics casting in Unity
/// with visual debugging through Gizmos.
/// </summary>
public class PhysicsRaycast : MonoBehaviour
{
	// Enum to define different types of physics casting methods
	public enum eType
	{
		Ray,    // Simple ray cast (line)
		Sphere, // Sphere cast (sweeping a sphere along a ray)
		Box     // Box cast (sweeping a box along a ray)
	}

	[SerializeField] eType type = eType.Ray;      // The type of cast to perform
	[SerializeField] float size = 1;              // The size of the sphere or box (diameter/width)
	[SerializeField] float distance = 2;          // How far to cast
	[SerializeField] LayerMask layerMask = Physics.AllLayers;  // Which layers to include in the cast

	// Array to store all hits from the physics cast
	RaycastHit[] raycastHits;

	void Update()
	{
		PerformRaycast(type); // Perform the cast based on the selected type
	}

	public void PerformRaycast(eType type)
	{
		// Perform the appropriate physics cast based on the selected type
		switch (type)
		{
			case eType.Ray:
				// Cast a ray from the object's position in its forward direction
				raycastHits = Physics.RaycastAll(transform.position, transform.forward, distance, layerMask);
				break;
			case eType.Sphere:
				// Cast a sphere from the object's position in its forward direction
				// Note: size * 0.5f is used to convert from diameter to radius
				raycastHits = Physics.SphereCastAll(transform.position, size * 0.5f, transform.forward, distance, layerMask);
				break;
			case eType.Box:
				// Cast a box from the object's position in its forward direction
				// Creates a cube with dimensions (size, size, size) and adjusts for center point
				raycastHits = Physics.BoxCastAll(transform.position, Vector3.one * size * 0.5f, transform.forward, transform.rotation, distance, layerMask);
				break;
		}
	}

	// Draw visual representations of the casts in the Scene view
	private void OnDrawGizmosSelected()
	{
		// Draw the cast shape in blue
		Gizmos.color = Color.blue;
		switch (type)
		{
			case eType.Ray:
				// Draw a simple line for ray cast
				Gizmos.DrawRay(transform.position, transform.forward * distance);
				break;
			case eType.Sphere:
				// Draw the ray and a wire sphere at the end position
				Gizmos.DrawRay(transform.position, transform.forward * distance);
				Gizmos.DrawWireSphere(transform.position + transform.forward * distance, size * 0.5f);
				break;
			case eType.Box:
				// Draw the ray and a wire cube at the end position
				Gizmos.DrawRay(transform.position, transform.forward * distance);
				Gizmos.DrawWireCube(transform.position + transform.forward * distance, Vector3.one * size);
				break;
		}

		// If we have raycast hits, draw red wire cubes around the colliders that were hit
		if (raycastHits != null)
		{
			Gizmos.color = Color.red;
			foreach (var hit in raycastHits)
			{
				// Draw a wire cube that matches the bounds of the hit collider
				Gizmos.DrawWireCube(hit.collider.bounds.center, hit.collider.bounds.size);
			}
		}
	}
}