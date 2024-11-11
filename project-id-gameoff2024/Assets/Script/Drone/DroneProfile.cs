using UnityEngine;

public class DroneProfile : MonoBehaviour
{
    [SerializeField] private float setDroneTimer = 5;
    [SerializeField] private float setBatteryCost = 5;
    public float droneTimer { get; private set; }
    public float batteryCost { get; private set; }

    private void Start()
    {
        droneTimer = setDroneTimer;
        batteryCost = setBatteryCost;
    }

    public void DeductTimer(float cost)
    {
        droneTimer -= cost;

        if(droneTimer <= 0)
        {
            droneTimer = 0;
        }
    }

    public void ResetTimer()
    {
        droneTimer = setDroneTimer;
    }
}
