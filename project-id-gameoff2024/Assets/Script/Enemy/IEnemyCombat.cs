using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public interface IEnemyCombat
{
    public Enemy Enemy { get; set; }
    public float AttackDelay { get; set; }
    public float SphereRadius { get; set; }
    public float MaxDistance { get; set; }
    public LayerMask CombatLayer { get; set; }
    public float RotateSpeed { get; set; }
    public AK.Wwise.Event _HitPlayer { get; set; }

    public void Awake();
    public Ray GetEnemyDirection();
    public void HandleAttack(Transform player, NavMeshAgent navMeshAgent);
    public IEnumerator InitAttack(float delay);
    public void OnDrawGizmosSelected();
}
