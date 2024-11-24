using Unity.VisualScripting;
using UnityEngine;

public class EnemyProjectileBullet : MonoBehaviour
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
        myBody.AddRelativeForce(direction * projectileSpeed, ForceMode.Acceleration);
    }

    private void Update()
    {
        if(lifeSpan <= 0)
        {
            Destroy(gameObject);
        }

        lifeSpan -= Time.deltaTime;
    }

    public void Init(Vector3 direction)
    {
        this.direction = direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.transform.GetComponent<PlayerProfile>().DeductHealth(projectileDamage);
            AkSoundEngine.PostEvent("Play_Firebolt_Explosion", gameObject);
        }

        if(other.CompareTag("Enemy") == false)
        {
            Destroy(gameObject);
        }
    }
}
