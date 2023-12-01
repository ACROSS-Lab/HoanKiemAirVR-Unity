using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PollutionManager : MonoBehaviour
{
    
    // [SerializeField] private GameObject fogPlane;
    // [SerializeField] private float fadeStartingHeight = 50;
    // [SerializeField] private float fadeEndingHeight = 75;
    // [SerializeField] private float maxPollutionLevel = 500.0f;
    // [SerializeField, Range(0.0f, 1.0f)] private float blendingCoef = 0.0003f;

    private float aqiMean;
    private float highPollutionArea;
    private float midPollutionArea;
    private float lowPollutionArea;

    public static PollutionManager Instance = null;


    // ############################################# UNITY FUNCTIONS #############################################

    void Awake() {
        Instance = this;
    }
    
    // ############################################# HANDLERS #############################################

    private void HandleWorldDataReceived(WorldJSONInfo data) {
        SetAQIMean(data.aqimean);
        SetHighPollutionArea(data.pAreaHigh);
        SetMidPollutionArea(data.pAreaMid);
        SetLowPollutionArea(data.pAreaLow);
    }

    // ############################################# UTILITY FUNCTIONS #############################################

    public void SetAQIMean(float mean) {
        aqiMean = mean;
    }

    public float GetAQIMean() {
        return aqiMean;
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
