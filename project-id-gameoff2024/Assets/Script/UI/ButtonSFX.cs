using UnityEngine;

public class ButtonSFX : MonoBehaviour
{

    public void onClick()
    {
        AkSoundEngine.PostEvent("Play_ButtonSFX", gameObject);

    }
}