using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


public class grimoireSystem : MonoBehaviour
{
    [SerializeField] GrimoireClassData grimoire;

    int SpellPhysicsLayer = 0;

    Ability QuickSpell;

    Ability HeavySpell;

    private const int Level = 0;

    private const int Pages = 0;
    void getSpellLayer()
    {
        string parentlayer = transform.root.gameObject.layer.ToString();
      if (parentlayer == "Player")
        {
            SpellPhysicsLayer = 6;
        }
      if (parentlayer == "Enemy")
        {
            SpellPhysicsLayer = 7;
        }
    }
    void Awake()
    {
        getSpellLayer();
        QuickSpell = this.AddComponent<Ability>();
        QuickSpell.spelldetails = grimoire.QuickSpell_Data;
        QuickSpell.ExecuteLoad(SpellPhysicsLayer);
        if(QuickSpell.spelldetails == null)
        {
            Debug.LogWarning("Failed to Load Data QuickSpell");
        }
        HeavySpell = this.AddComponent<Ability>();
        HeavySpell.spelldetails = grimoire.HeavySpell_Data;
        HeavySpell.ExecuteLoad(SpellPhysicsLayer);
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
