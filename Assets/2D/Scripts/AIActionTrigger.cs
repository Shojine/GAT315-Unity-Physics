using UnityEngine;

/// <summary>
/// Places a trigger in the game world that affects AI character behavior when they enter its zone.
/// Used to create environmental waypoints that guide AI movement and actions.
/// </summary>
public class AIActionTrigger : MonoBehaviour
{
	/// <summary>
	/// Defines possible actions that can be executed when an AI enters this trigger zone
	/// </summary>
	enum Action
	{
		Idle,           // Makes the AI pause for a specified duration
		LeftDirection,  // Forces AI to face left
		RightDirection, // Forces AI to face right
		FlipDirection,  // Reverses AI's current direction
		RandomDirection,// Sets AI direction randomly (50/50 chance)
		Jump            // Makes the AI perform a jump
	}

	[SerializeField] bool leftDirection = true;  // If true, affects AI moving leftward
	[SerializeField] bool rightDirection = true; // If true, affects AI moving rightward
	[SerializeField] Action action;              // Action to perform when triggered
	[SerializeField] AICharacter.State state = AICharacter.State.Any; // Required AI state for trigger to work
	[SerializeField, Range(0, 1)] float chance = 1; // Probability (0-1) that action will execute
	[SerializeField] float time;                 // Duration parameter (used for Idle action)

	/// <summary>
	/// Called when another collider enters this trigger's zone
	/// </summary>
	/// <param name="collision">The collider that entered the trigger</param>
	private void OnTriggerEnter2D(Collider2D collision)
	{ 
		// Check if the colliding object has an AICharacter component
		if (collision.gameObject.TryGetComponent(out AICharacter ai))
		{

			// Skip execution if AI's current state doesn't match the required state
			// (Only proceeds if the trigger accepts any state OR if AI is in the specific required state)
			if (state != AICharacter.State.Any && ai.CurrentState != state) return;

			// Probabilistic check - skip based on chance value
			if (Random.value > chance) return;

			// Get AI's current movement direction
			Vector2 velocity = ai.characterController.rb.linearVelocity;

			// Determine if action should execute based on movement direction settings
			// - Executes if both directions are enabled
			// - Executes if moving left and leftDirection is enabled
			// - Executes if moving right and rightDirection is enabled
			bool execute = (leftDirection && rightDirection) ||
						   (leftDirection && velocity.x < 0) ||
						   (rightDirection && velocity.x > 0);

			// Execute the configured action if conditions are met
			if (execute) ExecuteAction(ai);
		}
        Debug.Log(collision.gameObject.name + " entered trigger: " + gameObject.name);

    }

    /// <summary>
    /// Performs the selected action on the AI character
    /// </summary>
    /// <param name="ai">The AICharacter to affect</param>
    private void ExecuteAction(AICharacter ai)
    {
        Debug.Log($"Executing action: {action} on AI: {ai.name}");

        switch (action)
        {
            case Action.Idle:
                Debug.Log("AI will idle.");
                ai.Idle(time); // Ensure this calls the Idle method in AICharacter
                break;
            case Action.LeftDirection:
                Debug.Log("AI will face left.");
                ai.SetDirection(CharacterController2D.FACE_LEFT);
                break;
            case Action.RightDirection:
                Debug.Log("AI will face right.");
                ai.SetDirection(CharacterController2D.FACE_RIGHT);
                break;
            case Action.FlipDirection:
                Debug.Log("AI will flip direction.");
                ai.FlipDirection();
                break;
            case Action.RandomDirection:
                Debug.Log("AI will choose a random direction.");
                bool randomDirection = Random.value > 0.5f;
                ai.SetDirection(randomDirection ? CharacterController2D.FACE_RIGHT : CharacterController2D.FACE_LEFT);
                break;
            default:
                Debug.LogWarning("No valid action selected.");
                break;
        }
    }

}