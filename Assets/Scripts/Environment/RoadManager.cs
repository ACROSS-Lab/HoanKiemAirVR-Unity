using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class RoadManager : GAMAListener
{

    [SerializeField] private InputActionReference selectAction;
    [SerializeField] private GameObject rightController;
    [SerializeField] private Material roadMaterial;
    [SerializeField] private XRInteractionManager interactionManager;
    [SerializeField] private TMPro.TextMeshProUGUI selectedRoadText;
    [SerializeField] private GameObject fogPlane;

    private int ctr;
    private bool roadsInitialized;
    private RaycastHit hit;
    private XRSimpleInteractable xrInteractable;
    private Dictionary<string, List<GameObject>> roadsDict;
    private static string selectedRoad;
    private static bool roadStatusToSend;
    private static List<string> closedRoads;
    private static float roadClosedDist;

    public static RoadManager Instance = null;

    void Awake() {
        Instance = this;
    }

    void Start()
    {
        roadsInitialized = false;
        selectedRoad = "noRoad";
        roadStatusToSend = false;
        roadsDict = new Dictionary<string, List<GameObject>>();
        selectedRoadText.text = "";
        closedRoads = new List<string>();
        roadClosedDist = 0.0f;
    }

    void Update()
    {   
        // roads = GameObject.FindGameObjectsWithTag("Road");

        if (!roadsInitialized) InitRoads();
        HandleRoadsInteraction();     
    }



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

    private void HandleHoverEnter(string roadName) {
        List<GameObject> roads = roadsDict[roadName];
        foreach (GameObject road in roads) {
            GameObject child = road.transform.GetChild(0).gameObject;
            child.GetComponent<MeshRenderer>().material.SetInt("_Selected", 1);

            if (child.GetComponent<HKRoad>().closed) {
                SetOverlayContent(roadName + " (closed)", new Color(255, 0, 0, 255));
            } else {
                SetOverlayContent(roadName, new Color(0, 255, 0, 255));
            }
        }
    }

    private void HandleHoverExit(string roadName) {
        List<GameObject> roads = roadsDict[roadName];
        foreach (GameObject road in roads) {
            GameObject child = road.transform.GetChild(0).gameObject;
            SetOverlayContent("", new Color(0, 0, 0, 0));
            child.GetComponent<MeshRenderer>().material.SetInt("_Selected", 0);
        }
    }

    private void HandleRoadsInteraction() {
        Ray ray = new Ray(rightController.transform.position, rightController.transform.forward);
        if (selectAction.action.triggered) {
            if (Physics.Raycast(ray, out hit, 1000.0f)) {
                GameObject hitRoad = hit.collider.gameObject; // assume that collided object is a road (layer on right hand set to Road)
                string hitRoadName = hitRoad.name.StartsWith("bottom_") ? hitRoad.name.Substring(7) : hitRoad.name; // remove "bottom_" from name

                List<GameObject> roads = roadsDict[hitRoadName];
                foreach (GameObject road in roads) {
                        GameObject child = road.transform.GetChild(0).gameObject;
                        HKRoad hkRoad = child.GetComponent<HKRoad>();
                        hkRoad.toggleClosed();

                        if (hkRoad.closed) {
                            if (!closedRoads.Contains(hitRoadName)) closedRoads.Add(hitRoadName);
                            child.GetComponent<MeshRenderer>().material.SetInt("_Closed", 1);
                            SetOverlayContent(hitRoadName + " (closed)", new Color(255, 0, 0, 255));
                        } else {
                            closedRoads.Remove(hitRoadName);
                            child.GetComponent<MeshRenderer>().material.SetInt("_Closed", 0);
                            SetOverlayContent(hitRoadName, new Color(0, 255, 0, 255));
                        }
                }
                // Debug.Log(hitRoadName + " " + ctr.ToString());
                selectedRoad = hitRoadName;
                // roadStatusToSend = true;
            }
        } else {
            if (!roadStatusToSend) selectedRoad = "noRoad";
        }
        
    }


    public static string GetSelectedRoad() {
        return selectedRoad;
    }

    public static List<string> GetClosedRoads() {
        return closedRoads;
    }

    public static void SetRoadClosedDist(float roadClosedDist) {
        RoadManager.roadClosedDist = roadClosedDist;
    }

    public static float GetRoadClosedDist() {
        return roadClosedDist;
    }

    private void SetOverlayContent(string content, Color color) {
        selectedRoadText.text = content;
        selectedRoadText.color = color;
    }

    protected override void HandleGamaData(WorldJSONInfo infoWorld) {
        SetRoadClosedDist(infoWorld.roadClosedDist);
    }
}
