using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HorizontalConstrainst : MonoBehaviour
{ 

    public InputActionReference moveAction;
    private float initialHeight;
    private int ctr;
    // Start is called before the first frame update

    // void OnEnable()
    // {
    //     moveAction.action.canceled += UpdateVerticalPosition;
    // }

    // void OnDisable()
    // {
    //     moveAction.action.canceled -= UpdateVerticalPosition;
    // }
   
    void Start()
    {
        initialHeight = transform.position.y;
        ctr = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(moveAction.action.triggered) {
            ctr++;
            Debug.Log("Move action triggered " + ctr.ToString());
            transform.position = new Vector3(transform.position.x, initialHeight, transform.position.z);
        } else {
            initialHeight = transform.position.y;
            Debug.Log("Initial height updated");
        }  

    }

    // public void UpdateVerticalPosition(InputAction.CallbackContext context) {
    //         initialHeight = transform.position.y;
    // }
}
