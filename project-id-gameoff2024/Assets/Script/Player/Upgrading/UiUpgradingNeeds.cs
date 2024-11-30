using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiUpgradingNeeds : MonoBehaviour
{
    public int cost;
    public int Extracost;

    public InventoryHolder inv;

    public Sprite NeededItem;
    public Sprite ExtraNeededItem;

    public int lvl;

    public TextMeshProUGUI ItemCost;
    public TextMeshProUGUI ExtraItemCost;
    public TextMeshProUGUI ItemLvl;

    public Image ExtraNeededItemAvatar;
    public Image NeededItemAvatar;

    public bool IsExtra;

    private void Update()
    {
        ItemCost.text = cost.ToString();
        ItemLvl.text = "Level " + lvl.ToString();
        NeededItemAvatar.sprite = NeededItem;

        if (ExtraNeededItem != null)
        {
            ExtraItemCost.text = Extracost.ToString();
            ExtraNeededItemAvatar.sprite = ExtraNeededItem;
        }

        if(lvl >= 3)
        {
            Extracost = 1;
            IsExtra = true;
        }

        if(IsExtra)
        {
            ExtraNeededItemAvatar.gameObject.SetActive(true);
            ExtraItemCost.gameObject.SetActive(true);
        }

    }

}
