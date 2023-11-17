using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartOverlay : MonoBehaviour
{

    [Header("Timer Settings")]
    [SerializeField] Slider timerSlider;
    [SerializeField] TMPro.TextMeshProUGUI sliderText;
    
    [Header("Game Settings")]
    [SerializeField] TMPro.TextMeshProUGUI IPText;
    [SerializeField] Button startButton;

    private string ip;

    // ############################################################

    void Start() {
        ip = PlayerPrefs.GetString("IP");
        timerSlider.onValueChanged.AddListener (delegate {HandleSliderChange();});
        timerSlider.value = GameManager.Instance.GetTimer().GetTimerDuration();
    }

    void Update() {
        UpdateIPText();
    }

    void Disable() {
        timerSlider.onValueChanged.RemoveAllListeners();
    }

    // ############################################################

    public void HandleSliderChange() {
        float timerValue = timerSlider.value; 
        float minutes = Mathf.FloorToInt(timerValue / 60); 
        float seconds = Mathf.FloorToInt(timerValue % 60);
        sliderText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        GameManager.Instance.GetTimer().SetTimerDuration(timerValue);
    }

    // ############################################################

    private void UpdateIPText() {
        ip = PlayerPrefs.GetString("IP");
        IPText.text = ip;
    }

    
}
