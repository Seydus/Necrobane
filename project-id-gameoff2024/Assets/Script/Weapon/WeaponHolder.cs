using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [HideInInspector] public Weapon weapon;
    private BoxCollider boxCollider;
    private Rigidbody myBody;
    private Animator anim;
    public GameObject weaponbObj;
    
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        myBody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        weapon = GetComponent<Weapon>();
    }

    public void SetMeshState(bool state)
    {
        weaponbObj.SetActive(state);
    }

    public void SetBoxCollider(bool state)
    {
        boxCollider.enabled = state;
    }

    public void SetRigidbodyKinematic(bool state)
    {
        myBody.isKinematic = state;
    }

    public Animator GetAnimator()
    {
        return anim;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetWeaponParent(Transform transform)
    {
        transform.SetParent(transform);
    }

    public void SetRotation(Vector3 value)
    {
        transform.localEulerAngles = value;
    }
}
