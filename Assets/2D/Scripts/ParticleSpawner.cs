using UnityEngine;

/// <summary>
/// Spawns a particle effect prefab at this object's position or a given world position.
/// Wire SpawnAtSelf() to UnityEvents (e.g. Health.onDamage, CharacterController2D.onLand).
/// </summary>
public class ParticleSpawner : MonoBehaviour
{
    [SerializeField] ParticleSystem effectPrefab;

    /// <summary>
    /// Spawn the effect at a specific world position.
    /// </summary>
    public void Spawn(Vector3 position)
    {
        if (effectPrefab == null) return;

        ParticleSystem instance = Instantiate(effectPrefab, position, Quaternion.identity);
        Destroy(instance.gameObject, instance.main.duration + instance.main.startLifetime.constantMax);
    }

    /// <summary>
    /// Spawn the effect at this GameObject's position.
    /// Callable from UnityEvents with no parameters.
    /// </summary>
    public void SpawnAtSelf()
    {
        Spawn(transform.position);
    }
}
