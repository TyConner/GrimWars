using UnityEngine;

[CreateAssetMenu]
public class Ability : ScriptableObject
{
    
    [SerializeField] GameObject SpellPrefab;

    [SerializeField]public int manaCost = 0;

    [SerializeField] int level = 0;

    [SerializeField] float damageMultiplier = 1f;

    [SerializeField] float speedMultiplier = 1f;

    [SerializeField] float lifetimeMultiplier = 1f;

    public void cast(Quaternion dir, Vector2 origin)
    {
        if (SpellPrefab)
        {
            //Debug.LogWarning("Casting");

            GameObject obj = Instantiate(SpellPrefab, origin, dir);
            SpellScript objScript = obj.GetComponent<SpellScript>();
            if (objScript != null)
            {
                objScript.Recalculate(damageMultiplier, speedMultiplier, lifetimeMultiplier);
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