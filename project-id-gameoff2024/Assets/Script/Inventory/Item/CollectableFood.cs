using UnityEngine;

[CreateAssetMenu(fileName = "FoodCollectable", menuName = "ScriptableObjects/Inventory/FoodCollectable")]
public class CollectableFood : Collectable
{
    [Header("Food Information")]
    public float healthIncrease;
}
