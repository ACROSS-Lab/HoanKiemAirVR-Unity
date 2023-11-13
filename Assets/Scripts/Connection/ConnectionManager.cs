using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class ConnectionManager : WebSocketConnector
{

    [SerializeField] private ConnectionState currentState;
    private string id_vr;
    private bool connectionRequested;

    // called when the connection state is manually changed
    public static event Action<ConnectionState> OnConnectionStateChange;

    // called when a "json_simulation" message is received
    public static event Action<JObject> OnServerMessageReceived;

    // called when a "json_state" message is received 
    public static event Action<JObject> OnConnectionStateReceived;

    // called when a connection request fails
    public static event Action<bool> OnConnectionAttempted;

    public static ConnectionManager Instance = null;
    
    void Awake() {
        Instance = this;
    }

    void Start() {
        UpdateConnectionState(ConnectionState.DISCONNECTED);
        id_vr = Guid.NewGuid().ToString();
        connectionRequested = false;
    }

    
    public void UpdateConnectionState(ConnectionState newState) {

        switch(newState) {
            case ConnectionState.PENDING:
                Debug.Log("ConnectionManager: UpdateConnectionState -> PENDING");
                break;
            case ConnectionState.CONNECTED:
                Debug.Log("ConnectionManager: UpdateConnectionState -> CONNECTED");
                break;
            case ConnectionState.AUTHENTICATED:
                Debug.Log("ConnectionManager: UpdateConnectionState -> AUTHENTICATED");
                break;
            case ConnectionState.DISCONNECTED:
                Debug.Log("ConnectionManager: UpdateConnectionState -> DISCONNECTED");
                break;
            default:
                break;
        }

        OnConnectionStateChange?.Invoke(newState);

        currentState = newState;
    }

    // ################################## HANDLERS ###############################################

    protected override void HandleConnectionOpen(object sender, System.EventArgs e)
    {
        var jsonId = new Dictionary<string,string> {
                {"type", "connection"},
                { "id", id_vr}
            };
        string jsonStringId = JsonConvert.SerializeObject(jsonId);
        SendMessageToServer(jsonStringId, new Action<bool>((success) => {
            if (success) {}
        }));
        Debug.Log("ConnectionManager: Connection opened");
    }

    protected override void HandleReceivedMessage(object sender, MessageEventArgs e)
    {
        if (e.IsText)
        {
            JObject jsonObj = JObject.Parse(e.Data);
            string type = (string) jsonObj["type"];

            switch(type) {
                case "json_state":
                    OnConnectionStateReceived?.Invoke(jsonObj);
                    bool authenticated = (bool) jsonObj["player"][id_vr]["authentified"];
                    bool connected = (bool) jsonObj["player"][id_vr]["connected"];

                    if (authenticated && connected)  {
                        if (!IsConnectionState(ConnectionState.AUTHENTICATED)) {
                            Debug.Log("ConnectionManager: Successfully authenticated");
                            UpdateConnectionState(ConnectionState.AUTHENTICATED);
                        } else {
                            Debug.LogWarning("ConnectionManager: Already authenticated");
                        }
                        

                    } else if (connected && !authenticated) {
                        if(!IsConnectionState(ConnectionState.CONNECTED)) {
                            connectionRequested = false;
                            Debug.Log("ConnectionManager: Successfully connected, waiting for authentication...");
                            UpdateConnectionState(ConnectionState.CONNECTED);
                            OnConnectionAttempted?.Invoke(true);
                        } else {
                            Debug.LogWarning("ConnectionManager: Already connected, waiting for authentication...");
                        }
                        
                    }
                    break;

                case "json_simulation":
                    JObject content = (JObject) jsonObj["contents"];
                    // Debug.Log("ConnectionManager: Content received -> " + content.ToString());
                    OnServerMessageReceived?.Invoke(content);
                    break;

                default:
                    break;
            }
        }
    }

    protected override void HandleConnectionClosed(object sender, CloseEventArgs e) {   
        // checks if the connection was closed just after a connection request
        if (connectionRequested) {
            connectionRequested = false;
            OnConnectionAttempted?.Invoke(false);
            Debug.Log("ConnectionManager: Failed to connect to server");
        }
        UpdateConnectionState(ConnectionState.DISCONNECTED);
    }

    public void TryConnectionToServer() {
        if(IsConnectionState(ConnectionState.DISCONNECTED)) {
            Debug.Log("ConnectionManager: Attempting to connect to middleware...");
            connectionRequested = true;
            UpdateConnectionState(ConnectionState.PENDING);
            GetSocket().ConnectAsync();
        } else {
            Debug.LogWarning("ConnectionManager: Already connected to middleware");
        }
        
    }

    public bool IsConnectionState(ConnectionState currentState) {
        return this.currentState == currentState;
    }

    public void SendExecutableExpression(string expression) {
        Dictionary<string,string> jsonExpression = new Dictionary<string,string> {
            {"type", "expression"},
            {"expr", expression}
        };
        string jsonStringExpression = JsonConvert.SerializeObject(jsonExpression);
        SendMessageToServer(jsonStringExpression, new Action<bool>((success) => {
            if (success) {
                Debug.Log("ConnectionManager: Executable expression sent to middelware");
            } else {
                Debug.LogError("ConnectionManager: Failed to send executable expression");
            }
        }));
    }

}

public enum ConnectionState {
    DISCONNECTED,
    // waiting for connection to be established
    PENDING, 
    // connection established, waiting for authentication
    CONNECTED,
    // connection established and authenticated
    AUTHENTICATED
}
