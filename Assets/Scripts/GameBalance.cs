using UnityEngine;

[CreateAssetMenu(fileName = "GameBalance")]
public class GameBalance : ScriptableObject
{
    [Header("Player")]
    public float MoveSpeed;
    public float HealSpeed;
}