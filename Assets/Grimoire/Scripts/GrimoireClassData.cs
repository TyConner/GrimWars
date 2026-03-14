using UnityEngine;

[CreateAssetMenu]
public class GrimoireClassData : ScriptableObject
{
    public enum Element { Fire, Ice, Aero, Light, Dark }

    [SerializeField] public Element Type;

    [SerializeField] public SpellData QuickSpell_Data;
    [SerializeField] public SpellData HeavySpell_Data;

}

