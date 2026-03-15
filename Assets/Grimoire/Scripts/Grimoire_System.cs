using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


public class grimoireSystem : MonoBehaviour
{
    [SerializeField] GrimoireClassData grimoire;

    int SpellPhysicsLayer = 0;

    int SpellTargetLayer = 0;

    Ability QuickSpell;

    Ability HeavySpell;

    private const int Level = 0;

    private const int Pages = 0;
    void getSpellLayer()
    {
        int parentlayer = transform.root.gameObject.layer;
        switch (parentlayer)
        {
            case 8: //player
                SpellPhysicsLayer = 6;
                SpellTargetLayer = 9;
                break;
            case 9: //enemy
                SpellPhysicsLayer = 7;
                SpellTargetLayer = 8;
                break;
            default:
                SpellPhysicsLayer = parentlayer;
                break;
        }   
    }
    void Awake()
    {
        getSpellLayer();
        QuickSpell = this.AddComponent<Ability>();
        QuickSpell.spelldetails = grimoire.QuickSpell_Data;
        QuickSpell.ExecuteLoad(SpellPhysicsLayer, SpellTargetLayer);
        if(QuickSpell.spelldetails == null)
        {
            Debug.LogWarning("Failed to Load Data QuickSpell");
        }
        HeavySpell = this.AddComponent<Ability>();
        HeavySpell.spelldetails = grimoire.HeavySpell_Data;
        HeavySpell.ExecuteLoad(SpellPhysicsLayer, SpellTargetLayer);
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
