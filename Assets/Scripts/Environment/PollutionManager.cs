using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PollutionManager : MonoBehaviour
{
    
    [SerializeField] private GameObject fogPlane;
    [SerializeField] private float fadeStartingHeight = 50;
    [SerializeField] private float fadeEndingHeight = 75;
    [SerializeField] private float maxPollutionLevel = 500.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float blendingCoef = 0.0003f;

    private int pollutionLevel;
    private float aqiMean;
    private float aqiMax;
    private float aqiStd;
    private float highPollutionArea;
    private float midPollutionArea;
    private float lowPollutionArea;
    private float previousAlpha;
    private Material fogMaterial;

    public static PollutionManager Instance = null;

    // Fog gets always displayed even though pollution level is 0
    private float fogDisplayOffset;   

    private bool updateFogRequested;
    private GameState currentState = GameState.MENU;


    // ############################################# UNITY FUNCTIONS #############################################
    void Awake() {
        Instance = this;
    }

    void OnEnable() {
        GameManager.OnWorldDataReceived += HandleWorldDataReceived;
    }

    void OnDisable() {
        GameManager.OnWorldDataReceived -= HandleWorldDataReceived;
    }

    void Start()
    {
        fogMaterial = fogPlane.GetComponent<MeshRenderer>().material;
        previousAlpha = fogMaterial.GetFloat("_Color_Alpha");
        fogDisplayOffset = 0.02f;
    }

    void Update()
    {
        if (transform.position.y > fadeStartingHeight) {
            float x = (transform.position.y - fadeStartingHeight) / (fadeEndingHeight - fadeStartingHeight);
            float maxAlpha = fogMaterial.GetFloat("_Color_Alpha");
            fogMaterial.SetFloat("_Alpha_Clipping_Value", x * maxAlpha);
        }
        previousAlpha = fogMaterial.GetFloat("_Color_Alpha");
        float currentAlpha = (((float) pollutionLevel) / maxPollutionLevel) * 1.68f;
        fogMaterial.SetFloat("_Color_Alpha", blendingCoef * previousAlpha + (1 - blendingCoef) * currentAlpha + fogDisplayOffset);
    }

    void LateUpdate() {
        if (updateFogRequested) {
            fogPlane.SetActive(currentState == GameState.MENU);
            updateFogRequested = false;
        }
    }
    
    // ############################################# HANDLERS #############################################
    private void HandleGameStateChanged(GameState currentState) {
        this.currentState = currentState;
        updateFogRequested = true;
    }

    private void HandleWorldDataReceived(WorldJSONInfo data) {
        SetFogPollutionLevel(data.pollution);
        SetAQIMean(data.aqimean);
        SetAQIStd(data.aqistd);
        SetHighPollutionArea(data.pAreaHigh);
        SetMidPollutionArea(data.pAreaMid);
        SetLowPollutionArea(data.pAreaLow);
    }

    // ############################################# UTILITY FUNCTIONS #############################################
    public void SetFogPollutionLevel(int level) {
        pollutionLevel = level;
    }

    public int GetPollutionLevel() {
        return pollutionLevel;
    }

    public void SetAQIMean(float mean) {
        aqiMean = mean;
    }

    public float GetAQIMean() {
        return aqiMean;
    }

    public void SetAQIStd(float std) {
        aqiStd = std;
    }

    public float GetAQIStd() {
        return aqiStd;
    }

    public void SetHighPollutionArea(float area) {
        highPollutionArea = area;
    }

    public float GetHighPollutionArea() {
        return highPollutionArea;
    }

    public void SetMidPollutionArea(float area) {
        midPollutionArea = area;
    }

    public float GetMidPollutionArea() {
        return midPollutionArea;
    }

    public void SetLowPollutionArea(float area) {
        lowPollutionArea = area;
    }

    public float GetLowPollutionArea() {
        return lowPollutionArea;
    }
}
