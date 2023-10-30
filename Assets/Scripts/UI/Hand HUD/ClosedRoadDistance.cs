using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedRoadDistance : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI closedRoadText;

    // ############################################################

    void OnEnable() {
        RoadManager.OnClosedRoadDistanceUpdated += HandleClosedRoadDistanceUpdate;
    }

    void OnDisable() {
        RoadManager.OnClosedRoadDistanceUpdated -= HandleClosedRoadDistanceUpdate;
    }

    void Start()
    {
        closedRoadText.text = "0.00 km";

        if (GameManager.Instance.IsState(GameState.END)) {
            closedRoadText.text = (RoadManager.Instance.GetRoadClosedDist() / 1000.0f).ToString("0.00") + " km";
        }
    }

    // ############################################################

    private void HandleClosedRoadDistanceUpdate(float distance) {
        float dist = distance / 1000.0f;
        closedRoadText.text = dist.ToString("0.00") + " km";
    }
}
