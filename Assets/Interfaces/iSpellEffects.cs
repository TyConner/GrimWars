using UnityEngine;

public interface iSpellEffects
{
   public enum Buffs
    {
        healthUP = 0, manaUP = 1, damageUP = 2, speedUP = 3, HealDOT = 4, ManaDOT = 5
    }

    void enactBuff(Buffs buff, float time)
    {

    }
}
