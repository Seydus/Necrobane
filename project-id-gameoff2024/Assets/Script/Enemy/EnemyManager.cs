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
        if(enemyAttackingList.Count >= 1)
        {
            Debug.Log("Combat Music");
            AkSoundEngine.SetSwitch("Music","Combat", gameObject);
        }
        else
        {
            Debug.Log("Normal Music");
            AkSoundEngine.SetSwitch("Music", "Exploration", gameObject);
        }
    }
}
