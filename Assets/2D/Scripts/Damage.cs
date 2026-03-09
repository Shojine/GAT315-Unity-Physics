using System;
using UnityEngine;

/// <summary>
/// Damage type enum - extend this with your game-specific damage types
/// </summary>
public enum DamageType
{
	Normal,     // Standard damage with no special properties
	Fire,       // Fire-based damage, potentially causing burning effects
	Ice,        // Ice-based damage, potentially causing slowing effects
	Electric,   // Electric damage, potentially causing stun effects
	Poison,     // Poison damage, potentially causing damage over time
	Magic,      // Magic-based damage, might bypass physical defenses
	Physical,   // Pure physical damage, might be reduced by armor
				// Add more as needed
}

/// <summary>
/// Class to hold data about a damage event
/// </summary>
[Serializable]
public class DamageData
{
	public float Amount;            // How much damage was dealt
	public DamageType DamageType;   // What type of damage was applied
	public GameObject Source;       // What object caused the damage
	public GameObject Target;       // What object received the damage
}