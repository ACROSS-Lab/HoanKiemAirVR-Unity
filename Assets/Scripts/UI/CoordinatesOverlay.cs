using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinatesOverlay : MonoBehaviour
{
    public TMPro.TextMeshProUGUI debugOverlay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        debugOverlay.text = transform.position.ToString();
    }
}
