using UnityEngine;

[CreateAssetMenu(fileName = "GameBalance")]
public class GameBalance : ScriptableObject
{
    [Header("Player")]
    public float MoveSpeed;
    public float HealSpeed;
    public float SuitStrength;

    [Header("Zombie")]
    public float Damage;
    public float BiteCooldown;
}