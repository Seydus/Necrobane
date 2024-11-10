using UnityEngine;

public class EnemyHolder : MonoBehaviour
{
    public EnemyProfile EnemyProfile { get; private set; }
    public EnemyRoaming EnemyRoaming { get; private set; }
    public EnemyCombat EnemyCombat { get; private set; }

    private void Awake()
    {
        EnemyProfile = GetComponent<EnemyProfile>();
        EnemyRoaming = GetComponent<EnemyRoaming>();
        EnemyCombat = GetComponent<EnemyCombat>();
    }

    private void Update()
    {
        EnemyRoaming.Init();
    }
}
