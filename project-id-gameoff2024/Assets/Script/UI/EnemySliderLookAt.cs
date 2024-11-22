using UnityEngine;

public class EnemySliderLookAt : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        LookAtPlayer();
    }

    void LookAtPlayer()
    {

        transform.LookAt(player.transform);
    }
}
