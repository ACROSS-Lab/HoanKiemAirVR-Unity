using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Newtonsoft.Json;

public class RoadManager : MonoBehaviour
{

    [SerializeField] private InputActionReference selectAction;
    [SerializeField] private Transform controllerTransform;
    [SerializeField] private Material roadMaterial;
    [SerializeField] private XRInteractionManager interactionManager; // Needed for XRInteractable

    private bool roadsInitialized;
    private RaycastHit hit;
    private XRSimpleInteractable xrInteractable;
    private Dictionary<string, List<GameObject>> roadsDict;
    private static List<string> closedRoads;
    private float roadClosedDist;
    
    public static event Action<string, bool> OnRoadInteracted;
    public static event Action<float> OnClosedRoadDistanceUpdated;

    public static RoadManager Instance = null;

    // ############################################# UNITY FUNCTIONS #############################################
    void Awake() {
        Instance = this;
    }

    void OnEnable() {
        GameManager.OnGeometriesInitialized += HandleGeometriesInitialized;
        GameManager.OnWorldDataReceived += HandleWorldDataReceived;
    }

    void OnDisable() {
        GameManager.OnGeometriesInitialized -= HandleGeometriesInitialized;
        GameManager.OnWorldDataReceived -= HandleWorldDataReceived;
    }

    void Start()
    {
        roadsInitialized = false;
        roadsDict = new Dictionary<string, List<GameObject>>();
        closedRoads = new List<string>();
        roadClosedDist = 0.0f;
        Debug.Log("RoadManager started");
    }

    void Update()
    {   
        // if (!roadsInitialized && GameManager.Instance.IsGameState(GameState.WAITING)) InitRoads();
        if(GameManager.Instance.IsGameState(GameState.GAME)) {
            HandleRoadsInteraction();
        }   
    }

    // ############################################# INITIALIZERS #############################################

    private void InitRoads() {
        GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");
        if (roads.Length > 0) {
            foreach (GameObject road in roads) {
                road.layer = LayerMask.NameToLayer("Road");

                if (road.TryGetComponent(out LineRenderer lineRenderer))
                    Destroy(lineRenderer);         

                if (!roadsDict.ContainsKey(road.name)) {
                    roadsDict[road.name] = new List<GameObject>();
                }
                roadsDict[road.name].Add(road);

                GameObject child = road.transform.GetChild(0).gameObject;

                if (child.name == "bottom_" + road.name) {
                    child.AddComponent<HKRoad>();
                    child.tag = "Road";
                    child.layer = LayerMask.NameToLayer("Road");   
                    MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>(); 
                    meshRenderer.material = roadMaterial;
                    string childName = road.name; // remove "bottom_" from name

                    if (child.TryGetComponent(out MeshCollider meshCollider)) { 
                        Destroy(meshCollider);
                    }

                    if (!child.TryGetComponent(out BoxCollider boxCollider)) {
                        boxCollider = child.AddComponent<BoxCollider>();
                        float sizeX = meshRenderer.bounds.size.x;
                        float sizeZ = meshRenderer.bounds.size.z;

                        if (sizeX > sizeZ) {
                            boxCollider.size = new Vector3(sizeX, 0.1f, sizeZ + 5);
                        } else {
                            boxCollider.size = new Vector3(sizeX + 5, 0.1f, sizeZ);
                        }
                    }

                    // Add XR Interactable
                    xrInteractable = child.AddComponent<XRSimpleInteractable>();
                    xrInteractable.interactionManager = interactionManager;
                    xrInteractable.interactionLayers = InteractionLayerMask.GetMask("Road");
                    xrInteractable.hoverEntered.AddListener((hoverEnterEventArgs) => HandleHoverEnter(childName));
                    xrInteractable.hoverExited.AddListener((hoverExitEventArgs) => HandleHoverExit(childName));
                }
            }
            roadsInitialized = true;
            Debug.Log("Roads intialized");
        }     
    }

    // ############################################# HANDLERS #############################################
    private void HandleHoverEnter(string roadName) {
        List<GameObject> roads = roadsDict[roadName];
        foreach (GameObject road in roads) {
            GameObject child = road.transform.GetChild(0).gameObject;
            child.GetComponent<MeshRenderer>().material.SetInt("_Selected", 1);

            OnRoadInteracted?.Invoke(roadName, child.GetComponent<HKRoad>().closed);

        }
    }

    private void HandleHoverExit(string roadName) {
        List<GameObject> roads = roadsDict[roadName];
        foreach (GameObject road in roads) {
            GameObject child = road.transform.GetChild(0).gameObject;
            OnRoadInteracted?.Invoke("", false);
            child.GetComponent<MeshRenderer>().material.SetInt("_Selected", 0);
        }
    }

    private void HandleRoadsInteraction() {
        Ray ray = new Ray(controllerTransform.transform.position, controllerTransform.transform.forward);
        if (selectAction.action.triggered) {
            if (Physics.Raycast(ray, out hit, 1000.0f)) {
                GameObject hitRoad = hit.collider.gameObject; // assume that collided object is a road (layer on right hand set to Road)
                string hitRoadName = hitRoad.name.StartsWith("bottom_") ? hitRoad.name.Substring(7) : hitRoad.name; // remove "bottom_" from name

                List<GameObject> roads = roadsDict[hitRoadName];
                foreach (GameObject road in roads) {
                        GameObject child = road.transform.GetChild(0).gameObject;
                        HKRoad hkRoad = child.GetComponent<HKRoad>();
                        hkRoad.toggleClosed();

                        OnRoadInteracted?.Invoke(hitRoadName, hkRoad.closed);

                        if (hkRoad.closed) {
                            if (!closedRoads.Contains(hitRoadName)) closedRoads.Add(hitRoadName);
                            child.GetComponent<MeshRenderer>().material.SetInt("_Closed", 1);
                        } else {
                            closedRoads.Remove(hitRoadName);
                            child.GetComponent<MeshRenderer>().material.SetInt("_Closed", 0);
                        }
                }
                SendClosedRoads();
            }
        }
    }

    private void HandleGeometriesInitialized(GAMAGeometry data) {
        InitRoads();
    }

    private void HandleWorldDataReceived(WorldJSONInfo infoWorld) {
        if (infoWorld.roadClosedDist != roadClosedDist) {
            SetRoadClosedDist(infoWorld.roadClosedDist);
            OnClosedRoadDistanceUpdated?.Invoke(roadClosedDist);
        }
    }


    // ############################################# UTILITY FUNCTIONS #############################################
    private void SendClosedRoads() {
        string closedRoadsJSON = JsonConvert.SerializeObject(closedRoads);
        ConnectionManager.Instance.SendExecutableExpression("do update_road_closed(" + closedRoadsJSON + ");");
    }

    public static List<string> GetClosedRoads() {
        return closedRoads;
    }

    public float GetRoadClosedDist() {
        return roadClosedDist;
    }

    public bool AreRoadsInitialized() {
        return roadsInitialized;
    }

    public void SetRoadClosedDist(float dist) {
        roadClosedDist = dist;
    }
}
