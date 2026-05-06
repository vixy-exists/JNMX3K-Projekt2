using UnityEngine;

public static class DamageCalculator
{
    /// <summary>
    /// Calculates new HP after applying the skill damage formula:
    /// newHP = CurrentHP - ((Strength * 1.5 + Speed * 0.5) * SkillPower - Defense * 0.75)
    /// SkillPower is provided as parameter (user requested it to be 1).
    /// Result is rounded to nearest integer and clamped to a minimum of 0.
    /// </summary>
    public static int CalculateNewHP(int currentHP, int strength, int speed, int defense, int skillPower)
    {
        float damage = (strength * 1.5f + speed * 0.5f) * skillPower - defense * 0.75f;

        // If damage is negative, treat as zero (no healing from attack)
        if (damage < 0f)
            damage = 0f;

        int dmgInt = Mathf.RoundToInt(damage);
        int newHP = currentHP - dmgInt;
        if (newHP < 0) newHP = 0;
        return newHP;
    }
}
