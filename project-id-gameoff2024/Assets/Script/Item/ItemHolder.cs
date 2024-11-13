using UnityEngine;
using UnityEngine.Timeline;

public class ItemHolder : MonoBehaviour
{
    private Transform itemBody;
    private Transform markerUI;
    private BoxCollider boxCollider;
    private Rigidbody myBody;

    private bool itemFound;

    private void Start()
    {
        itemBody = transform.Find("Body");
        markerUI = transform.Find("DroneMarkerUI");

        boxCollider = GetComponent<BoxCollider>();
        myBody = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        Init();
    }

    private void Init()
    {
        if(!itemFound)
        {
            itemBody.gameObject.SetActive(GameManager.Instance.playerManager.enableDrone ? true : false);
            boxCollider.enabled = GameManager.Instance.playerManager.enableDrone ? true : false;
            myBody.isKinematic = GameManager.Instance.playerManager.enableDrone ? false : true;
        }
        else
        {
            markerUI.gameObject.SetActive(GameManager.Instance.playerManager.enableDrone ? true : false);
        }
    }

    public void HandleSelected(bool state)
    {
        if(state)
        {
            markerUI.gameObject.SetActive(true);
            itemFound = true;
        }
        else
        {
            markerUI.gameObject.SetActive(false);
            itemFound = false;
        }
    }

    public void SetBoxCollider(bool state)
    {
        boxCollider.enabled = state;
    }

    public void SetRigidbodyKinematic(bool state)
    {
        myBody.isKinematic = state;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetItemParent(Transform transform)
    {
        transform.SetParent(transform);
    }


    public void SetRotation(Vector3 value)
    {
        Debug.LogError(value);
        transform.localEulerAngles = value;
    }
}
