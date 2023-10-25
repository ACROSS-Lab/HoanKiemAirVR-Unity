using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PollutionSlider : MonoBehaviour
{

    [SerializeField] private Slider pollutionSlider;
    [SerializeField] private float sliderMaxValue;
    [SerializeField] private float sliderMinValue;

    private float aqiStd;
    
    void Start()
    {
        aqiStd = 0;
        pollutionSlider.maxValue = sliderMaxValue;
        pollutionSlider.minValue = sliderMinValue;
    }

    void Update()
    {
        aqiStd = PollutionManager.Instance.GetAQIStd();
        pollutionSlider.value = Math.Min(aqiStd, sliderMaxValue);
    }
}
