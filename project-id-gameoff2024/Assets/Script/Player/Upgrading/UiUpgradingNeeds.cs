using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiUpgradingNeeds : MonoBehaviour
{
    public int cost;
    public int Extracost;

    public InventoryHolder inv;

    public string Name;

    public Sprite Avatar;
    public Sprite NeededItem;
    public Sprite ExtraNeededItem;

    public string Description;
    public int lvl;

    public TextMeshProUGUI ItemCost;
    public TextMeshProUGUI ExtraItemCost;
    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI ItemDescription;
    public TextMeshProUGUI ItemLvl;

    public Image ItemAvatar;
    public Image ExtraNeededItemAvatar;
    public Image NeededItemAvatar;

   //private void Update()
  //  {
    //    ItemCost.text = cost.ToString();
    //  /  ItemName.text = Name;
    ///    ItemDescription.text = Description;
    //    ItemLvl.text = lvl.ToString();
    //    ItemAvatar.sprite = Avatar;
    //    NeededItemAvatar.sprite = NeededItem;

    //    if (ExtraNeededItem != null)
    //    {
    //        ExtraItemCost.text = Extracost.ToString();
    //        ExtraNeededItemAvatar.sprite = ExtraNeededItem;
    //    }
   // }
}
