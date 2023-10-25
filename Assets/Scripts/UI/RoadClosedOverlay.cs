using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadClosedOverlay : MonoBehaviour
{
    public TMPro.TextMeshProUGUI closedRoadText;
    float dist;

    // Start is called before the first frame update
    void Start()
    {
        closedRoadText.text = "Closed : 0.00 km";
        dist = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        dist = RoadManager.GetRoadClosedDist() / 1000.0f; // km

        closedRoadText.text = "Closed : " + dist.ToString("0.00") + " km";
    }
}
