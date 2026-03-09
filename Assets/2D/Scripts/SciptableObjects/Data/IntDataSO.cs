using UnityEngine;

/// <summary>
/// A ScriptableObject that stores a boolean value with a default value that gets reset on play.
/// Can be created as an asset in the project and referenced by multiple scripts.
/// </summary>
[CreateAssetMenu(fileName = "IntData", menuName = "Scriptable Objects/Data/IntData")]
public class IntDataSO : ValueData<int>
{
	/// <summary>
	/// Implicit conversion operator to int.
	/// </summary>
	public static implicit operator int(IntDataSO variable)
	{
		return variable != null ? variable.Value : 0;
	}
}