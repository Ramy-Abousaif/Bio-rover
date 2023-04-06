using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindObjective : MonoBehaviour
{
    public string missionName;
    public string objectiveName;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Rover"))
        {
            Mission findMission = MissionsManager.instance.GetMissionByName(missionName);
            Mission.Objective findObjective = null;

            if (findMission != null)
                findObjective = findMission.GetObjectiveByTitle(objectiveName);


            if (findObjective != null && !findObjective.completed)
                findObjective.currentProgress++;

            if (findMission != null)
                findMission.CheckCompletion();
        }
    }
}
