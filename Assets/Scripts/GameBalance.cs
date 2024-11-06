using UnityEngine;

[CreateAssetMenu(fileName = "GameBalance")]
public class GameBalance : ScriptableObject
{
    [Header("Player")]
    public float MoveSpeed;
    public float HealSpeed;
    public float SuitStrength;
    public float SavingSpeed;

    [Header("Zombie")]
    public float Damage;
    public float BiteCooldown;
}