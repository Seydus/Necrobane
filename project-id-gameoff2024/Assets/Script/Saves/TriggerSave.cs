using UnityEngine;

public class TriggerSave : MonoBehaviour
{
    public SaveAndLoad sal;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "save")
        {
            if(other.gameObject.name == sal.spawn2.name)
            {
                sal.currentLevel = 2;
                sal.Save();
            }

            if (other.gameObject.name == sal.spawn3.name)
            {
                sal.currentLevel = 3;
                sal.Save();
            }

        }
    }
}
