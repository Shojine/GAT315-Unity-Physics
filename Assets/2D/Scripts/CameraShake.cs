using System.Collections;
using UnityEngine;

/// <summary>
/// Singleton camera shake component. Attach to the Main Camera.
/// Call CameraShake.Shake(intensity, duration) from anywhere to trigger a shake.
/// </summary>
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Shake Defaults")]
    [SerializeField] float defaultIntensity = 0.2f;
    [SerializeField] float defaultDuration = 0.15f;

    Vector3 originalLocalPosition;
    Coroutine shakeCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        originalLocalPosition = transform.localPosition;
    }

    /// <summary>
    /// Trigger a camera shake with custom intensity and duration.
    /// </summary>
    public static void Shake(float intensity, float duration)
    {
        if (Instance == null) return;
        Instance.TriggerShake(intensity, duration);
    }

    /// <summary>
    /// Trigger a camera shake using the inspector defaults.
    /// Callable from UnityEvents (no parameters).
    /// </summary>
    public void ShakeDefault()
    {
        TriggerShake(defaultIntensity, defaultDuration);
    }

    void TriggerShake(float intensity, float duration)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeRoutine(intensity, duration));
    }

    IEnumerator ShakeRoutine(float intensity, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            float currentIntensity = intensity * (1f - progress); // fade out

            transform.localPosition = originalLocalPosition + (Vector3)Random.insideUnitCircle * currentIntensity;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPosition;
        shakeCoroutine = null;
    }
}
