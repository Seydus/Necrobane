using UnityEngine;

public class BossTriggerZone : MonoBehaviour
{
    private bool isTriggerd;
    [SerializeField] private BossController bossController;

    private void OnTriggerEnter(Collider other)
    {
        if(isTriggerd == false && other.tag == "Player")
        {
            bossController.State();
            GameManager.Instance.isBossFight = true;
            isTriggerd = true;
        }
    }
}
