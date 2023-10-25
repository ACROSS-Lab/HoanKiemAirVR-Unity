using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    
    [SerializeField] private GameObject startOverlay;
    [SerializeField] private GameObject ingameOverlay;
    [SerializeField] private GameObject endOverlay;
    [SerializeField] private GameObject crashOverlay;

    
    void Awake() {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    void OnDestroy() {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameState state) {
        startOverlay.SetActive(state == GameState.MENU);
        ingameOverlay.SetActive(state == GameState.GAME);
        endOverlay.SetActive(state == GameState.END);
        crashOverlay.SetActive(state == GameState.CRASH);
    }    
}
