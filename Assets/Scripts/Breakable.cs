using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    private void OnDestroy()
    {
        if (MissionsManager.instance.GetMissionByName("Remove any nearby " + transform.name.ToLower() + "s").GetObjectiveByTitle("Remove any nearby " +
            transform.name.ToLower() + "s") != null && !MissionsManager.instance.GetMissionByName("Remove any nearby " +
            transform.name.ToLower() + "s").GetObjectiveByTitle("Remove any nearby " +
            transform.name.ToLower() + "s").completed)
        {
            MissionsManager.instance.GetMissionByName("Remove any nearby " + transform.name.ToLower() + "s").GetObjectiveByTitle("Remove any nearby " +
                transform.name.ToLower() + "s").currentProgress++;
        }
    }
}
