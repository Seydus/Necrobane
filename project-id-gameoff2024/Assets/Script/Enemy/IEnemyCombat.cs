using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public interface IEnemyCombat
{
    public Enemy Enemy { get; set; }
    public float AttackSpeed { get; set; }
    public float RotateSpeed { get; set; }

    public void Awake();
    public Ray GetEnemyDirection();
    public void HandleAttack(Transform player, NavMeshAgent navMeshAgent, float range);
    public IEnumerator InitAttack(Transform player, float delay);
}
