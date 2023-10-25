using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private TMPro.TextMeshProUGUI infoText;

    public static GameManager Instance = null;
    public static event Action<GameState> OnGameStateChanged;
    public GameState currentState;

    private Timer timer;
    private float minSimulationDuration;

    void Awake() {
        Instance = this;
        timer = GetComponent<Timer>();
    }

    void Start() {
        UpdateState(GameState.MENU);
        RemoveInfoText();
        minSimulationDuration = 10f;
    }
    
    public void UpdateState(GameState newState) {
        currentState = newState;

        OnGameStateChanged?.Invoke(newState);

        switch(currentState) {
            case GameState.MENU:
                HandleMenuState();
                break;
            case GameState.GAME:
                HandleGameState();
                break;
            case GameState.END:
                HandleEndState();
                break;
            case GameState.CRASH:
                HandleCrashState();
                break;
        }

        
    }

    public bool IsState(GameState state) {
        return currentState == state;
    }

    private void HandleMenuState() {

    }

    private void HandleGameState() {
        RemoveInfoText();
        timer.SetTimerRunning(true);
    }

    private void HandleEndState() {
        timer.SetTimerRunning(false);
    }

    private void HandleCrashState() {
        timer.Reset();
    }

    public void DisplayInfoText(string text, Color color) {
        infoText.text = text;
        infoText.color = color;
    }

    public void RemoveInfoText() {
        DisplayInfoText("", new Color(0,0,0,0));
    }

    public Timer GetTimer() {
        return timer;
    }

    public float GetMinSimulationDuration() {
        return minSimulationDuration;
    }

}

public enum GameState {
    MENU,
    GAME,
    END,
    CRASH
}

