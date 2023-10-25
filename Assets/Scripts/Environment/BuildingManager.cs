using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class BuildingManager : MonoBehaviour
{

    [SerializeField] private Material buildingMaterial;
    private static float blendingCoefficient = 0.1f;
    private static bool buildingsHandled;
    private bool buildingsInitialized;
    private Dictionary<int, GameObject> buildingsMap;

    void Start()
    {
        buildingsHandled = false;
        buildingsInitialized = false;
    }

    void Update()
    {
        if (!buildingsInitialized) InitBuildings();
        // if (!buildingsHandled) HandleBuildings();
    }

    private void InitBuildings() {
        // buildings = GameObject.FindGameObjectsWithTag("Building");
        if (PolygonGenerator.GetGeneratedBuildings() != null) {
            buildingsMap = PolygonGenerator.GetGeneratedBuildings();
            Debug.Log(buildingsMap.Count.ToString() + " buildings initialized");
            buildingsInitialized = true;
            HandleBuildings();
        }
        // if (buildings.Length > 0) {
        //     buildingsInitialized = true;
        //     Debug.Log(buildings.Length.ToString() + " buildings initialized");
        // }
    }

    private void HandleBuildings() {
        GameObject building;
        List<int> ids = new List<int>();
        foreach (var entry in buildingsMap) {
            building = entry.Value;
            // if (InLakeArea(building) || OutOfMap(building) || OutOfMap2(building) || OutOfMap3(building)) { // Destroy buildings polygons in the lake area
            if (InLakeArea(building) || OutOfMap(building)) { // Destroy buildings polygons in the lake area
                // buildingsMap.Remove(id);
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

    public static void UpdateBuildingsPollution(WorldJSONInfo infoWorld, Dictionary<int, GameObject> buildings) {
        float previousPollution;
        float alpha;
        foreach (BuildingInfo bi in infoWorld.buildings)
        {
            int id = bi.b[0];
            float pollution = (float) bi.b[1];
            GameObject building = null;
            if (buildings.TryGetValue(id, out building))
            {
                // building = buildings[id];
                // Change "pollution" property of the building material
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

    // DESTROY BUILDINGS in AREA (595,-855);(650,-650)
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

    private bool OutOfMap2(GameObject building) {
        Vector3 pos = building.transform.position;
        float xa = 324;
        float ya = -178;
        float xb = 128;
        float yb = -1213;
        
        float a = (yb-ya)/(xb - xa);
        float b = ya - a * xa;
        return pos.z > a * pos.x + b;
    }

    private bool OutOfMap3(GameObject building) {
        Vector3 pos = building.transform.position;
        float xa = 307;
        float ya = -368;
        float xb = 652;
        float yb = -290;
        
        float a = (yb-ya)/(xb - xa);
        float b = ya - a * xa;
        return pos.z > a * pos.x + b;
    }
    
}
