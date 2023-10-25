using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartOverlay : MonoBehaviour
{

    [SerializeField] Slider timerSlider;
    [SerializeField] TMPro.TextMeshProUGUI sliderText;
    [SerializeField] Button startButton;

    void Start()
    {
        timerSlider.onValueChanged.AddListener (delegate {HandleSliderChange();});
        timerSlider.value = GameManager.Instance.GetTimer().GetTimerDuration();
    }

    void Update()
    {
        startButton.interactable = GameManager.Instance.GetTimer().GetTimerDuration() > GameManager.Instance.GetMinSimulationDuration();
    }

    void Disable() {
        timerSlider.onValueChanged.RemoveAllListeners();
    }

    public void HandleSliderChange() {
        float timerValue = timerSlider.value; 
        float minutes = Mathf.FloorToInt(timerValue / 60); 
        float seconds = Mathf.FloorToInt(timerValue % 60);
        sliderText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        GameManager.Instance.GetTimer().SetTimerDuration(timerValue);
    }
}
