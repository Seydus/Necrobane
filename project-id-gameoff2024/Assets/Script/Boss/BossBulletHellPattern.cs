using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.AI;
using Unity.AI.Navigation;


public class BossBulletHellPattern : MonoBehaviour
{
    [Header("Bullet Hell")]
    [SerializeField] private Transform bulletHellPos;
    [SerializeField] private float bulletHellGap = 20f;
    [SerializeField] private float bulletHellDelaySpawn;
    [SerializeField] private GameObject bulletHellPrefab;

    private List<Vector3> bulletHellDirection = new List<Vector3>();

    private void Start()
    {
        GetBulletHellDirection();
    }

    private void GetBulletHellDirection()
    {
        float rotationY = 0;

        while (rotationY <= 360)
        {
            rotationY += bulletHellGap;
            bulletHellDirection.Add(new Vector3(0f, rotationY, 0f));
        }
    }

    public IEnumerator DoubleSequentialBulletHell()
    {
        for (int i = 0; i < bulletHellDirection.Count; i++)
        {
            GameObject bulletHellObj = Instantiate(bulletHellPrefab, bulletHellPos.position, Quaternion.Euler(bulletHellDirection[i]));
            GameObject bulletHellObjTwo = Instantiate(bulletHellPrefab, bulletHellPos.position, Quaternion.Euler(bulletHellDirection[(bulletHellDirection.Count - 1) - i]));
            bulletHellObj.GetComponent<BossBulletHellProjectile>().Init(bulletHellObj.transform.forward);
            bulletHellObj.GetComponent<BossBulletHellProjectile>().Init(bulletHellObj.transform.forward);
            yield return new WaitForSeconds(bulletHellDelaySpawn);
        }
    }

    public IEnumerator StartDominoBulletHell()
    {
        for (int i = 0; i < bulletHellDirection.Count; i++)
        {
            GameObject bulletHellObj = Instantiate(bulletHellPrefab, bulletHellPos.position, Quaternion.Euler(bulletHellDirection[i]));
            bulletHellObj.GetComponent<BossBulletHellProjectile>().Init(bulletHellObj.transform.forward);
            yield return new WaitForSeconds(bulletHellDelaySpawn);
        }
    }

    public IEnumerator StartReverseSequentialBulletHell()
    {
        for (int i = bulletHellDirection.Count - 1; i >= 0; i--)
        {
            GameObject bulletHellObj = Instantiate(bulletHellPrefab, bulletHellPos.position, Quaternion.Euler(bulletHellDirection[i]));
            yield return new WaitForSeconds(bulletHellDelaySpawn);
        }
    }

    public void StartSequentialBulletHell()
    {
        for (int i = 0; i < bulletHellDirection.Count; i++)
        {
            GameObject bulletHellObj = Instantiate(bulletHellPrefab, bulletHellPos.position, Quaternion.Euler(bulletHellDirection[i]));
            bulletHellObj.GetComponent<BossBulletHellProjectile>().Init(bulletHellObj.transform.forward);
        }
    }

}
