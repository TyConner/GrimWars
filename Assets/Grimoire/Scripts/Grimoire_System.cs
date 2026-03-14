using UnityEngine;


public class grimoireSystem : MonoBehaviour
{
    public enum Element { Fire, Ice, Aero, Light, Dark }
    
    private const int Level = 0;

    private const int Pages = 0;

    [SerializeField] public Element Type;

    [SerializeField] public Ability QuickSpell;

    [SerializeField] public Ability HeavySpell;

}
