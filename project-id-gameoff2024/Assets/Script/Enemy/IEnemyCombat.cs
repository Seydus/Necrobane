using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public interface IEnemyCombat
{
    public Enemy Enemy { get; set; }
    public float AttackDelay { get; set; }
    public float RotateSpeed { get; set; }

    public void Awake();
    public Ray GetEnemyDirection();
    public void HandleAttack(Transform player, NavMeshAgent navMeshAgent);
    public IEnumerator InitAttack(Transform player, float delay);
}