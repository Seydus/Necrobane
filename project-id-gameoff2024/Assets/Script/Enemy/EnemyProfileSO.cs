using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy")]

public class EnemyProfileSO : ScriptableObject
{
    [Header("Enemy Profile")]
    public string EnemyName;
    public float EnemyHealth;

    [Header("Enemy Combat Profile")]
    public float EnemyDamage;
    public float EnemyRange;

    [Header("Enemy Item Drop")]
    public GameObject itemDrop;
}
