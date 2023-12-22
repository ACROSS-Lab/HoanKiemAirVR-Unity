using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {
    
    [SerializeField] private List<GameObject> allOverlays;
    [SerializeField] private List<GameState> associatedGameStates;

// private bool changeRequested = false;
// private GameState currentState;

    void Start() {
        if (allOverlays.Count != associatedGameStates.Count) {
            Debug.LogError("MenuManager: All Overlays and Associated Game States must have the same length");
        }
    }
    
    void OnEnable() {
        SimulationManager.OnGameStateChanged += HandleMenuOnGameStateChange;
        Debug.Log("MenuManager: Subscribed to OnGameStateChanged");
    }

    void OnDisable() {
        SimulationManager.OnGameStateChanged -= HandleMenuOnGameStateChange;
        Debug.Log("MenuManager: Unsubscribed from OnGameStateChanged");
    }

    // void LateUpdate() {
    //     if (changeRequested) {
    //         changeRequested = false;
    //         UpdateOverlays();
    //     }
    // }

    // private void HandleMenuOnGameStateChange(GameState newState) {
    //     Debug.Log("MenuManager: HandleMenuOnGameStateChange fired");
    //     this.currentState = newState;
    //     changeRequested = true;
    // }   
    private void HandleMenuOnGameStateChange(GameState newState) {
        for (int i = 0; i < allOverlays.Count; i++) {
            if (associatedGameStates[i] == newState) {
                allOverlays[i].SetActive(true);
            } else {
                allOverlays[i].SetActive(false);
            }
        }
    }

    // private void UpdateOverlays() {
    //     for (int i = 0; i < allOverlays.Count; i++) {
    //         if (associatedGameStates[i] == this.currentState) {
    //             allOverlays[i].SetActive(true);
    //         } else {
    //             allOverlays[i].SetActive(false);
    //         }
    //     }
    // }
    
}
