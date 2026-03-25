using System;
using UnityEngine;

public class SkeletonEnemy : MonoBehaviour, ITakeDamage
{
    [Header("Health")]
    [SerializeField] private int maxHP = 3;
    public int currentHP;

    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stopDistance = 5f;

    [Header("Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireCooldown = 2f;
    [SerializeField] private float projectileSpeed = 5f;

    private Transform player;
    private Rigidbody2D rb;
    private float fireTimer;

    public static event Action<SkeletonEnemy> OnEnemyKilled;

    void Awake()
    {
        currentHP = maxHP;
        player = GameObject.FindWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            return;
        }

        fireTimer += Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);

        if (fireTimer >= fireCooldown && projectilePrefab != null && distance <= stopDistance)
        {
            ShootAtPlayer();
            fireTimer = 0f;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            rb.linearVelocity = direction * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void ShootAtPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
        if (projRb != null)
        {
            projRb.linearVelocity = dir * projectileSpeed;
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void TakeDamage(int amount)
    {
        currentHP = Math.Clamp(currentHP - amount, 0, maxHP);

        if (currentHP <= 0)
        {
            Death();
        }
    }

    public void Heal(int amount)
    {
        currentHP = Math.Clamp(currentHP + amount, 0, maxHP);
    }

    public void Death()
    {
        OnEnemyKilled?.Invoke(this);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.TakeDamage(1);
        }
    }
}