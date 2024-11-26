using UnityEngine;
using System.Collections.Generic;


public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; set;}
    public List<Enemy> enemyAttackingList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        SwtichToCombatMode();
    }

    private void SwtichToCombatMode()
    {
        if(enemyAttackingList.Count > 0)
        {
            Debug.Log("Combat Music");
        }
        else
        {
            Debug.Log("Normal Music");
        }
    }
}
