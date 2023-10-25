using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSOverlay : MonoBehaviour
{   

    
    public TMPro.TextMeshProUGUI fpsText;

    private int frameCount;
    private int cumFrameCount;
    private int FPS_UPDATE;
    
    void Start()
    {
        frameCount = 0;
        cumFrameCount = 0;
        FPS_UPDATE = 60;
    }

    void Update()
    {  
        frameCount++;
        cumFrameCount += (int) Mathf.Round(1.0f / Time.unscaledDeltaTime);

        if (frameCount % FPS_UPDATE == 0)
        {
            fpsText.text = "FPS: " + (cumFrameCount / frameCount).ToString();
            frameCount = 0;
            cumFrameCount = 0;
        }
    }


}
