using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    private void OnDestroy()
    {
        Mission removeMission = MissionsManager.instance.GetMissionByName("Remove any nearby " + transform.name.ToLower() + "s");
        Mission.Objective removeObjective = null;

        if (removeMission != null)
            removeObjective = removeMission.GetObjectiveByTitle("Remove any nearby " + transform.name.ToLower() + "s");

        if (removeObjective != null && !removeObjective.completed)
            removeObjective.currentProgress++;

        if (removeMission != null)
            removeMission.CheckCompletion();
    }
}
