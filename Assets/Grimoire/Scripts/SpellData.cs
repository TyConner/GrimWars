using UnityEngine;

[CreateAssetMenu]
public class SpellData : ScriptableObject
{
    [SerializeField] public GameObject SpellPrefab;

    [SerializeField] public int manaCost = 0;

    [SerializeField] public int level = 0;

    [SerializeField] public float damageMultiplier = 1f;

    [SerializeField] public float speedMultiplier = 1f;

    [SerializeField] public float lifetimeMultiplier = 1f;

    [SerializeField] public float CoolDown = 1f;
}

