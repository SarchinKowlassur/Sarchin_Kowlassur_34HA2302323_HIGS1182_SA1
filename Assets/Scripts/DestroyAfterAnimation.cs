using UnityEngine;

/// <summary>
/// Automatically destroys the death effect GameObject when its sprite animation finishes.
/// </summary>
public class DestroyAfterAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // Called via an Animation Event placed on the last frame of Death.anim
    public void AnimationFinished()
    {
        Destroy(gameObject);
    }
}