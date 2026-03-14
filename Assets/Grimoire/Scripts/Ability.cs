using UnityEngine;

[CreateAssetMenu]
public class Ability : ScriptableObject
{
    
    [SerializeField] GameObject SpellPrefab;

    [SerializeField] SpellScript Spell;
 
    [SerializeField] int manaCost = 0;

    [SerializeField] int level = 0;

    [SerializeField] float damageMultiplier = 1f;

    [SerializeField] float speedMultiplier = 1f;

    [SerializeField] float lifetimeMultiplier = 1f; 

    public void cast(Quaternion dir, Vector2 origin)
    {
        if (Spell)
        {
            Spell.Recalculate(damageMultiplier, speedMultiplier, lifetimeMultiplier);
            Instantiate(SpellPrefab, origin, dir);
        }
        
    }

    private void Awake()
    {
        if (SpellPrefab)
        {
            SpellScript reference = SpellPrefab.GetComponent<SpellScript>();
            if (reference != null)
            {
                Spell = reference;
            }
        }
    }
    public bool CanCast(int currMana)
    {
        return currMana >= manaCost;
    }

}