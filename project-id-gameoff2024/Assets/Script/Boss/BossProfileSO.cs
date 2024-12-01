using UnityEngine;

[CreateAssetMenu(fileName = "Boss", menuName = "ScriptableObjects/Boss")]
public class BossProfileSO : ScriptableObject
{
    public string bossName;
    public float bossHealth;

    [Header("Attack Settings")]
    public float projectileDamage;
    public float bulletHellDamage;
    public float bossSpikeDamage;

    [Header("Summoning")]
    public float numberOfSpawns;
    public GameObject entityPrefab;
}
