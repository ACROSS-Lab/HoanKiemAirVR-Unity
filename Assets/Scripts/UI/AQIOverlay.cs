using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AQIOverlay : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI AQIText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AQIText.text = "mean: " + PollutionManager.Instance.GetAQIMean().ToString("0.00") + "\nmax: " + PollutionManager.Instance.GetAQIMax().ToString("0.00") + "\nstd: " + PollutionManager.Instance.GetAQIStd().ToString("0.00");
    } 
}
