using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class AvailablePrefabs
{
    private static bool isInitialized = false;
    private static List<GameObject> list;

    public static List<GameObject> List
    {
        get
        {
            if (!isInitialized) Initialize();
            return list;
        }
    }

    public static void Initialize()
    {
        list = new List<GameObject>();
        var prefabNames = new string[]
        {
            "Hex Tile",
            "Sphere",
            "Projectile"
        };

        foreach (var name in prefabNames)
        {
            var prefab = (GameObject)Resources.Load("Prefab/" + name);
            list.Add(prefab);
        }
        isInitialized = true;

        //availablePrefabs = new List<GameObject>();
        //DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Prefab");
        //FileInfo[] files = dir.GetFiles("*.prefab");
        //foreach(var file in files)
        //{
        //    var prefab = (GameObject)Resources.Load("Prefab/" + Path.GetFileNameWithoutExtension(file.Name) );
        //    availablePrefabs.Add(prefab);
        //}
    }
}
