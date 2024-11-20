using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public interface IEnemyCombat
{
    public Enemy Enemy { get; set; }
    public float RotateSpeed { get; set; }
    public bool IsAttacking { get; set; }

    public void Awake();
    public Ray GetEnemyDirection();
    public void HandleAttack(Transform player, NavMeshAgent navMeshAgent, float range);
    public void InitAttack();
}
