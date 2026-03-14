using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;


public class grimoireSystem : MonoBehaviour
{
    [SerializeField] GrimoireClassData grimoire;

    Ability QuickSpell;

    Ability HeavySpell;

    private const int Level = 0;

    private const int Pages = 0;
    void Awake()
    {
        QuickSpell = this.AddComponent<Ability>();
        QuickSpell.spelldetails = grimoire.QuickSpell_Data;
        QuickSpell.ExecuteLoad();
        if(QuickSpell.spelldetails == null)
        {
            Debug.LogWarning("Failed to Load Data QuickSpell");
        }
        HeavySpell = this.AddComponent<Ability>();
        HeavySpell.spelldetails = grimoire.HeavySpell_Data;
        HeavySpell.ExecuteLoad();
        if (HeavySpell.spelldetails == null)
        {
            Debug.LogWarning("Failed to Load Data HeavySpell");
        }
    }

    public Ability GetQuick()
    {
        return QuickSpell;
    }

    public Ability GetHeavy()
    {
        return HeavySpell;
    }
}
