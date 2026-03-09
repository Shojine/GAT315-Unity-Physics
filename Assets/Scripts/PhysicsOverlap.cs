using UnityEngine;

/// <summary>
/// A utility class that demonstrates different types of physics overlap detection in Unity
/// with visual debugging through Gizmos. Also includes a gravity toggle feature.
/// </summary>
public class PhysicsOverlap : MonoBehaviour
{
	// Enum to define different types of overlap detection methods
	public enum eType
	{
		BoundingBox, // Check for overlaps with a box shape
		Sphere       // Check for overlaps with a sphere shape
	}

	[SerializeField] eType type = eType.BoundingBox; // The type of overlap detection to perform
	[SerializeField] float size = 1;                 // The size of the box or sphere (width/diameter)
	[SerializeField] LayerMask layerMask = Physics.AllLayers; // Which layers to include in the detection

	// Array to store all colliders detected in the overlap
	Collider[] colliders;

	void Update()
	{
		PerformOverlap(type); // Perform the overlap based on the selected type
	}

	public void PerformOverlap(eType type)
	{
		// Perform the appropriate overlap detection based on the selected type
		switch (type)
		{
			case eType.BoundingBox:
				// Detect all colliders overlapping with a box at the object's position
				// Note: size * 0.5f converts from full size to half-extents (required by OverlapBox)
				colliders = Physics.OverlapBox(transform.position, Vector3.one * size * 0.5f, transform.rotation, layerMask);
				break;
			case eType.Sphere:
				// Detect all colliders overlapping with a sphere at the object's position
				// Note: size * 0.5f converts from diameter to radius
				colliders = Physics.OverlapSphere(transform.position, size * 0.5f, layerMask);
				break;
		}
	}

	// Draw visual representations of the overlap shapes in the Scene view
	private void OnDrawGizmosSelected()
	{
		// Draw the overlap detection shape in green
		Gizmos.color = Color.green;
		switch (type)
		{
			case eType.BoundingBox:
				// Draw a wire cube to represent the box overlap area
				Gizmos.DrawWireCube(transform.position, Vector3.one * size);
				break;
			case eType.Sphere:
				// Draw a wire sphere to represent the sphere overlap area
				Gizmos.DrawWireSphere(transform.position, size * 0.5f);
				break;
		}

		// If we have detected colliders, draw red wire cubes around them
		Gizmos.color = Color.red;
		if (colliders != null)
		{
			foreach (var collider in colliders)
			{
				// Draw a wire cube that matches the bounds of each detected collider
				Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
			}
		}
	}
}
