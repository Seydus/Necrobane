using UnityEngine;

public class test : MonoBehaviour
{
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, 10f);
    }
}
