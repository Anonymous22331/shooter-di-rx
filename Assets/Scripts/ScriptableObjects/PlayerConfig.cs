using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackSpeed;
    [SerializeField] private int damage;
    [SerializeField] private float attackRadius;

    public float MoveSpeed => moveSpeed;
    public float AttackSpeed => attackSpeed;
    public int Damage => damage;
    public float AttackRadius => attackRadius;
}