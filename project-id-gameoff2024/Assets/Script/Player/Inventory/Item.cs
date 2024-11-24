using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public string Objname;
    public int amount;
    public Sprite avatar;

    public bool inv;
     
    public Image Image;
    public TextMeshProUGUI Count;
    public TextMeshProUGUI Name;

    private void Update()
    {
        if (inv)
        {
            Image.sprite = avatar;
            Count.text = amount.ToString();
            Name.text = Objname;
        }
    }
}
