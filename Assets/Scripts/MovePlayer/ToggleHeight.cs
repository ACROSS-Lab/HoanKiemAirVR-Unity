using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ToggleHeight : MonoBehaviour
{
    [SerializeField] private InputActionReference toggleHeightAction;
    [SerializeField] private float maxHeight;
    [SerializeField] private float minHeight;
    [SerializeField] private GameObject rightController;
    private bool highPosition = true;

    void Update() {
        if (toggleHeightAction.action.triggered) {
            toggleHeight();
        }
    }


    private void toggleHeight() {
        if (highPosition) {
            
            Vector3 controllerPos = rightController.transform.position;
            Vector3 controllerDir = rightController.transform.forward;
            Vector3 planePos = new Vector3(0, 0, 0);
            Vector3 planeNormal = new Vector3(0, 1, 0);
            float t = Vector3.Dot(planeNormal, (planePos - controllerPos)) / Vector3.Dot(planeNormal, controllerDir);
            Vector3 intersectionPoint = controllerPos + t * controllerDir;
            transform.position = new Vector3(intersectionPoint.x, minHeight, intersectionPoint.z);
        } else {
            transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z);
        }
        highPosition = !highPosition;
    }
}
