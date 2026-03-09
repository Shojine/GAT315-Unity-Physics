using UnityEngine.Events;
using UnityEngine;

/// <summary>
/// Component that deals damage to health components on contact
/// </summary>
public class DamageInflictor : MonoBehaviour
{
	[Header("Damage Settings")]
	[SerializeField] float damageAmount = 10;
	[SerializeField] float damageRate = 1;
	[SerializeField] bool destroySelfOnDamage = false;
	[SerializeField] string affectTag;
	[SerializeField] LayerMask affectLayers = Physics.AllLayers;

	[Header("Knockback")]
	[SerializeField] float knockbackForce = 4f;    // Horizontal knockback strength
	[SerializeField] float knockbackVertical = 3f; // Vertical bounce height

	float damageTimer = 0;


    private void OnCollisionEnter2D(Collision2D collision)
    {
		ApplyDamage(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
		ApplyDamage(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ApplyDamage(other.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        ApplyDamage(other.gameObject);
    }

    private void ApplyDamage(GameObject target)
    {
        if (Time.time < damageTimer || !IsValid(target)) return;

        if (target.TryGetComponent<Health>(out Health health))
        {
            Vector2 knockback = CalculateKnockback(target.transform.position);
            health.ApplyDamage(damageAmount, knockback);
            damageTimer = Time.time + damageRate;
            if (destroySelfOnDamage) Destroy(gameObject);
        }
    }

    private Vector2 CalculateKnockback(Vector3 targetPosition)
    {
        float horizontal = Mathf.Sign(targetPosition.x - transform.position.x);
        return new Vector2(horizontal * knockbackForce, knockbackVertical);
    }


    private bool IsValid(GameObject target)
    {
        if((affectLayers & (1 << target.layer)) == 0)
        {
            return false;
        }

        return (string.IsNullOrEmpty(affectTag) || target.CompareTag(affectTag));
    }

}