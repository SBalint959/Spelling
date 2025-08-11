using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells")]
public class SpellScriptableObject : ScriptableObject
{
    public float DamageAmount = 10f;
    public float Lifetime = 2f;
    public float Speed = 5f;
    public float SpellRadius = 0.5f;
    public string SpellType = "";

    public string SpellElement = "";

    public bool applyKnockback = false;
    public float knockbackForce = 2f;
    public bool applySlow = false;
    public bool passThrough = false;
    


    // Status effects
    // Cooldown
    // Magic elements
}
