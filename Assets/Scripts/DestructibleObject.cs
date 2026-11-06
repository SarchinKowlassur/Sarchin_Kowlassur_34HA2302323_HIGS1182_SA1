using System.Collections;
using UnityEngine;

/// <summary>
/// Handles mesh hiding, death effect spawning, audio, and cleanup for destructible obstacles and enemies.
/// </summary>
public class DestructibleObject : MonoBehaviour
{
    [SerializeField] private Renderer[] meshRenderers;
    [SerializeField] private GameObject deathEffectPrefab;
    private bool isDestroyed = false;

    public void TriggerDeathSequence()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        StartCoroutine(DeathSequenceRoutine());
    }

    private IEnumerator DeathSequenceRoutine()
    {
        // Play corresponding death sound effect based on object tag
        if (AudioManager.Instance != null)
        {
            if (CompareTag("Asteroid"))
            {
                AudioManager.Instance.PlayAsteroidDeath();
            }
            else if (CompareTag("Enemy"))
            {
                AudioManager.Instance.PlayEnemyDeath();
            }
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (meshRenderers != null)
        {
            foreach (Renderer r in meshRenderers)
            {
                if (r != null) r.enabled = false;
            }
        }

        float effectDuration = 1.0f;
        if (deathEffectPrefab != null)
        {
            GameObject deathFX = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

            if (Camera.main != null)
            {
                deathFX.transform.LookAt(Camera.main.transform);
            }

            Animator anim = deathFX.GetComponent<Animator>();
            if (anim != null && anim.runtimeAnimatorController != null && anim.runtimeAnimatorController.animationClips.Length > 0)
            {
                AnimationClip clip = anim.runtimeAnimatorController.animationClips[0];
                if (clip != null) effectDuration = clip.length;
            }
        }

        yield return new WaitForSeconds(effectDuration);
        Destroy(transform.root.gameObject);
    }
}