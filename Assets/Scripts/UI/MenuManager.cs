using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    
    [SerializeField] private GameObject startOverlay;
    [SerializeField] private GameObject ingameOverlay;
    [SerializeField] private GameObject endOverlay;
    [SerializeField] private GameObject crashOverlay;
    [SerializeField] private GameObject waitingOverlay;
    [SerializeField] private GameObject loadingDataOverlay;
    // [SerializeField] private GameObject handHud;

    private bool updateRequested;
    private GameState curentState;

    void Start() {
        updateRequested = true;
        curentState = GameState.MENU;
    }
    
    void OnEnable() {
        GameManager.OnGameStateChanged += HandleStateChange;
    }

    void OnDisable() {
        GameManager.OnGameStateChanged -= HandleStateChange;
    }

    void LateUpdate() {
        if (updateRequested) {
            startOverlay.SetActive(curentState == GameState.MENU);
            waitingOverlay.SetActive(curentState == GameState.WAITING);
            loadingDataOverlay.SetActive(curentState == GameState.LOADING_DATA);
            ingameOverlay.SetActive(curentState == GameState.GAME);
            // handHud.SetActive(curentState == GameState.GAME);
            endOverlay.SetActive(curentState == GameState.END);
            crashOverlay.SetActive(curentState == GameState.CRASH);
            updateRequested = false;
        }
    }

    private void HandleStateChange(GameState newState) {
        updateRequested = true;
        curentState = newState; 
    }   
}
