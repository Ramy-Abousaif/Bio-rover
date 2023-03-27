using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionsManager : MonoBehaviour
{
    public static MissionsManager instance { get; private set; }

    public int selection = 0;
    public List<Mission> availableMissions;
    public List<Mission> completedMissions;
    private TMP_Text missionText;

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

    public void Setup()
    {
        missionText = GameObject.Find("MissionDetails").GetComponent<TMP_Text>();
        UpdateMissionText();
        ResetMissions();
    }

    private void Update()
    {
        if (availableMissions.Count > 0)
        {
            if (PlayerInputManager.instance.leftArrow)
            {
                selection = (selection - 1 + availableMissions.Count) % availableMissions.Count;
                UpdateMissionText();
            }

            if (PlayerInputManager.instance.rightArrow)
            {
                selection = (selection + 1) % availableMissions.Count;
                UpdateMissionText();
            }
        }
    }

    private void OnApplicationQuit()
    {
        ResetMissions();
    }

    private void OnDestroy()
    {
        ResetMissions();
    }

    public void UpdateMissionText()
    {
        missionText.text = "";
        for (int i = 0; i < availableMissions[selection].objectives.Count; i++)
        {
            string _color = "<color=white>";
            string tempObjective = availableMissions[selection].objectives[i].title;
            if (availableMissions[selection].objectives[i].currentProgress >= availableMissions[selection].objectives[i].targetProgress)
                _color = "<color=green>";
            else
                _color = "<color=white>";

            if (availableMissions[selection] == GetMissionByName("Scan objects") &&
                !(availableMissions[selection].objectives[i].currentProgress >= availableMissions[selection].objectives[i].targetProgress))
            {
                if (tempObjective.Contains("uncommon"))
                {
                    tempObjective = tempObjective.Insert(5, "<color=green>");
                    tempObjective = tempObjective.Insert(26, "<color=white>");
                }

                if (tempObjective.Contains("rare"))
                {
                    tempObjective = tempObjective.Insert(5, "<color=#00FFFF>");
                    tempObjective = tempObjective.Insert(24, "<color=white>");
                }

                if (tempObjective.Contains("exotic"))
                {
                    tempObjective = tempObjective.Insert(5, "<color=orange>");
                    tempObjective = tempObjective.Insert(25, "<color=white>");
                }
            }

            missionText.text += _color + tempObjective + ": " +
                availableMissions[selection].objectives[i].currentProgress + "/" + availableMissions[selection].objectives[i].targetProgress + "\n";
        }
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
