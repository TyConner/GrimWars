using UnityEngine;

[CreateAssetMenu]
public class SpellData : ScriptableObject
{
    [SerializeField] public GameObject SpellPrefab;

    [SerializeField] public int manaCost = 0;
    [SerializeField] public int MINmanaCost = 0;

    [SerializeField] public int Maxlevel = 5;

    [SerializeField] public float buffTimeMultiplier = 1f;
    [SerializeField] public float MAXbuffTimeMultiplier = 1f;

    [SerializeField] public float damageMultiplier = 1f;
    [SerializeField] public float MAXdamageMultiplier = 1f;

    [SerializeField] public float speedMultiplier = 1f;
    [SerializeField] public float MAXspeedMultiplier = 1f;

    [SerializeField] public float lifetimeMultiplier = 1f;
    [SerializeField] public float MAXlifetimeMultiplier = 1f;

    [SerializeField] public float AOEMultiplier = 1f;
    [SerializeField] public float MAXAOEMultiplier = 1f;

    [SerializeField] public float DOTRateMultiplier = 1f;
    [SerializeField] public float MINDOTRateMultiplier = 1f;

    [SerializeField] public float CoolDown = 1f;
    [SerializeField] public float MinCoolDown = 1f;
}

