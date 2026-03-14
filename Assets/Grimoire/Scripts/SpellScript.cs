using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] float FXTime = .04f;
    [SerializeField] float dmgAmount = 1f;
    [SerializeField] float AOESize = 0f;
    [SerializeField] bool DestroyAfterSeconds = false;
    [SerializeField] bool isBuff = false;
    [SerializeField] float BuffTime = 0f;
    [SerializeField] bool explodeOnImpact = false;
    [SerializeField] iSpellEffects.Buffs Buff;

    float dmgOrig;
    float speedOrig;
    float lifetimeOrig;
    float AOESizeOrig;
    float buffTimeOrig;

    //timers

    float lifeTimer;
    float FXTimer;
    enum state
    {
        playing, markedfordeath
    }
    state mystate = state.playing;
    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if(lifeTimer >= lifetime && DestroyAfterSeconds && mystate == state.playing)
        {
            if(Type == spellType.aoe)
            {
                HandleAOE();
                mystate = state.markedfordeath;
            }
            if (Type == spellType.projectile)
            {
                mystate = state.markedfordeath;
            }
        }
        if (mystate == state.markedfordeath)
        {
            FXTimer += Time.deltaTime;
            if (FXTimer >= FXTime)
            {
                FXTimer = 0;
                Destroy(gameObject);
            }
        }
    }
    private void Awake()
    {
        dmgOrig = dmgAmount;
        speedOrig = speed;
        lifetimeOrig = lifetime;
        AOESizeOrig = AOESize;
        buffTimeOrig = BuffTime;
    }

    public void Recalculate(float dmgAdj = 1f, float spdAdj = 1f, float lifeAdj = 1f, float AOEAdj = 1f, float BuffTimeAdj = 1f)
    {
        dmgAmount = dmgOrig * dmgAdj;
        speed = speedOrig * spdAdj;
        lifetime = lifetimeOrig * lifeAdj;
        AOESize = AOESizeOrig * AOEAdj;
        BuffTime = BuffTimeAdj * buffTimeOrig;
    }

    public enum spellType
    {
        projectile, aoe, self
    }
    [SerializeField] spellType Type;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger || mystate != state.playing)
        {
            return;
        }
        if(mystate == state.playing)
        {
            if (Type == spellType.projectile)
            {
                if (explodeOnImpact)
                {
                    HandleAOE();
                }
                else
                {
                    HandleHit(collision);
                }


            }
        }
    }


    private void PlayHitFX()
    {
        if (spellhit && spellHitSFX)
        {
            spr.sprite = spellhit;
            AudioSource.PlayClipAtPoint(spellHitSFX, transform.position, SpellHitVol);
        }
    }
    private void HandleHit(Collider2D col)
    {
        mystate = state.markedfordeath;
        ITakeDamage damage = col.GetComponent<ITakeDamage>();
        if (damage != null)
        {
            damage.TakeDamage((int)dmgAmount);

        }
        PlayHitFX();
    }
    private void HandleAOE()
    {
        mystate = state.markedfordeath;
        LayerMask mask = ~(1 << gameObject.layer);
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, AOESize, mask);
        foreach (Collider2D hit in hits)
        {
            if (isBuff)
            {
                iSpellEffects eff = hit.GetComponent<iSpellEffects>();
                if(eff != null)
                {
                    eff.enactBuff(Buff, BuffTime, dmgAmount);
                }
            }
            else
            {
                ITakeDamage dmg = hit.GetComponent<ITakeDamage>();
                if (dmg != null)
                {
                    dmg.TakeDamage((int)dmgAmount);
                }
          
            }
        }
        PlayHitFX();
    }
    private void Start()
    {
        if (Type == spellType.self)
        {
            iSpellEffects owner = transform.root.GetComponent<iSpellEffects>();
            if(owner != null)
            {
                owner.enactBuff(Buff, BuffTime, dmgAmount);
            }
        }
    
        if (spr && spell && spellCastSFX)
        {
            spr.sprite = spell;
            AudioSource.PlayClipAtPoint(spellCastSFX, transform.position, SpellCastVol);
            
        }
        if(Type == spellType.projectile)
        {
            if (GetComponent<Rigidbody2D>() != null)
            {
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                rb.linearVelocity = transform.right * speed;
            }
        }

        if(Type == spellType.aoe)
        {
            HandleAOE();

        }
        
    }

}
