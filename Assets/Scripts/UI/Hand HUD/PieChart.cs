using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChart : MonoBehaviour
{

    [SerializeField] private Image[] pieImages;
    [SerializeField] private float[] pieValues;

    private float totalAmount;

    // ############################################################
    
    void Start()
    {
        totalAmount = ComputeTotalAmount();
        if (SimulationManager.Instance.IsGameState(GameState.END)) {
            totalAmount = ComputeTotalAmount();
            float[] vals = {PollutionManager.Instance.GetLowPollutionArea(), PollutionManager.Instance.GetMidPollutionArea(), PollutionManager.Instance.GetHighPollutionArea()};
            ConvertValuesToPie(vals);
        }
    }

    void Update() {
        if (SimulationManager.Instance.IsGameState(GameState.GAME)) {
            totalAmount = ComputeTotalAmount();
            float[] vals = {PollutionManager.Instance.GetLowPollutionArea(), PollutionManager.Instance.GetMidPollutionArea(), PollutionManager.Instance.GetHighPollutionArea()};
            ConvertValuesToPie(vals);
        } 
    }

    // ############################################################

    public void ConvertValuesToPie(float[] values) {
        pieValues = values;
        totalAmount = ComputeTotalAmount();
        float[] ratios = ComputeRatios();
        float startAngle = 0;
        for (int i = 0; i < pieImages.Length; i++) {
            pieImages[i].fillAmount = ratios[i];
            pieImages[i].transform.rotation = Quaternion.Euler(pieImages[i].transform.eulerAngles.x, pieImages[i].transform.eulerAngles.y, startAngle);
            startAngle -= ratios[i] * 360;
        }
    }

    private float[] ComputeRatios() {
        float[] ratios = new float[pieValues.Length];
        for (int i = 0; i < pieValues.Length; i++) {
            ratios[i] = pieValues[i] / totalAmount;
        }
        return ratios;
    }

    private float ComputeTotalAmount() {
        float total = 0;
        foreach (float value in pieValues) {
            total += value;
        }
        return total;
    }

    // ############################################################

    public float[] GetPieValues() {
        return pieValues;
    }
}
