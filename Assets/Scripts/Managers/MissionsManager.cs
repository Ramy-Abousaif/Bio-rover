using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionsManager : MonoBehaviour
{
    public static MissionsManager instance { get; private set; }

    public List<Mission> availableMissions;
    public List<Mission> completedMissions;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        ResetMissions();
    }

    private void OnApplicationQuit()
    {
        ResetMissions();
    }

    private void OnDestroy()
    {
        ResetMissions();
    }

    public Mission GetMissionByName(string _missionName)
    {
        Mission mission = availableMissions.Find(x => x.title == _missionName);

        if (mission == null)
            Debug.LogWarning("Mission with name " + _missionName + " does not exist.");

        return mission;
    }

    void ResetMissions()
    {
        foreach (var mission in availableMissions)
        {
            mission.completed = false;
            foreach (var objective in mission.objectives)
            {
                objective.currentProgress = 0;
                objective.completed = false;
            }
        }
    }
}
