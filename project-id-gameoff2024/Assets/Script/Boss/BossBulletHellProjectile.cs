using UnityEngine;

public class BossBulletHellProjectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileDamage;
    private Vector3 direction;
    private float lifeSpan;

    private Rigidbody myBody;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();

        lifeSpan = 3f;
    }

    private void FixedUpdate()
    {
        myBody.AddRelativeForce(transform.localPosition * projectileSpeed, ForceMode.Acceleration);
    }

    private void Update()
    {
        if (lifeSpan <= 0)
        {
            Destroy(gameObject);
        }

        lifeSpan -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {

    }
}
