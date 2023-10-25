using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOverlay : MonoBehaviour
{
    
    [SerializeField] private TMPro.TextMeshProUGUI highLevelText;
    [SerializeField] private TMPro.TextMeshProUGUI midLevelText;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GetTimer().IsTimerRunning()) {
            highLevelText.text = PollutionManager.Instance.GetHighPollutionArea().ToString("0") + "m²";
            midLevelText.text = PollutionManager.Instance.GetMidPollutionArea().ToString("0") + "m²";
        }
    }
}
