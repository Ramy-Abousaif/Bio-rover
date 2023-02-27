using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mission", menuName = "Mission")]
public class Mission : ScriptableObject
{
    public enum MissionType
    {
        FIND,
        COLLECT,
        SCAN
    }

    [System.Serializable]
    public class Objective
    {
        public string title;
        public bool completed;
        public int currentProgress;
        public int targetProgress;
    }

    public string title;
    public MissionType type;
    [TextArea] public string description;
    public int moneyReward;
    public List<Objective> objectives;
    public bool completed = false;

    public Objective GetObjectiveByTitle(string _title)
    {
        Objective objective = objectives.Find(x => x.title == _title);

        if (objective == null)
            Debug.LogWarning("Objective with title " + _title + " does not exist.");

        return objective;
    }

    public bool CheckCompletion()
    {
        foreach (Objective objective in objectives)
        {
            if (objective.currentProgress >= objective.targetProgress)
                objective.completed = true;

            if (!objective.completed)
                return false;
        }

        if(!completed)
        {
            MissionsManager.instance.completedMissions.Add(this);
            GameManager.instance.money += moneyReward;
            completed = true;
        }

        return true;
    }
}