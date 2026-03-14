using System.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class Ability : MonoBehaviour
{
    [SerializeField] public SpellData spelldetails;

    GameObject SpellPrefab;

    public int manaCost = 0;

    int level = 0;

    float damageMultiplier = 1f;

    float speedMultiplier = 1f;

    float lifetimeMultiplier = 1f;

    float CoolDown = 1f;

    bool castAvailable = true;

    int spellLayer;

    void loadValues()
    {
        if (spelldetails != null)
        {
            SpellPrefab = spelldetails.SpellPrefab;
            manaCost = spelldetails.manaCost;
            level = spelldetails.level;
            damageMultiplier = spelldetails.damageMultiplier;
            speedMultiplier = spelldetails.speedMultiplier;
            lifetimeMultiplier = spelldetails.lifetimeMultiplier;
            CoolDown = spelldetails.CoolDown;
        }
       
        else
        {
            Debug.LogWarning("Failed to load Spell Details as it is null...");
        }
    }
    public void ExecuteLoad(int _spellLayer)
    {
        spellLayer = _spellLayer;
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
    public enum upgrade { damageUP = 0, speedUP = 1, lifetimeUP = 2 , levelUP = 3, manaUP = 4};

    public void AbilityUpgrade (upgrade stat, float amount)
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
                    objScript.Recalculate(damageMultiplier, speedMultiplier, lifetimeMultiplier);
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
        return currMana >= manaCost;
    }

}