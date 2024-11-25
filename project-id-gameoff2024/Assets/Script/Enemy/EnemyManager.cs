using UnityEngine;
using System.Collections.Generic;


public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; set;}
    public List<Enemy> enemyList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
