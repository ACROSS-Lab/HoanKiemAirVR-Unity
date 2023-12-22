using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class BuildingManager : MonoBehaviour
{

    [SerializeField] private Material buildingMaterial;

    private float blendingCoefficient = 0.1f;
    private bool buildingsHandled;
    private Dictionary<int, GameObject> buildingsMap;
    private WorldJSONInfo infoWorld;
    private bool buildingsUpdateRequested;
    // private bool buildingsUpdating;
    
    public static BuildingManager Instance = null;

    // ############################################ UNITY FUNCTIONS ############################################
    void Awake() {
        Instance = this;
    }

    void OnEnable() {
        SimulationManager.OnGeometriesInitialized += HandleGeometriesInitialized;
        SimulationManager.OnWorldDataReceived += HandleWorldDataReceived;
    }

    void OnDisable() {
        SimulationManager.OnGeometriesInitialized -= HandleGeometriesInitialized;
        SimulationManager.OnWorldDataReceived -= HandleWorldDataReceived;
    }

    void Start()
    {
        Debug.Log("BuildingManager started");
        buildingsHandled = false;
        // buildingsUpdating = false;
        buildingsUpdateRequested = false;
    }

    void Update() {
        if (buildingsUpdateRequested) {
            buildingsUpdateRequested = false;
            UpdateBuildingsPollution(infoWorld, buildingsMap);
        }
    }

    // ############################################ INITIALIZERS ############################################
    private void InitBuildings() {
        if (PolygonGenerator.GetInstance().GetGeneratedGeometries3D() != null) {
            buildingsMap = PolygonGenerator.GetInstance().GetGeneratedGeometries3D();
            HandleBuildings();
        }
    }


    // ############################################ UPDATERS ############################################
    private void UpdateBuildingsPollution(WorldJSONInfo infoWorld, Dictionary<int, GameObject> buildings) {
        float previousPollution;
        float alpha;
        foreach (BuildingInfo bi in infoWorld.buildings)
        {
            int id = bi.b[0];
            float pollution = (float) bi.b[1];

            GameObject building = null;
            if (buildings.TryGetValue(id, out building)) {
                if(building != null) {
                    foreach (Transform childTransform in building.transform) {
                        GameObject child = childTransform.gameObject;
                        Material childMaterial = child.GetComponent<MeshRenderer>().material;
                        previousPollution = buildingsHandled ? childMaterial.GetFloat("_Pollution_Level") : 0.0f;
                        alpha = previousPollution != 0.0f ? blendingCoefficient : 1.0f;
                        childMaterial.SetFloat("_Pollution_Level", (1.0f - alpha) * previousPollution + alpha * pollution);
                    }
                }                 
            }
        }
    }

    // ############################################ HANDLERS ############################################
    private void HandleGeometriesInitialized(GAMAGeometry geoms) {
        InitBuildings();
    }

    private void HandleWorldDataReceived(WorldJSONInfo infoWorld) {
        this.infoWorld = infoWorld;
        buildingsUpdateRequested = true;
    }

    private void HandleBuildings() {
        GameObject building;
        List<int> ids = new List<int>();
        foreach (var entry in buildingsMap) {
            building = entry.Value;
            if (InLakeArea(building) || OutOfMap(building)) { // Destroy buildings polygons in the lake area
                ids.Add(entry.Key);
            } else {
                if (building.TryGetComponent(out LineRenderer lineRenderer)) {
                    Destroy(lineRenderer);
                }

                foreach (Transform childTransform in building.transform) {
                    GameObject child = childTransform.gameObject;
                    child.GetComponent<MeshRenderer>().material = buildingMaterial;
                }  
            }
        }
        foreach (int id in ids) {
            Destroy(buildingsMap[id]);
            buildingsMap.Remove(id);
        }
        buildingsHandled = true;
        Debug.Log("Buildings handled"); 
    }

    
    // ############################################ UTILITY FUNCTIONS ############################################
    private bool InLakeArea(GameObject building) {
        Vector3 pos = building.transform.position;
        return (pos.x >= 595 && pos.x <= 650 && pos.z >= -855 && pos.z <= -650);
    }

    private bool OutOfMap(GameObject building) {
        Vector3 pos = building.transform.position;
        float xa = 1057;
        float ya = -1336;
        float xb = 64;
        float yb = -1030;
        
        float a = (yb-ya)/(xb - xa);
        float b = ya - a * xa;
        return pos.z < a * pos.x + b;
    }

    public bool AreBuildingsHandled() {
        return buildingsHandled;
    }
    
}
