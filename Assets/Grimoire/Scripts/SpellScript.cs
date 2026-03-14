using System.Collections;
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
    [SerializeField] float dmgRate = 1f;
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
    float dmgRateOrig;

    public int spellTargetLayer = 0;

  

    //timers

    float lifeTimer;
    float FXTimer;
    float dmgTimer;
    enum state
    {
        playing, markedfordeath
    }
    state mystate = state.playing;
    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if(Type == spellType.DOT)
        {
            dmgTimer += Time.deltaTime;
        }
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
            if(Type == spellType.DOT)
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
        if(Type == spellType.DOT && dmgTimer >= dmgRate)
        {
            dmgTimer = 0;
            HandleDOT();
            AudioSource.PlayClipAtPoint(spellHitSFX, transform.position, SpellHitVol);
        }
    }
    private void Awake()
    {
        dmgOrig = dmgAmount;
        speedOrig = speed;
        lifetimeOrig = lifetime;
        AOESizeOrig = AOESize;
        buffTimeOrig = BuffTime;
        dmgRateOrig = dmgRate;

    }

    public void Recalculate(float dmgAdj = 1f, float spdAdj = 1f, float lifeAdj = 1f, float AOEAdj = 1f, float BuffTimeAdj = 1f, float DOTAdj = 1f)
    {
        dmgAmount = dmgOrig * dmgAdj;
        speed = speedOrig * spdAdj;
        lifetime = lifetimeOrig * lifeAdj;
        AOESize = AOESizeOrig * AOEAdj;
        BuffTime = BuffTimeAdj * buffTimeOrig;
        dmgRate = dmgRateOrig * DOTAdj;
    }

  
    public enum spellType
    {
        projectile, aoe, self, DOT
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
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        mystate = state.markedfordeath;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, AOESize, spellTargetLayer);
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

    private void HandleDOT()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, AOESize, spellTargetLayer);
        foreach (Collider2D hit in hits)
        {
            if (isBuff)
            {
                iSpellEffects eff = hit.GetComponent<iSpellEffects>();
                if (eff != null)
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
       
    }

    private void Start()
    {
        if (Type == spellType.self)
        {
            iSpellEffects owner = transform.root.GetComponent<iSpellEffects>();
            if(owner != null)
            {
                owner.enactBuff(Buff, BuffTime, dmgAmount);
                mystate = state.markedfordeath;
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
        
        if(Type == spellType.DOT)
        {
            dmgTimer = dmgRate;
        }
    }

}
