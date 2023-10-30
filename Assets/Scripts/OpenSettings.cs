using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenSettings : MonoBehaviour
{

    [SerializeField] private InputActionReference toggleSettingsAction;
    [SerializeField] private GameObject ipMenu;

    private bool openSettings;

    // ############################################################

    void OnEnable()
    {
        toggleSettingsAction.action.Enable();
        toggleSettingsAction.action.performed += HandleSettingsVisivility;
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDisable()
    {
        toggleSettingsAction.action.Disable();
        toggleSettingsAction.action.performed -= HandleSettingsVisivility;
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    void Start() {
        openSettings = false;
        ipMenu.SetActive(openSettings);
    }

    // ############################################################

    private void HandleSettingsVisivility(InputAction.CallbackContext obj) {
        openSettings = !openSettings;
        ipMenu.SetActive(openSettings);
    }

    private void HandleGameStateChanged(GameState state) {
        if (state == GameState.MENU) {
            toggleSettingsAction.action.Enable();
        } else {
            toggleSettingsAction.action.Disable();
        }
    }

    // ############################################################

    public void CloseSettings() {
        openSettings = false;
        ipMenu.SetActive(openSettings);
    }
}
