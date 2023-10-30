using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    
    [SerializeField] private GameObject startOverlay;
    [SerializeField] private GameObject ingameOverlay;
    [SerializeField] private GameObject endOverlay;
    [SerializeField] private GameObject crashOverlay;
    [SerializeField] private GameObject pendingOverlay;
    [SerializeField] private GameObject handHud;

    
    void OnEnable() {
        GameManager.OnGameStateChanged += HandleStateChange;
    }

    void OnDisable() {
        GameManager.OnGameStateChanged -= HandleStateChange;
    }

    private void HandleStateChange(GameState state) {
        startOverlay.SetActive(state == GameState.MENU);
        ingameOverlay.SetActive(state == GameState.GAME);
        handHud.SetActive(state == GameState.GAME);
        endOverlay.SetActive(state == GameState.END);
        crashOverlay.SetActive(state == GameState.CRASH);
        pendingOverlay.SetActive(state == GameState.PENDING);
    }    
}
