using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Complete Player Controller with side-scrolling movement, raycast shooting, spread-shot power-up, particles, audio, and delayed death sequence.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Speeds & Tilt")]
    [SerializeField] private float moveSpeedX = 12f;
    [SerializeField] private float moveSpeedY = 16f;
    [SerializeField] private float cameraScrollSpeed = 5f;
    [SerializeField] private float tiltAngle = 20f;

    [Header("Screen Edge Padding")]
    [SerializeField] private float paddingX = 1.0f;
    [SerializeField] private float paddingY = 0.3f;

    [Header("Shooting & Projectile Settings")]
    [SerializeField] private GameObject laserProjectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float weaponRange = 100f;
    [SerializeField] private LayerMask shootableLayer;

    [Header("Visuals, Particles & Animation")]
    [SerializeField] private ParticleSystem idleThrusters;
    [SerializeField] private ParticleSystem movingThrusters;
    [SerializeField] private Animator animator;
    [SerializeField] private Renderer[] playerMeshRenderers;
    [SerializeField] private GameObject deathEffectPrefab;

    private Camera mainCamera;
    private bool isSpreadShotActive = false;
    private bool isDead = false;

    private void Start()
    {
        mainCamera = Camera.main;
        if (animator == null) animator = GetComponent<Animator>();

        if (idleThrusters != null) idleThrusters.Play();
        if (movingThrusters != null) movingThrusters.Stop();

        // Ensure gameplay music plays when player starts
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }
    }

    private void Update()
    {
        if (isDead || (GameManager.Instance != null && GameManager.Instance.IsGameOver)) return;

        HandleMovementAndClamping();
        HandleShooting();
    }

    private void HandleMovementAndClamping()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveY = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveY = -1f;
        }

        bool isMovingVertically = Mathf.Abs(moveY) > 0.1f || Mathf.Abs(moveX) > 0.1f;
        if (isMovingVertically)
        {
            if (idleThrusters != null && idleThrusters.isPlaying) idleThrusters.Stop();
            if (movingThrusters != null && !movingThrusters.isPlaying) movingThrusters.Play();
        }
        else
        {
            if (movingThrusters != null && movingThrusters.isPlaying) movingThrusters.Stop();
            if (idleThrusters != null && !idleThrusters.isPlaying) idleThrusters.Play();
        }

        if (animator != null)
        {
            animator.SetFloat("MoveY", moveY);
        }

        float velX = (moveX * moveSpeedX) + cameraScrollSpeed;
        float velY = moveY * moveSpeedY;

        Vector3 movement = new Vector3(velX, velY, 0f) * Time.deltaTime;
        Vector3 targetPos = transform.position + movement;

        float distanceToCam = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);
        Vector3 minScreen = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, distanceToCam));
        Vector3 maxScreen = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, distanceToCam));

        targetPos.x = Mathf.Clamp(targetPos.x, minScreen.x + paddingX, maxScreen.x - paddingX);
        targetPos.y = Mathf.Clamp(targetPos.y, minScreen.y + paddingY, maxScreen.y - paddingY);
        targetPos.z = 0f;

        transform.position = targetPos;

        float targetTilt = Mathf.Clamp(moveY * tiltAngle, -tiltAngle, tiltAngle);
        transform.rotation = Quaternion.Euler(0f, 90f, targetTilt);
    }

    private void HandleShooting()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (animator != null)
            {
                animator.SetTrigger("Shoot");
            }

            // Play Laser Shoot SFX
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayLaserShoot();
            }

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;

            Vector3 spawnPoint = firePoint != null ? firePoint.position : transform.position;
            Quaternion baseRotation = Quaternion.Euler(0f, 90f, 0f);

            if (Physics.Raycast(ray, out hit, weaponRange, shootableLayer))
            {
                Vector3 targetDirection = (hit.point - spawnPoint).normalized;
                baseRotation = Quaternion.LookRotation(targetDirection);

                if (hit.collider.CompareTag("Asteroid") || hit.collider.CompareTag("Enemy"))
                {
                    DestructibleObject destructible = hit.collider.GetComponentInParent<DestructibleObject>();
                    if (destructible != null)
                    {
                        destructible.TriggerDeathSequence();
                    }
                    else
                    {
                        Destroy(hit.transform.root.gameObject);
                    }
                }
            }

            if (laserProjectilePrefab != null)
            {
                if (isSpreadShotActive)
                {
                    Instantiate(laserProjectilePrefab, spawnPoint, baseRotation);
                    Instantiate(laserProjectilePrefab, spawnPoint, Quaternion.AngleAxis(15f, Vector3.forward) * baseRotation);
                    Instantiate(laserProjectilePrefab, spawnPoint, Quaternion.AngleAxis(-15f, Vector3.forward) * baseRotation);
                }
                else
                {
                    Instantiate(laserProjectilePrefab, spawnPoint, baseRotation);
                }
            }
        }
    }

    public void ActivateSpreadShot(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(PowerUpRoutine(duration));
    }

    private IEnumerator PowerUpRoutine(float duration)
    {
        isSpreadShotActive = true;
        yield return new WaitForSeconds(duration);
        isSpreadShotActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckCollisionTag(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckCollisionTag(collision.gameObject);
    }

    private void CheckCollisionTag(GameObject hitObject)
    {
        if (!isDead && (hitObject.CompareTag("Asteroid") || hitObject.CompareTag("Enemy")))
        {
            StartCoroutine(HandlePlayerDeathSequence());
        }
    }

    private IEnumerator HandlePlayerDeathSequence()
    {
        isDead = true;
        Debug.Log("Player hit! Executing death sequence.");

        // 1. Play Player Death SFX immediately
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlayerDeath();
        }

        if (idleThrusters != null) idleThrusters.Stop();
        if (movingThrusters != null) movingThrusters.Stop();

        if (animator != null)
        {
            animator.SetTrigger("IsDead");
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        float deathAnimDuration = 1.2f;
        if (deathEffectPrefab != null)
        {
            GameObject deathFX = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            deathFX.transform.LookAt(mainCamera.transform);

            ParticleSystem deathPS = deathFX.GetComponentInChildren<ParticleSystem>();
            if (deathPS != null)
            {
                deathAnimDuration = Mathf.Max(deathPS.main.duration, 1.0f);
            }
        }

        if (playerMeshRenderers != null)
        {
            foreach (Renderer r in playerMeshRenderers)
            {
                if (r != null) r.enabled = false;
            }
        }

        // 2. WAIT for the death animation/particles to finish playing completely
        yield return new WaitForSeconds(deathAnimDuration);

        // 3. NOW switch to menu music after the death sequence ends
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMenuMusic();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver(false);
        }

        Destroy(gameObject);
    }
}