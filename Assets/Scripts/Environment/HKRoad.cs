using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HKRoad : MonoBehaviour
{

    public bool closed = false;   

    public void toggleClosed() {
        closed = !closed;
    }
}
