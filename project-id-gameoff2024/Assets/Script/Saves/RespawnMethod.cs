using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnMethod : MonoBehaviour
{
    public void Respawn()
    {
        SceneManager.LoadScene(1);
    }
}
