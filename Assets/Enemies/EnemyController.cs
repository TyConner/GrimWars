using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyController : MonoBehaviour, ITakeDamage
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float acceleration = 60f;
    [SerializeField] float deceleration = 80f;
    [SerializeField] float SensoryRadius = 5;
    [SerializeField] int Damage = 1;
    [SerializeField] float AttackRate = 0.5f;
    [SerializeField] CircleCollider2D SensoryRange;
    [SerializeField] Transform Pivot;
    [SerializeField] Enemy_Attack_Script attack_script;
    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer Spr;
    [SerializeField] Rigidbody2D rb;


    Vector2 moveInput;
    Vector2 lookInput;
    Vector2 desiredVelocity;
    Vector2 velSmoothRef;
    bool bDead = false;

    [SerializeField] int maxHP = 1;
    public int currentHP;

    public void Heal(int amount)
    {
        currentHP = Math.Clamp(currentHP+amount, 0, maxHP);
    }

    public void TakeDamage(int amount)
    {
        if(bDead) return;
        currentHP = Math.Clamp(currentHP - amount, 0, maxHP);
        if(currentHP <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        bDead = true;
        anim.SetBool("bIsDead", true);
        Pivot.gameObject.SetActive(false);
        Destroy(gameObject, 3f);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
        SensoryRange.radius = SensoryRadius;
        if (attack_script != null)
        {
            attack_script.damage = Damage;
            attack_script.AttackRate = AttackRate;
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (bDead)
        {
            desiredVelocity = Vector2.zero;
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


    void movement(Vector2 dir)
    {
        if (bDead) return;
        if (dir.sqrMagnitude > 0f)
        {
           

                if (dir.sqrMagnitude > 0f)
                {
                    moveInput = dir.normalized;
                   if(anim.runtimeAnimatorController != null)
                {
                    anim.SetFloat("LastInputX", moveInput.x);
                    anim.SetFloat("LastInputY", moveInput.y);
                    anim.SetBool("bIsMoving", true);
                }
                    

                    if (dir.x < 0f)
                    {
                    Spr.flipX = true;
                    }
                    else if (dir.x > 0f)
                    {
                    Spr.flipX = false;
                    }
                }
                else
                {
                if (anim.runtimeAnimatorController != null)
                {
                    anim.SetBool("bIsMoving", false);
                }
                }
          
        }
        else
        {
            if (anim.runtimeAnimatorController != null)
            {
                anim.SetBool("bIsMoving", false);
            }
        }
    }
void look(Vector2 dir)
    {
        if (bDead) return;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle+270);
        Pivot.rotation = rot;
    }
     
private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger)
        {
            return;
        }
        if ( collision.CompareTag("Player"))
        {
            if (attack_script != null)
            {
                Vector2 dir = collision.transform.position - transform.position;
                attack_script.Attack();
                look(dir);
                movement(dir);
            }
        }
        
    }

  
}
