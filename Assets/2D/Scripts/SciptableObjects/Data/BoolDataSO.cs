using UnityEngine;

/// <summary>
/// A ScriptableObject that stores a boolean value with a default value that gets reset on play.
/// Can be created as an asset in the project and referenced by multiple scripts.
/// </summary>
[CreateAssetMenu(fileName = "BoolData", menuName = "Scriptable Objects/Data/BoolData")]
public class BoolDataSO : ValueData<bool>
{
	/// <summary>
	/// Implicit conversion operator that allows using this ScriptableObject directly as a bool.
	/// Example usage: bool isActive = myBoolData; // instead of bool isActive = myBoolData.Value;
	/// Returns false if the variable is null.
	/// </summary>
	/// <param name="variable">The BoolDataSO object to convert</param>
	public static implicit operator bool(BoolDataSO variable)
	{
		return variable != null && variable.Value;
	}
}