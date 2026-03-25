using System.Collections;
using UnityEngine;

public class ScorpionEnemy : MonoBehaviour, ITakeDamage
{
    [Header("Health")]
    [SerializeField] private int maxHP = 3;
    public int currentHP;

    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float attackDistance = 1.5f;

    [Header("Attack")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Poison")]
    [SerializeField] private float poisonDuration = 3f;
    [SerializeField] private float poisonTickRate = 1f;
    [SerializeField] private int poisonDamage = 1;

    private Transform player;
    private Rigidbody2D rb;

    private Coroutine poisonRoutine;
    private Coroutine attackRoutine;

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

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackDistance)
        {
            if (attackRoutine == null)
                attackRoutine = StartCoroutine(AttackCoroutine());
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        if (distance > attackDistance)
        {
            rb.linearVelocity = direction * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private IEnumerator AttackCoroutine()
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        ITakeDamage dmg = player.GetComponent<ITakeDamage>();

        while (pc != null && dmg != null &&
               Vector2.Distance(transform.position, player.position) <= attackDistance)
        {
            rb.linearVelocity = Vector2.zero;

            dmg.TakeDamage(damage);

            if (poisonRoutine != null)
                StopCoroutine(poisonRoutine);

            poisonRoutine = StartCoroutine(PoisonCoroutine(pc));

            yield return new WaitForSeconds(attackCooldown);
        }

        attackRoutine = null;
    }

    private IEnumerator PoisonCoroutine(PlayerController pc)
    {
        float timer = 0f;

        while (timer < poisonDuration)
        {
            pc.TakeDamage(poisonDamage);
            yield return new WaitForSeconds(poisonTickRate);
            timer += poisonTickRate;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);

        if (currentHP <= 0)
        {
            Death();
        }
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}