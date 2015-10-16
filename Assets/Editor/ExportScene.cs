using UnityEngine;
using UnityEditor;
using System.Collections;

public class ExportManager
{
    [MenuItem("Export/Scene")]
    private static void ExportScene()
    {
        var prop = new HierarchyProperty(HierarchyType.GameObjects);
        var expanded = new int[0];
        while (prop.Next(expanded))
        {
            var gameObject = prop.pptrValue as GameObject;
            examineGameObject(gameObject);
        }
    }

    private static void examineGameObject(GameObject gameObject, int depth = 0)
    {
        var indent = new string('\t', depth);
        Debug.Log(indent + gameObject.name);

        var components = gameObject.GetComponents<Component>();
        foreach (var component in components)
        {
            Debug.Log(indent + "---" + component.GetType().Name);
            examineComponent(component);
        }

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            var child = gameObject.transform.GetChild(i).gameObject;
            examineGameObject(child, depth + 1);
        }
    }

    private static void examineComponent(Component component)
    {
        
    }
}
