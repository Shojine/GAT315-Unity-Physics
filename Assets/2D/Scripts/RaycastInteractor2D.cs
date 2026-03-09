using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A component that performs raycasts in 2D space and invokes events when objects are hit.
/// Supports filtering by tags and layers and provides visual debugging in the editor.
/// </summary>
public class RaycastInteractor2D : MonoBehaviour
{
	// Configuration parameters
	[SerializeField] float length = 1;                        // How far the raycast extends
	[SerializeField] float timeInterval = 0;                  // Time between raycasts (0 = every frame)
	[SerializeField] string[] tagsToAffect;                   // Only affect objects with these tags (empty array = affect all)
	[SerializeField] LayerMask affectLayers = Physics.AllLayers; // Only affect objects on these layers

	// Event triggered when raycast hits or misses an object
	// Passes the hit GameObject or null if nothing was hit
	[SerializeField] UnityEvent<GameObject> onRaycastHit;

	// Direction control (1 = right, -1 = left)
	float raycastDirection = 1;

	// Public property to get/set the raycast direction from other scripts
	public float Direction { get { return raycastDirection; } set { raycastDirection = value; } }

	// Reference to the active coroutine to prevent multiple instances
	private Coroutine raycastCoroutine;

	// Track the last hit object to avoid redundant events
	private GameObject lastHitObject = null;

	/// <summary>
	/// Start the raycast coroutine when the component is enabled
	/// </summary>
	void OnEnable()
	{
		// Stop any existing coroutine to prevent duplicates
		if (raycastCoroutine != null)
		{
			StopCoroutine(raycastCoroutine);
		}

		// Start a new raycast coroutine
		raycastCoroutine = StartCoroutine(PerformRaycast(timeInterval));
	}

	/// <summary>
	/// Clean up the coroutine when the component is disabled
	/// </summary>
	private void OnDisable()
	{
		// Stop the coroutine if it's running
		if (raycastCoroutine != null)
		{
			StopCoroutine(raycastCoroutine);
			raycastCoroutine = null;
		}
	}

	/// <summary>
	/// Continuously performs raycasts at the specified time interval
	/// </summary>
	/// <param name="time">Time in seconds between raycasts (0 = every frame)</param>
	IEnumerator PerformRaycast(float time)
	{
		while (true)
		{
			// Perform the raycast in the current direction
			RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, Vector2.right * raycastDirection, length, affectLayers);

			// Check if the raycast hit something
			if (raycastHit2D.collider != null)
			{
				// Only process if it's a different object than the last one hit
				if (raycastHit2D.collider.gameObject != lastHitObject)
				{
					// Check if the hit object has a tag we're looking for, or if we're accepting all tags
					if (tagsToAffect.Length == 0 || System.Array.Exists(tagsToAffect, tag => raycastHit2D.collider.gameObject.CompareTag(tag)))
					{
						// Invoke the hit event with the hit GameObject
						onRaycastHit.Invoke(raycastHit2D.collider.gameObject);

						// Update the last hit object
						lastHitObject = raycastHit2D.collider.gameObject;
					}
				}
			}
			else
			{
				// Nothing was hit, invoke the event with null
				onRaycastHit.Invoke(null);

				// Clear the last hit object
				lastHitObject = null;
			}

			// Wait for the next raycast
			if (time > 0) yield return new WaitForSeconds(time);  // Wait for the specified time
			else yield return null;  // Wait until the next frame if no time specified
		}
	}

	/// <summary>
	/// Draw debug visualization in the Unity editor
	/// </summary>
	private void OnDrawGizmosSelected()
	{
		// Draw the ray in green
		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, Vector2.right * raycastDirection * length);

		// Draw a red sphere at the hit point if something is being hit
		if (lastHitObject != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(lastHitObject.transform.position, 0.1f);
		}
	}
}