using System.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class Ability : MonoBehaviour
{
    [SerializeField] public SpellData spelldetails;

    GameObject SpellPrefab;

    public int manaCost = 0;

    int level = 0;
    int Maxlevel = 0;

    float bufftimeMultiplier = 1f;

    float damageMultiplier = 1f;

    float speedMultiplier = 1f;

    float lifetimeMultiplier = 1f;

    float AOEMultiplier = 1f;

    float DOTRateMultiplier = 1f;

    float CoolDown = 1f;

    bool castAvailable = true;

    int spellLayer;
    int spellTargetLayer;
    public void levelUP()
    {
        AbilityUpgrade(upgrade.levelUP, 1f);
        ScaleSpellonLVL();
    }

    float Scale(float a, float b)
    {
        float x = (float)level / Maxlevel;
        return Mathf.Lerp(a, b, x);
    }
    void ScaleSpellonLVL()
    {
        damageMultiplier = Scale(spelldetails.damageMultiplier, spelldetails.MAXdamageMultiplier);
        speedMultiplier = Scale(spelldetails.speedMultiplier, spelldetails.MAXspeedMultiplier); 
        lifetimeMultiplier = Scale(spelldetails.lifetimeMultiplier, spelldetails.MAXlifetimeMultiplier);
        AOEMultiplier  = Scale(spelldetails.AOEMultiplier, spelldetails.MAXAOEMultiplier);
        DOTRateMultiplier = Scale(spelldetails.MINDOTRateMultiplier, spelldetails.DOTRateMultiplier);
        bufftimeMultiplier = Scale(spelldetails.buffTimeMultiplier, spelldetails.MAXbuffTimeMultiplier);
        CoolDown = Scale(spelldetails.MinCoolDown, spelldetails.CoolDown);
        manaCost = (int)Scale((float)spelldetails.MINmanaCost, (float)spelldetails.manaCost);



    }
    void loadValues()
    {
        if (spelldetails != null)
        {
            SpellPrefab = spelldetails.SpellPrefab;
            manaCost = spelldetails.manaCost;
            Maxlevel = spelldetails.Maxlevel;
            damageMultiplier = spelldetails.damageMultiplier;
            speedMultiplier = spelldetails.speedMultiplier;
            bufftimeMultiplier = spelldetails.buffTimeMultiplier;
            lifetimeMultiplier = spelldetails.lifetimeMultiplier;
            AOEMultiplier = spelldetails.AOEMultiplier;
            DOTRateMultiplier = spelldetails.DOTRateMultiplier;
            CoolDown = spelldetails.CoolDown;
        }
       
        else
        {
            Debug.LogWarning("Failed to load Spell Details as it is null...");
        }
    }
    public void ExecuteLoad(int _spellLayer, int _spellTargetLayer)
    {
        spellLayer = _spellLayer;
        spellTargetLayer = _spellTargetLayer;
        loadValues();
    }
    IEnumerator CastTimeout()
    {
        if (castAvailable)
        {
            castAvailable = false;
            yield return new WaitForSeconds(CoolDown);
            castAvailable = true;
        }
    }
    public enum upgrade { damageUP = 0, speedUP = 1, lifetimeUP = 2 , AOEUP = 3, bufftimeUP =4 , DOTRateUP = 5, levelUP = 6, manaUP = 7};

    private void AbilityUpgrade (upgrade stat, float amount)
    {
        switch (stat)
        {
            case upgrade.damageUP:
                damageMultiplier += amount;
                break;
            case upgrade.speedUP:
                speedMultiplier += amount;
                break;
            case upgrade.lifetimeUP:
                lifetimeMultiplier += amount;
                break;
            case upgrade.bufftimeUP:
                bufftimeMultiplier += amount;
                break;
            case upgrade.AOEUP:
                AOEMultiplier += amount;
                break;
            case upgrade.DOTRateUP:
                DOTRateMultiplier += amount;
                break;
            case upgrade.levelUP:
                level += (int)amount;
                break;
            case upgrade.manaUP:
                manaCost += (int)amount;
                break;
        }
    }
    public void cast(Quaternion dir, Vector2 origin)
    {
        if (SpellPrefab)
        {
            //Debug.LogWarning("Casting");
            if (castAvailable)
            {
                StartCoroutine(CastTimeout());
                GameObject obj = Instantiate(SpellPrefab, origin, dir, transform.root);
                obj.layer = spellLayer;
                SpellScript objScript = obj.GetComponent<SpellScript>();
                if (objScript != null)
                {
                    objScript.Recalculate(damageMultiplier, speedMultiplier, lifetimeMultiplier, AOEMultiplier, bufftimeMultiplier, DOTRateMultiplier);
                    objScript.spellTargetLayer = 1 << spellTargetLayer;
                }
            }
           
        }
        else
        {
            Debug.LogWarning("CastFailed unexpectedly no spell prefab ");
        }
    }

    public bool CanCast(int currMana)
    {
        return currMana >= manaCost && castAvailable;
    }

}