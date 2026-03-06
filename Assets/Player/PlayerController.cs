using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ITakeDamage
{
    [Header("Input System")]
    [SerializeField] InputActionReference moveAction;

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

    [SerializeField] float damageFlashTime = 0.08f;

    Color originalColor;
    Coroutine flashRoutine;

    bool bDead;

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
    }

    void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }
    }

    void Update()
    {
        if (bDead) return;

        if (moveAction == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = moveAction.action.ReadValue<Vector2>();

        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput = moveInput.normalized;
        }
    }

    void FixedUpdate()
    {
        if (bDead) return;

        desiredVelocity = moveInput * moveSpeed;

        float accel = (moveInput.sqrMagnitude > 0f) ? acceleration : deceleration;

        Vector2 newVel = Vector2.SmoothDamp(
            rb.linearVelocity,
            desiredVelocity,
            ref velSmoothRef,
            1f / Mathf.Max(accel, 0.0001f)
        );

        rb.linearVelocity = newVel;

    }

    public void TakeDamage(int amount)
    {
        if (bDead) return;

        GameInstance.Instance.ShakeCamera(0.05f, 0.1f);
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
