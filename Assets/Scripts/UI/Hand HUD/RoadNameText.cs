using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadNameText : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI roadNameText;
    [SerializeField] private Color closedColor = new Color(255, 0, 0, 255);
    [SerializeField] private Color openColor = new Color(0, 255, 0, 255);
    
    // ############################################################

    void OnEnable() {
        RoadManager.OnRoadInteracted += HandleRoadInteracted;
    }

    void OnDisable() {
        RoadManager.OnRoadInteracted -= HandleRoadInteracted;
    }

    void Start()
    {
        roadNameText.text = "";
    }

   // ############################################################

    private void HandleRoadInteracted(string roadName, bool isClosed) {
        if (isClosed) {
            roadNameText.text = roadName + " (closed)";
            roadNameText.color = closedColor;
        } else {
            roadNameText.text = roadName;
            roadNameText.color = openColor;
        }
    }
}
