using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Collections;

public class GameManager : MonoBehaviour
{

    [SerializeField] private TMPro.TextMeshProUGUI infoText;
    [SerializeField] private GameState currentState;

    public static GameManager Instance = null;
    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnGameRestarted;

    private Timer timer;
    private float minSimulationDuration;

    // ############################################################

    void Awake() {
        Instance = this;
    }

    void Start() {
        timer = GetComponent<Timer>();
        minSimulationDuration = 10f;
        UpdateState(GameState.MENU);
        RemoveInfoText();
    }

    void Update() {
        if (RoadManager.Instance.AreRoadsInitialized() && BuildingManager.Instance.AreBuildingsHandled() && currentState == GameState.PENDING) {
            UpdateState(GameState.GAME);
        }
    }

    // ############################################################
    
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
            case GameState.PENDING:
                HandlePendingState();
                break;
            
        }
    }

    // ############################## HANDLERS ##############################
    

    private void HandleMenuState() { }

    private void HandleGameState() {
        RemoveInfoText();
        timer.SetTimerRunning(true);
    }

    private void HandleEndState() {
        timer.SetTimerRunning(false);
        TCPConnector.GetSocketConnection().Close(); 
        TCPConnector.GetClientReceiveThread().Abort();
        TCPConnector.ResetConnection();
    }

    private void HandleCrashState() {
        timer.Reset();
    }
    
    private void HandlePendingState() {
        RemoveInfoText();
    }

    public void DisplayInfoText(string text, Color color) {
        infoText.text = text;
        infoText.color = color;
    }

    public void RemoveInfoText() {
        DisplayInfoText("", new Color(0,0,0,0));
    }    

    public void RestartGame() {
        OnGameRestarted?.Invoke();        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ############################################################

    public bool IsState(GameState state) {
        return currentState == state;
    }
    
    public Timer GetTimer() {
    return timer;
    }   

    public float GetMinSimulationDuration() {
        return minSimulationDuration;
    }
}

// ############################################################

public enum GameState {
    MENU,
    GAME,
    END,
    CRASH,
    PENDING // Waiting for incoming init data from GAMA
}

