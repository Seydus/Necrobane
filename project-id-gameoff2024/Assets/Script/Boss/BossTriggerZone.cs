using UnityEngine;

public class BossTriggerZone : MonoBehaviour
{
    private bool isTriggerd;

    private void OnTriggerEnter(Collider other)
    {
        if(isTriggerd == false && other.tag == "Player")
        {
            GameManager.Instance.isBossFight = true;
            isTriggerd = true;
        }
    }
}
