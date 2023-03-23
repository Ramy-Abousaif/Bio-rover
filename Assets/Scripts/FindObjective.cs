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
            if (MissionsManager.instance.GetMissionByName(missionName).GetObjectiveByTitle(objectiveName) != null &&
                !MissionsManager.instance.GetMissionByName(missionName).GetObjectiveByTitle(objectiveName).completed)
            {
                MissionsManager.instance.GetMissionByName(missionName).GetObjectiveByTitle(objectiveName).currentProgress++;
            }

            MissionsManager.instance.GetMissionByName(missionName).CheckCompletion();
        }
    }
}
