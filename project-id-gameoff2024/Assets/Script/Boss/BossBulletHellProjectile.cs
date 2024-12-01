using UnityEngine;

public class BossBulletHellProjectile : MonoBehaviour
{
    public float projectileSpeed { get; set; }
    [SerializeField] private float projectileDamage;
    private Vector3 direction;
    private float lifeSpan;

    private Rigidbody myBody;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();

        lifeSpan = 5f;
    }

    private void FixedUpdate()
    {
        myBody.AddRelativeForce(direction * projectileSpeed, ForceMode.Acceleration);
    }

    public void Init(Vector3 direction)
    {
        this.direction = direction;
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
        if (other.tag == "Player")
        {
            if (other.transform.GetComponent<PlayerManager>().PlayerProfile.isDefending)
            {
                other.transform.GetComponent<PlayerManager>().PlayerProfile.DeductStamina(other.transform.GetComponent<PlayerManager>().PlayerCombat.WeaponHolder.weapon.weaponData.WeaponStaminaCost);
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
                other.transform.GetComponent<PlayerManager>().PlayerProfile.DeductHealth(projectileDamage);
            }

            AkSoundEngine.PostEvent("Play_Firebolt_Explosion", gameObject);
        }

        if (other.tag == "Obstacle" || other.tag == "Door" || other.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
