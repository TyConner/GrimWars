using System.Collections;
using UnityEngine;

public class SlimeEnemy : MonoBehaviour, ITakeDamage
{
    [Header("Stats")]
    [SerializeField] private int maxHP = 3;
    public int currentHP;
    [SerializeField] private int damage = 1;

    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Splitting")]
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private Vector2 spawnOffset = new Vector2(0.5f, 0);
    private bool hasSplit = false;

    private Transform player;
    private bool attacking = false;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (GetComponent<Collider2D>() == null)
            gameObject.AddComponent<CircleCollider2D>();
    }

    void Start()
    {
        currentHP = maxHP;
        player = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackDistance && !attacking)
        {
            StartCoroutine(Attack());
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        Vector2 direction = ((Vector2)player.position - rb.position).normalized;

        if (dist > attackDistance)
        {
            rb.linearVelocity = direction * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private IEnumerator Attack()
    {
        attacking = true;

        ITakeDamage dmg = player.GetComponent<ITakeDamage>();

        while (dmg != null &&
               Vector2.Distance(transform.position, player.position) <= attackDistance)
        {
            rb.linearVelocity = Vector2.zero;
            dmg.TakeDamage(damage);

            yield return new WaitForSeconds(attackCooldown);
        }

        attacking = false;
    }

    public void TakeDamage(int amount)
    {
        currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);

        if (currentHP <= 0)
            Death();
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
    }

    private void Death()
    {
        if (!hasSplit && slimePrefab != null)
        {
            SpawnSplit(spawnOffset);
            SpawnSplit(-spawnOffset);
            hasSplit = true;
        }

        Destroy(gameObject);
    }

    private void SpawnSplit(Vector2 offset)
    {
        GameObject newSlime = Instantiate(slimePrefab, (Vector2)transform.position + offset, Quaternion.identity);

        SlimeEnemy slimeComp = newSlime.GetComponent<SlimeEnemy>();
        if (slimeComp != null)
        {
            slimeComp.hasSplit = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.TakeDamage(1);
        }
    }
}