using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProfile", menuName = "ScriptableObjects/PlayerProfile")]
public class PlayerProfileSO : ScriptableObject
{
    public string PlayerName;
    public float PlayerHealth = 100f;
    public float PlayerStamina = 100f;
}
