using UnityEngine;
using static UnityEngine.UI.Image;

public class SpellScript : MonoBehaviour
{

    [SerializeField] SpriteRenderer spr;
    [SerializeField] Sprite spell;
    [SerializeField] Sprite spellhit;
    [SerializeField] AudioClip spellCastSFX;
    [SerializeField] AudioClip spellHitSFX;
    [Range(0.0f, 1f)][SerializeField] float SpellCastVol;
    [Range(0.0f, 1f)][SerializeField] float SpellHitVol;
    [SerializeField] float speed = 0;
    [SerializeField] float lifetime = .1f;
    [SerializeField] float dmgAmount;
    [SerializeField] bool DestroyAfterSeconds = false;
    [SerializeField] bool isBuff = false;
    [SerializeField] iSpellEffects.Buffs Buff;

    float dmgOrig;
    float speedOrig;
    float lifetimeOrig;

    private void Awake()
    {
        dmgOrig = dmgAmount;
        speedOrig = speed;
        lifetimeOrig = lifetime;
    }

    public void Recalculate(float dmgAdj, float spdAdj, float lifeAdj)
    {
        dmgAmount = dmgOrig * dmgAdj;
        speedOrig = speedOrig * spdAdj;
        lifetime = lifetimeOrig * lifeAdj;
    }

    public enum spellType
    {
        projectile, aoe, self
    }
    [SerializeField] spellType Type;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Type == spellType.projectile)
        {
            ITakeDamage damage = collision.GetComponent<ITakeDamage>();
            if (damage != null)
            {
                damage.TakeDamage((int)dmgAmount);

            }
            if (spellhit && spellHitSFX)
            {
                spr.sprite = spellhit;
                AudioSource.PlayClipAtPoint(spellHitSFX, transform.position, SpellHitVol);
            }
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (DestroyAfterSeconds)
        {
            Destroy(gameObject, lifetime);
        }
        if (spr && spell && spellCastSFX)
        {
            spr.sprite = spell;
            AudioSource.PlayClipAtPoint(spellCastSFX, transform.position, SpellCastVol);
            
        }
        if(GetComponent<Rigidbody2D>() != null)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = transform.right * speed;
        }
    }

}
