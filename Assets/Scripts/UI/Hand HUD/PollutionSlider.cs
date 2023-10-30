using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PollutionSlider : MonoBehaviour
{

    [SerializeField] private Slider pollutionSlider;

    // ############################################################
    
    void Start()
    {
        pollutionSlider.value = 0.0f;

        if (GameManager.Instance.IsState(GameState.END)) {
            pollutionSlider.value = PollutionManager.Instance.GetAQIMean();
            pollutionSlider.interactable = false;
        }
    }

    void Update()
    {
        if (GameManager.Instance.IsState(GameState.GAME)) 
        {
            pollutionSlider.value = PollutionManager.Instance.GetAQIMean();
        } 
    }

}
