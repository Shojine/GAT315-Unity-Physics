using UnityEngine;

/// <summary>
/// Generic base class for ScriptableObjects that store a single value of type T.
/// Provides reset-to-default functionality and basic value operations.
/// </summary>
/// <typeparam name="T">The type of value stored in this ScriptableObject</typeparam>
public abstract class ValueData<T> : ScriptableObjectBase
{
	/// <summary>
	/// The default value that will be restored when the game starts.
	/// </summary>
	[Tooltip("The default value that will be restored when the game starts")]
	[SerializeField] protected T defaultValue;

	/// <summary>
	/// The current value stored in this ScriptableObject.
	/// Will be reset to defaultValue when the game starts.
	/// </summary>
	[SerializeField] protected T value;

	/// <summary>
	/// Public property to access or modify the stored value.
	/// </summary>
	public virtual T Value { get => value; set => this.value = value; }

	/// <summary>
	/// Public property to access the default value.
	/// </summary>
	public virtual T DefaultValue => defaultValue;

	/// <summary>
	/// Called when the ScriptableObject is loaded or when the game starts.
	/// Resets the value to the default value.
	/// </summary>
	protected virtual void OnEnable()
	{
		ResetToDefault();
	}

	/// <summary>
	/// Resets the current value to the default value.
	/// Can be called manually if needed.
	/// </summary>
	public virtual void ResetToDefault()
	{
		value = defaultValue;
	}

	/// <summary>
	/// Sets the default value and also updates the current value.
	/// </summary>
	public virtual void SetDefaultValue(T newDefault)
	{
		defaultValue = newDefault;
		value = newDefault;
	}
}