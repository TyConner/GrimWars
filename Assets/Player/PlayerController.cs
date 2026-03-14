using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ITakeDamage
{
    [Header("Input System")]
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference firePrimaryAction;

    [Header("Refs")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer spr;
    [SerializeField] Sprite deathSprite;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float acceleration = 60f;
    [SerializeField] float deceleration = 80f;

    Vector2 moveInput;
    Vector2 desiredVelocity;
    Vector2 velSmoothRef;

    [Header("Stats")]
    [SerializeField] int maxHP = 3;
    int currentHP;

    [Header("Player Feedback")]
    [SerializeField] float damageFlashTime = 0.08f;
    [SerializeField] float damageCameraShakeDuration = 0.2f;
    [SerializeField] float damageCameraShakeAmplitude = 0.02f;

    [Header("Fireball")]
    [SerializeField] float fireballLockTime = 0.45f;

    [SerializeField] Animator anim;

    Color originalColor;
    Coroutine flashRoutine;
    Coroutine fireballRoutine;

    bool bDead;
    bool bMovementLocked;
    bool bIsCastingFireball;

    void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.linearVelocity = Vector2.zero;
    }

    void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        currentHP = maxHP;
        originalColor = spr.color;
    }

    void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }

        if (firePrimaryAction != null)
        {
            firePrimaryAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }

        if (firePrimaryAction != null)
        {
            firePrimaryAction.action.Disable();
        }
    }

    void Update()
    {
        if (bDead) return;

        if (!bMovementLocked)
        {
            if (moveAction == null)
            {
                moveInput = Vector2.zero;
            }
            else
            {
                moveInput = moveAction.action.ReadValue<Vector2>();

                if (moveInput.sqrMagnitude > 0f)
                {
                    moveInput = moveInput.normalized;

                    anim.SetFloat("LastInputX", moveInput.x);
                    anim.SetFloat("LastInputY", moveInput.y);
                    anim.SetBool("bIsMoving", true);

                    if (moveInput.x < 0f)
                    {
                        spr.flipX = true;
                    }
                    else if (moveInput.x > 0f)
                    {
                        spr.flipX = false;
                    }
                }
                else
                {
                    anim.SetBool("bIsMoving", false);
                }
            }
        }
        else
        {
            moveInput = Vector2.zero;
            anim.SetBool("bIsMoving", false);
        }

        if (firePrimaryAction != null && firePrimaryAction.action.WasPressedThisFrame())
        {
            TryFireball();
        }
    }

    void FixedUpdate()
    {
        if (bDead) return;

        if (bMovementLocked)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        desiredVelocity = moveInput * moveSpeed;

        float accel = moveInput.sqrMagnitude > 0f ? acceleration : deceleration;

        Vector2 newVel = Vector2.SmoothDamp(
            rb.linearVelocity,
            desiredVelocity,
            ref velSmoothRef,
            1f / Mathf.Max(accel, 0.0001f)
        );

        rb.linearVelocity = newVel;
    }

    void TryFireball()
    {
        if (bDead) return;
        if (bMovementLocked) return;
        if (bIsCastingFireball) return;

        fireballRoutine = StartCoroutine(FireballRoutine());
    }

    IEnumerator FireballRoutine()
    {
        bIsCastingFireball = true;
        bMovementLocked = true;

        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        velSmoothRef = Vector2.zero;

        anim.SetBool("bIsMoving", false);
        anim.SetBool("bIsFireball", true);

        yield return new WaitForSeconds(fireballLockTime / 2);
        anim.SetBool("bIsFireball", false); //Must do this or blend out will cause two fireball casts on animation
        yield return new WaitForSeconds(fireballLockTime / 2);
        bMovementLocked = false;
        bIsCastingFireball = false;
        fireballRoutine = null;
    }

    public void TakeDamage(int amount)
    {
        if (bDead) return;

        GameInstance.Instance.ShakeCamera(damageCameraShakeDuration, damageCameraShakeAmplitude);
        currentHP = Math.Clamp(currentHP - amount, 0, maxHP);
        print("New Player HP: " + currentHP.ToString());

        if (currentHP <= 0)
        {
            Die();
        }

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(DamageFlash());
    }

    public void Heal(int amount)
    {
        if (bDead) return;

        currentHP = Math.Clamp(currentHP + amount, 0, maxHP);
    }

    public void Die()
    {
        rb.linearVelocity = Vector2.zero;
        spr.sprite = deathSprite;
        bDead = true;

        anim.SetBool("bIsMoving", false);
        anim.SetBool("bIsFireball", false);
        anim.SetBool("bIsDead", true);
    }

    IEnumerator DamageFlash()
    {
        for (int i = 0; i < 3; i++)
        {
            spr.color = Color.red;
            yield return new WaitForSeconds(0.03f);
            spr.color = originalColor;
            yield return new WaitForSeconds(0.03f);
        }

        flashRoutine = null;
    }
}