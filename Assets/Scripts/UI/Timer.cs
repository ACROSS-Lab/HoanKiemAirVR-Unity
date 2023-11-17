using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    [SerializeField] private TMPro.TextMeshProUGUI timerText;
    [SerializeField] private float timerDuration = 30.0f;
    [SerializeField] private Color startColor;
    [SerializeField] private Color midColor;
    [SerializeField] private Color endColor;
    
    private bool timerRunning;
    private float midTime;
    private float timeRemaining;

    // ############################################################

    void Start() {
        timerRunning = false;
        timeRemaining = timerDuration;
        midTime = timerDuration / 2;
        DisplayTime(timeRemaining-1);
    }

    void Update() {
        if(timerRunning) {
            if (timeRemaining > 0) {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            } else {
                timerRunning = false;
                timeRemaining = 0;
                GameManager.Instance.UpdateGameState(GameState.END);
            }
        } else {
            timeRemaining = timerDuration;
        }
    }

    // ############################################################

    private void DisplayTime(float time) {
        time += 1;
        float minutes = Mathf.FloorToInt(time / 60); 
        float seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if (time > midTime) {
            timerText.color = Color.Lerp(midColor, startColor, (time - midTime) / midTime);
        } else {
            timerText.color = Color.Lerp(endColor, midColor, (time) / midTime);
        }
    }

    public void Reset() {
        timerRunning = false;
        timeRemaining = timerDuration;
    }

    // ############################################################

    public void SetTimerDuration(float duration) {
        timerDuration = duration;
    }

    public float GetTimerDuration() {
        return timerDuration;
    }

    public void SetTimerRunning(bool running) {
        timerRunning = running;
    }

    public bool IsTimerRunning() {
        return timerRunning;
    }

    
}
