using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameListener : MonoBehaviour
{

    protected void OnEnable() {
        GlobalTest.OnGamaDataReceived += HandleGamaData;
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    protected void OnDisable() {
        GlobalTest.OnGamaDataReceived -= HandleGamaData;
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    protected abstract void HandleGamaData(WorldJSONInfo data);
    
    protected abstract void HandleGameStateChanged(GameState state);
    
}
