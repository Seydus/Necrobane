using UnityEngine;

public class EnemyProjectileBullet : MonoBehaviour
{
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileDamage;
    private float lifeSpan;

    private Rigidbody myBody;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();

        lifeSpan = 10f;
    }

    private void Update()
    {
        if(lifeSpan <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Init(Vector3 direction)
    {
        myBody.AddRelativeForce(direction * projectileSpeed, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            collision.transform.GetComponent<PlayerProfile>().DeductHealth(projectileDamage);
            Destroy(gameObject);
        }
    }
}
