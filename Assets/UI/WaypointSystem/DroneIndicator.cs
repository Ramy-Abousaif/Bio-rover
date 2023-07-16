using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneIndicator : MonoBehaviour
{

    public GameObject player;
    public Image waypointArrow;
    private PlayerState state;
    private GameObject target;
    private PlayerManager playerManager;
    private BoatController bc;
    private enum playerState
    {
        ROVER,
        BOAT
    }
    private bool visible;
    private bool isDisplayed = false;

    // Start is called before the first frame update
    void Start()
    {
        waypointArrow.enabled = false; // Hide the marker by default

        GameObject player = GameObject.Find("Player");
        playerManager = player.GetComponent<PlayerManager>();
        bc = playerManager.bc;
    }

    public void SetTarget()
    {

        if (bc.activeRovers.Count > 0)
        {
            target = bc.activeRovers[playerManager.roverIndex].transform.GetChild(0).gameObject;
            CheckMarkerVisibility();
        }

    }


    // Update is called once per frame
    void Update()
    {

        CheckMarkerVisibility();
        SetTarget();
        state = playerManager.playerState;

        if ((bc.activeRovers.Count > 0))
        {
            Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(target.transform.position);
            float distance = Vector3.Distance(player.transform.position, target.transform.position);

            Vector3 forward = player.transform.forward;
            forward.y = 0; // Make sure the vector is horizontal

        
            if (targetScreenPosition.z > 0)
            {
                waypointArrow.enabled = visible;

                Vector3 screenPos = targetScreenPosition;
                screenPos.x = Mathf.Clamp(screenPos.x, 0, Screen.width);
                screenPos.y = Mathf.Clamp(screenPos.y, 0, Screen.height);
                screenPos.z = 0;

                float angle = Vector3.SignedAngle(target.transform.position - player.transform.position, forward, Vector3.up);

                waypointArrow.transform.eulerAngles = new Vector3(0, 0, -angle);

                if (screenPos.x == 0)
                {
                    screenPos.x += waypointArrow.rectTransform.sizeDelta.x / 2;
                }
                else if (screenPos.x == Screen.width)
                {
                    screenPos.x -= waypointArrow.rectTransform.sizeDelta.x / 2;
                }

                if (screenPos.y == 0)
                {
                    screenPos.y += waypointArrow.rectTransform.sizeDelta.y / 2;
                }
                else if (screenPos.y == Screen.height)
                {
                    screenPos.y -= waypointArrow.rectTransform.sizeDelta.y / 2;
                }

                waypointArrow.rectTransform.anchoredPosition = screenPos - new Vector3(Screen.width / 2, Screen.height / 2, 0);
            }
            else
            {
                waypointArrow.enabled = visible;

                Vector3 screenPos = Vector3.zero;

                if (targetScreenPosition.x <= 0)
                {
                    screenPos.x = waypointArrow.rectTransform.sizeDelta.x / 2;
                }
                else if (targetScreenPosition.x >= Screen.width)
                {
                    screenPos.x = Screen.width - waypointArrow.rectTransform.sizeDelta.x / 2;
                }

                if (targetScreenPosition.y <= 0)
                {
                    screenPos.y = waypointArrow.rectTransform.sizeDelta.y / 2;
                }
                else if (targetScreenPosition.y >= Screen.height)
                {
                    screenPos.y = Screen.height - waypointArrow.rectTransform.sizeDelta.y / 2;
                }

                screenPos.z = 0;
                waypointArrow.rectTransform.anchoredPosition = screenPos - new Vector3(Screen.width / 2, Screen.height / 2, 0);

                float angle = Vector3.SignedAngle(-target.transform.position, forward, Vector3.up);

                waypointArrow.transform.eulerAngles = new Vector3(0, 0, -angle);


            }
        }

    }

    public void CheckMarkerVisibility()
    {

        if (bc.activeRovers.Count > 0)
        {
            if (state == PlayerState.BOAT)
            {
                visible = true;
                waypointArrow.enabled = visible;
            }

            else
            {
                visible = false;
                waypointArrow.enabled = visible;
            }
        }
        else
        {
            visible = false;
            waypointArrow.enabled = visible;
        }

    }

}
