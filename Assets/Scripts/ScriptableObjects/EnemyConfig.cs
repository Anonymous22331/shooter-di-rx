using UnityEngine;

[CreateAssetMenu(menuName = "Configs/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [SerializeField] private int maxHp;
    [SerializeField] private float respawnTime;
    [SerializeField] private int maxEnemiesOnLevel;
    [SerializeField] private float moveSpeed;

    public int MaxHp => maxHp;
    public float RespawnTime => respawnTime;
    public int MaxEnemiesOnLevel => maxEnemiesOnLevel;
    public float MoveSpeed => moveSpeed;
}