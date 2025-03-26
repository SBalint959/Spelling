using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells")]
public class SpellScriptableObject : ScriptableObject
{
    public float DamageAmount = 10f;
    public float Lifetime = 2f;
    public float Speed = 5f;
    public float SpellRadius = 0.5f;

    // Status effects
    // Cooldown
    // Magic elements
}
