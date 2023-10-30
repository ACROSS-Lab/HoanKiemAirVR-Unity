using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonGenerator
{
    CoordinateConverter converter;
    float offsetYBackgroundGeom;
    private Dictionary<int, GameObject> buildingsMap;

    private static PolygonGenerator instance;

    private PolygonGenerator() {}

    public void Init(CoordinateConverter c, float Yoffset) {
        converter = c;
        offsetYBackgroundGeom = Yoffset;
        buildingsMap = new Dictionary<int, GameObject>();
    }

    public static PolygonGenerator GetInstance() {
        if (instance == null) {
            instance = new PolygonGenerator();
        }
        return instance;
    }

    public static void DestroyInstance() {
        instance = null;
    }

    public void GeneratePolygons(GAMAGeometry geom)
    {
        GameObject generated = new GameObject();
        generated.name = "Generated Roads";
        GameObject generatedBuildings = new GameObject();
        generatedBuildings.name = "Generated Buildings";


        List<Vector2> pts = new List<Vector2>();
        int cpt = 0;
        for (int i = 0; i < geom.points.Count; i++)
        {
            GAMAPoint pt = geom.points[i];
            if (pt.c.Count < 2)
            {
                if (pts.Count > 2)
                {                    
                    GameObject p = GeneratePolygon(pts.ToArray(), geom.names.Count > 0 ?  geom.names[cpt] : "", geom.tags.Count > 0 ?  geom.tags[cpt] : "", geom.heights[cpt], geom.hasColliders[cpt], geom.is3D[cpt]);
                    if (geom.tags.Count > 0 && geom.tags[cpt] == "Building")
                    {
                        p.transform.SetParent(generatedBuildings.transform);
                        buildingsMap.Add(Int32.Parse(geom.names[cpt].Substring(8)), p);
                    }
                    else
                    {
                        p.transform.SetParent(generated.transform);
                    }
                }
                pts = new List<Vector2>();
                cpt++;
            }
            else
            {
                pts.Add(converter.fromGAMACRS2D(pt.c[0], pt.c[1]));
            }


        }
    }

    // Start is called before the first frame update
    GameObject GeneratePolygon(Vector2[] MeshDataPoints, string name, string tag, float extrusionHeight, bool isUsingCollider, bool is3D)
    {
        // bool is3D = false;
        bool isUsingBottomMeshIn3D = false;
        bool isOutlineRendered = true;

        // create new GameObject (as a child)
        GameObject polyExtruderGO = new GameObject();
        if (name != "")
            polyExtruderGO.name = name;
        
        if (tag != "") {
            polyExtruderGO.tag = tag;
            // polyExtruderGO.layer = Layer Mask.NameToLayer(tag);
        }

        // reference to setup example poly extruder 
        PolyExtruder polyExtruder;

        // add PolyExtruder script to newly created GameObject and keep track of its reference
        polyExtruder = polyExtruderGO.AddComponent<PolyExtruder>();

        // global PolyExtruder configurations
        polyExtruder.isOutlineRendered = isOutlineRendered;
        Vector3 pos = polyExtruderGO.transform.position;
        pos.y += offsetYBackgroundGeom;
        polyExtruderGO.transform.position = pos;
        polyExtruder.createPrism(polyExtruderGO.name, extrusionHeight, MeshDataPoints, Color.grey, is3D, isUsingBottomMeshIn3D, isUsingCollider);
        // if (isUsingCollider)
        // {
        //     MeshCollider mc = polyExtruderGO.AddComponent<MeshCollider>();
        //     mc.sharedMesh = polyExtruder.surroundMesh;
        // }
        return polyExtruderGO;


    }

    public Dictionary<int, GameObject> GetGeneratedBuildings()
    {
        return buildingsMap;
    }

    // public static void SetGeneratedBuildings(Dictionary<int, GameObject> buildings)
    // {
    //     buildingsMap = buildings;
    // }
}


