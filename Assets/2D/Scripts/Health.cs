using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Health component for any damageable entity in the game
/// </summary>

public class Health : MonoBehaviour
{
	[Header("Health Settings")]
	[SerializeField] private float maxHealth = 100f;
	[SerializeField] private float health = 100;
	[SerializeField] private bool destroyOnDeath = true;
	[SerializeField] private float destroyDelay = 0f;
	[SerializeField] private UnityEvent onDamage;
	[SerializeField] private UnityEvent onDeath;

    private bool isDead = false;


	private void Awake()
	{
		health = maxHealth;

	}

	public void ApplyDamage(float damage)
	{
		if (isDead) return;

		health -= damage;

		// Trigger damage event
		gameObject.GetComponent<CharacterController2D>().OnHit();

		onDamage?.Invoke();

        if (health <= 0)
		{
			Die();
		}
	}

	public void Heal(float amount)
	{
		if (isDead) return;

		health = Mathf.Min(health + amount, maxHealth);
	}

	public void Die()
	{
		if (isDead) return;

		onDeath?.Invoke();

        isDead = true;
        health = 0;
		// Trigger death event	
		gameObject.GetComponent<CharacterController2D>().OnDeath();

        if (destroyOnDeath)
		{
			Destroy(gameObject, destroyDelay);
		}


    }

}