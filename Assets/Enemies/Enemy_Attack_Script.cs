using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class Enemy_Attack_Script : MonoBehaviour
{

    [SerializeField] CapsuleCollider2D DamageZone;
    [SerializeField] SpriteRenderer Spr;
    [SerializeField] GameObject effect;
    [SerializeField] float effectkilltime = .4f;
    [SerializeField] float hitboxtime = 0.2f;
    public float AttackRate = 0.5f;
    public int damage;
    int randVal;
    bool bIsAttacking = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    IEnumerator AttackTimeOut()
    {
      
        bIsAttacking = true;
        
        yield return new WaitForSeconds(AttackRate);
        bIsAttacking = false;
        
    }

    IEnumerator Effects()
    {
        spawnSwipeTrail(effectkilltime);
        Spr.enabled = true;
        DamageZone.enabled = true;
        yield return new WaitForSeconds(hitboxtime);
        DamageZone.enabled = false;
        Spr.enabled = false;
    }
    void randflip(SpriteRenderer spr)
    {
        if (spr)
        {
            spr.flipX = Random.Range(0, 2) == 0;
        }
          
    }
    void spawnSwipeTrail(float killtime)
    {
        //Debug.Log("SPAWNED EFFECT");
        if (effect != null)
        {
            GameObject obj = Instantiate(effect, transform.position, transform.rotation);
            obj.transform.localScale = this.transform.localScale;
            SpriteRenderer spr = obj.GetComponent<SpriteRenderer>();
            if (spr != null)
            {
                spr.flipX = Spr.flipX;
            }
            ;
            Destroy(obj, killtime);
        }
    }
    public void Attack()
    {
        if (!bIsAttacking)
        {
            StartCoroutine(AttackTimeOut());
            randflip(Spr);
            StartCoroutine(Effects());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            ITakeDamage dmg = other.GetComponent<ITakeDamage>();
            if(dmg != null)
            {
                dmg.TakeDamage(damage);
            }
        }
    }

}
