using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, ITakeDamage, iUseItems
{
    [Header("Input System")]
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference firePrimaryAction;
    [SerializeField] InputActionReference lookAction;
    [SerializeField] InputActionReference fireSecondaryAction;

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
    [SerializeField] int maxMana = 100;
    int currentMana;
    [Header("Grimoire")]
    [SerializeField] grimoireSystem Grimoire;

    [Header("Player Feedback")]
    [SerializeField] float damageFlashTime = 0.08f;
    [SerializeField] float damageCameraShakeDuration = 0.2f;
    [SerializeField] float damageCameraShakeAmplitude = 0.02f;

    [Header("Fireball")]
    [SerializeField] float fireballLockTime = 0.45f;

    [SerializeField] Animator anim;

    [Header("UI")]
    [SerializeField] Image healthBar;
    [SerializeField] Image manaBar;
    [SerializeField] Image GrimoireSprite;
    [SerializeField] ButtonFunctions Screen;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip healing;
    [SerializeField] AudioClip damageTaken;
    [SerializeField] AudioClip pickupClip;
    [SerializeField] private AudioClip challengeRoomMusic;
    [SerializeField] private AudioClip dungeonMusic;

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
        currentMana = maxMana;
        originalColor = spr.color;
        GrimoireSprite.sprite = Grimoire.GetGrimoireSprite();
        UpdateHealthBar();
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
        if (fireSecondaryAction != null)
        {
            fireSecondaryAction.action.Enable();

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
        if (firePrimaryAction != null)
        {
            firePrimaryAction.action.Disable();
        }


        if (fireSecondaryAction != null)
        {
            fireSecondaryAction.action.Disable();
        }

        if (lookAction != null)
        {
            lookAction.action.Disable();
        }
    }

    void Update()
    {
        if (bDead) return;

        if (!bMovementLocked)
            //move
            if (moveAction == null)
            {
                moveInput = Vector2.zero;
                return;
            }


        moveInput = moveAction.action.ReadValue<Vector2>();


        if (moveInput.sqrMagnitude > 0f)
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
        //look

        if (firePrimaryAction != null && firePrimaryAction.action.WasPressedThisFrame())
        {
            //TryFireball();
            lookInput = lookAction.action.ReadValue<Vector2>();
            //attack
            Attack();

        }

        if (fireSecondaryAction != null && fireSecondaryAction.action.WasPressedThisFrame())
        {
            //TryFireball();
            lookInput = lookAction.action.ReadValue<Vector2>();

            //attack
            AttackHeavy();

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

        if (audioSource != null && damageTaken != null)
        {
            audioSource.PlayOneShot(damageTaken);
        }

        flashRoutine = StartCoroutine(DamageFlash());
        UpdateHealthBar();
    }

    public void Heal(int amount)
    {
        if (bDead) return;

        currentHP = Math.Clamp(currentHP + amount, 0, maxHP);
        UpdateHealthBar();

        if (audioSource != null && healing != null)
        {
            audioSource.PlayOneShot(healing);
        }

        StartCoroutine(HealFlash());
    }

    public void RestoreMana(int amount)
    {
        if (bDead) return;

        currentMana = Math.Clamp(currentMana + amount, 0, maxMana);
        UpdateManaBar();

        if (audioSource != null && healing != null)
        {
            audioSource.PlayOneShot(healing);
        }

        StartCoroutine(HealFlash());
    }

    public void Die()
    {
        rb.linearVelocity = Vector2.zero;
        spr.sprite = deathSprite;
        bDead = true;

        anim.SetBool("bIsMoving", false);
        anim.SetBool("bIsFireball", false);
        anim.SetBool("bIsDead", true);

        if (Screen != null)
            StartCoroutine(WaitForDeathAnimation(4.0f));
    }
    private IEnumerator WaitForDeathAnimation(float delay)
    {
        yield return new WaitForSeconds(delay); 
        Screen.GameOver();
    }

    public void Attack()
    {
        if (Grimoire != null)
        {
            //Debug.LogWarning("Cast attempt");
            if (Grimoire.GetQuick() == null)
            {
                return;
            }
            if (Grimoire.GetQuick().CanCast(currentMana))
            {
                //Debug.LogWarning("I can cast this and have the mana");
                currentMana -= Grimoire.GetQuick().manaCost;
                Vector3 playerpos = Camera.main.WorldToScreenPoint(transform.position);
                Vector2 playerScreenPos = new Vector2(playerpos.x, playerpos.y);
                Vector2 dir = lookInput - playerScreenPos;
                Debug.Log(dir);
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion rot = Quaternion.Euler(0f, 0f, angle);
                Grimoire.GetQuick().cast(rot, transform.position);
                UpdateManaBar();
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

    private void AttackHeavy()
    {
        if (Grimoire != null)
        {
            //Debug.LogWarning("Cast attempt");
            if (Grimoire.GetHeavy() == null)
            {
                return;
            }
            if (Grimoire.GetHeavy().CanCast(currentMana))
            {
                //Debug.LogWarning("I can cast this and have the mana");
                currentMana -= Grimoire.GetHeavy().manaCost;
                Vector3 playerpos = Camera.main.WorldToScreenPoint(transform.position);
                Vector2 playerScreenPos = new Vector2(playerpos.x, playerpos.y);
                Vector2 dir = lookInput - playerScreenPos;
                Debug.Log(dir);
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion rot = Quaternion.Euler(0f, 0f, angle);
                Grimoire.GetHeavy().cast(rot, transform.position);
                UpdateManaBar();
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

    IEnumerator HealFlash()
    {
        for (int i = 0; i < 3; i++)
        {
            spr.color = Color.green;
            yield return new WaitForSeconds(0.03f);
            spr.color = originalColor;
            yield return new WaitForSeconds(0.03f);
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHP / maxHP;
        }
    }

    void UpdateManaBar()
    {
        if (manaBar != null)
        {
            manaBar.fillAmount = (float)currentMana / maxMana;
        }
    }
    void iUseItems.GrimoirePickup(GrimoireClassData _grimoire)
    {
        Grimoire.SwapGrimoires(_grimoire);

        //USE THIS TO UPDATE UI!!!
        GrimoireSprite.sprite = Grimoire.GetGrimoireSprite();
    }

    void iUseItems.PagePickup()
    {
        Grimoire.PageAquired();
    }

    void iUseItems.HealthPotion(int _heal)
    {
        Heal(_heal);
    }

    void iUseItems.ManaPotion(int _mana)
    {
        throw new NotImplementedException();
    }

    public void PlayPickupSound()
    {
        if (audioSource != null && pickupClip != null)
        {
            audioSource.PlayOneShot(pickupClip);
        }
    }

    public void PlayChallengeRoomMusic()
    {
        if (audioSource != null && challengeRoomMusic != null)
        {
            audioSource.clip = challengeRoomMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void StopChallengeRoomMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();

            if (dungeonMusic != null)
            {
                audioSource.clip = dungeonMusic;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }
}