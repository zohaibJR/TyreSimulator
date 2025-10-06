using UnityEngine;

public class ServiceTrigger : MonoBehaviour
{
    public NPCManager npcManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            npcManager.FinishService();
        }
    }
}
