using System;
using UnityEngine;

public class EnemyController : MonoBehaviour, ITakeDamage
{
    [SerializeField] int maxHP = 1;
    public int currentHP;

    public void Heal(int amount)
    {
        currentHP = Math.Clamp(currentHP+amount, 0, maxHP);
    }

    public void TakeDamage(int amount)
    {
        currentHP = Math.Clamp(currentHP - amount, 0, maxHP);
        if(currentHP <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        ITakeDamage dmg = other.GetComponent<ITakeDamage>();
        if (dmg != null && other.GetComponent<PlayerController>() != null)
        {
            dmg.TakeDamage(1);
        }
    }
}
