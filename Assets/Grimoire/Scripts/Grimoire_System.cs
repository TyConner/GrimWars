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

    public int Level = 0;

    public int Pages = 0;

    public void PageAquired()
    {
        Pages = (int)Mathf.Clamp(Pages + 1,0, grimoire.MaxPages);
    }

    public void LevelUP()
    {
        Level = (int)Mathf.Clamp(Level + 1, 0, grimoire.MaxLevel);
    }

    public void SwapGrimoires(GrimoireClassData newGrimoire)
    {
        grimoire = newGrimoire;
        LoadQuickSpell();
        LoadHeavySpell();

    }
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
        LoadQuickSpell();
        HeavySpell = this.AddComponent<Ability>();
        LoadHeavySpell();
    }

    private void LoadQuickSpell()
    {
        QuickSpell.spelldetails = grimoire.QuickSpell_Data;
        QuickSpell.ExecuteLoad(SpellPhysicsLayer, SpellTargetLayer);
        if (QuickSpell.spelldetails == null)
        {
            Debug.LogWarning("Failed to Load Data QuickSpell");
        }
    }

    private void LoadHeavySpell()
    {
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

    public  Sprite GetGrimoireSprite() {
        return grimoire.Grimoire_Sprite;
    }
}
