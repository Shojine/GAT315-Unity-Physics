// Required Unity namespace for ScriptableObject functionality
using UnityEngine;

// Base class that other ScriptableObjects can inherit from
// Provides common functionality like description field
[CreateAssetMenu(fileName = "FloatData", menuName = "ScriptableObjects/Data/Float Data")]
public class ScriptableObjectBase : ScriptableObject
{
    public float value;
}