using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GAMAListener : MonoBehaviour
{

    void OnEnable() {
        GlobalTest.OnGamaDataReceived += HandleGamaData;
    }

    void OnDisable() {
        GlobalTest.OnGamaDataReceived -= HandleGamaData;
    }

    protected abstract void HandleGamaData(WorldJSONInfo data);
    
}
