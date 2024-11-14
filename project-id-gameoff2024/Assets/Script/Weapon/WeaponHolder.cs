using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public Weapon weapon;
    private BoxCollider boxCollider;
    private Rigidbody myBody;
    private Animator anim;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        myBody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
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
        Debug.LogError(value);
        transform.localEulerAngles = value;
    }
}
