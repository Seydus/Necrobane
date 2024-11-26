using UnityEngine;

public class BossSpikeAttack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.transform.GetComponent<PlayerProfile>().DeductHealth(20f);
        }
    }
}
