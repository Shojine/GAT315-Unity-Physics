using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Routes animation events to registered listeners
/// </summary>
public class AnimationEventRouter : MonoBehaviour
{
	// We need to use Action<AnimationEvent> for multicast delegates to work properly
	private Dictionary<string, Action<AnimationEvent>> eventRegistry = new Dictionary<string, Action<AnimationEvent>>();

	/// <summary>
	/// Add a listener to a specific animation event
	/// </summary>
	public void AddListener(string eventName, Action<AnimationEvent> listener)
	{
		// Normalize event name to avoid case sensitivity issues
		eventName = eventName.ToLower();

		// Check if the key exists
		if (eventRegistry.ContainsKey(eventName))
		{
			// Add the listener to the existing delegate chain
			eventRegistry[eventName] += listener;
		}
		else
		{
			// Create a new entry in the dictionary with just this listener
			eventRegistry[eventName] = listener;
		}
	}

	/// <summary>
	/// Remove a listener from a specific animation event
	/// </summary>
	public void RemoveListener(string eventName, Action<AnimationEvent> listener)
	{
		// Normalize event name
		eventName = eventName.ToLower();

		// Check if event exists
		if (!eventRegistry.ContainsKey(eventName))
		{
			Debug.LogWarning($"Cannot remove listener: Event '{eventName}' not registered on {gameObject.name}");
			return;
		}

		// Remove the listener from the delegate chain
		eventRegistry[eventName] -= listener;
	}

	/// <summary>
	/// Trigger an animation event - called directly from animation events
	/// </summary>
	public void TriggerEvent(AnimationEvent animEvent)
	{
		// Extract the event name from the string parameter
		string eventName = animEvent.stringParameter.ToLower();

		// Check if we have listeners for this event
		if (eventRegistry.TryGetValue(eventName, out Action<AnimationEvent> action))
		{
			if (action != null)
			{
				// Invoke all registered listeners
				action.Invoke(animEvent);
			}
			else
			{
				Debug.LogWarning($"Event '{eventName}' exists but has no active listeners on {gameObject.name}");
			}
		}
	}
}
	