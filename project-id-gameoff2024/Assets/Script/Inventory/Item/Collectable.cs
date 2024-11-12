using UnityEngine;
using UnityEngine.UI;

public class Collectable : MonoBehaviour
{
    [Header("Item Information")]
    public int ItemId;
    public string ItemName;
    public string ItemDescription;
    [Space]
    public Image ItemImage;
}
