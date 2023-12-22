using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveOverlay : MonoBehaviour
{

    public List<GameObject> overlays;

    public void setActiveOverlay(bool active) {
        foreach (GameObject overlay in overlays) {
            overlay.SetActive(active);
        }
    }
}
