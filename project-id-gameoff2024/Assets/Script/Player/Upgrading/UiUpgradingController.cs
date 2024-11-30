using UnityEngine;

public class UiUpgradingController : MonoBehaviour
{
    public GameObject Item1, Item2, Item3;

    public void TurnOff(int number)
    {
        if(number == 0)
        {
            Item1.SetActive(true);
            Item2.SetActive(false);
            Item3.SetActive(false);
        }

        if (number == 1)
        {
            Item1.SetActive(false);
            Item2.SetActive(true);
            Item3.SetActive(false);
        }

        if (number == 2)
        {
            Item1.SetActive(false);
            Item2.SetActive(false);
            Item3.SetActive(true);
        }

    }
}
