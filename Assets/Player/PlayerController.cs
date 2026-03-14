using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, ITakeDamage
{
    [Header("Input System")]
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference lookAction;
    [SerializeField] InputActionReference attackAction;

    [Header("Refs")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer spr;
    [SerializeField] Sprite deathSprite;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float acceleration = 60f;
    [SerializeField] float deceleration = 80f;

    Vector2 moveInput;
    Vector2 lookInput;
    Vector2 desiredVelocity;
    Vector2 velSmoothRef;

    [Header("Stats")]
    [SerializeField] int maxHP = 3;
    int currentHP;
    [SerializeField] int maxMana = 10;
    int currentMana;
    [Header("Player Feedback")]
    [SerializeField] float damageFlashTime = 0.08f;
    [SerializeField] float damageCameraShakeDuration = 0.2f;
    [SerializeField] float damageCameraShakeAmplitude = 0.02f;

    [SerializeField] Animator anim;

    [SerializeField] grimoireSystem Grimoire;

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
        if (attackAction != null)
        {
            attackAction.action.Enable();

        }
        if (lookAction != null)
        {
            lookAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }
        if (attackAction != null)
        {
            attackAction.action.Disable();

        }
        if(lookAction != null)
        {
            lookAction.action.Disable();
        }
    }

    void Update()
    {
        if (bDead) return;
        //move
        if (moveAction == null)
        {
            moveInput = Vector2.zero;
            return;
        }

     
        moveInput = moveAction.action.ReadValue<Vector2>();
        

        if (moveInput.sqrMagnitude > 0f)
        {
            anim.SetFloat("LastInputX", moveInput.x);
            anim.SetFloat("LastInputY", moveInput.y);
            anim.SetBool("bIsMoving", true);
            moveInput = moveInput.normalized;
            if (moveInput.x < 0)
            {
                spr.flipX = true;
            }
            else if (moveInput.x != 0)
            {
                spr.flipX = false;
            }
        }
        else
        {
            anim.SetBool("bIsMoving", false);
        }
        //look

        lookInput = lookAction.action.ReadValue<Vector2>();

        //attack
        if(attackAction.action.WasPressedThisFrame())
        {
            Attack();
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
        anim.SetBool("bIsDead", true);
    }

    public void Attack()
    {
        if (Grimoire != null)
        {
            //Debug.LogWarning("Cast attempt");
            if (Grimoire.QuickSpell.CanCast(currentMana))
            {
                //Debug.LogWarning("I can cast this and have the mana");
                currentMana -= Grimoire.QuickSpell.manaCost;
                Vector2 dir = lookInput.normalized;
                Debug.Log(dir);
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion rot = Quaternion.Euler(0f, 0f, angle);
                Grimoire.QuickSpell.cast(rot, transform.position);
            }
            else
            {
                Debug.LogWarning("Not Enough Mana");
            }
            
        }
        else
        {
            Debug.LogWarning("Grimoire reference null");
        }
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
