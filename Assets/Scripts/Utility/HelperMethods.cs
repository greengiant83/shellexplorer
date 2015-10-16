using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class HelperMethods
{
    public static Bounds BoundsFromPoints(IEnumerable<Vector3> points)
    {
        Bounds bounds = new Bounds();
        bounds.center = points.First();
        bounds.size = Vector3.zero;
        Debug.Log("Center: " + bounds.center);
        foreach(var point in points.Skip(1))
        {
            Debug.Log("Encapsulate " + point);
            bounds.Encapsulate(point);
        }
        Debug.Log("Inner bounds: " + bounds);
        return bounds;
    }

    public static Vector3[] Corners(this Bounds bounds)
    {
        return new Vector3[]
        {
            new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z) + bounds.center,
            new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z) + bounds.center,
            new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z) + bounds.center,
            new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z) + bounds.center,
            new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z) + bounds.center,
            new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z) + bounds.center,
            new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z) + bounds.center,
            new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z) + bounds.center,
        };
    }

    public static Vector3 WorldToLocal(this Vector3 worldPosition, Transform transform)
    {
        return transform.parent.InverseTransformPoint(worldPosition);
    }

    public static Quaternion WorldToLocal(this Quaternion worldRotation, Transform transform)
    {
        //thisLocal = thisWorld - parentWorld
        return Quaternion.Inverse(transform.parent.rotation) * worldRotation;
    }

    public static float GetGroundLevel(this Terrain terrain, Vector3 worldPosition) //Returns the world y coordinate of the ground at the given worldPosition
    {
        var terrainLocalPos = worldPosition - terrain.transform.position;
        var normalizedPos = new Vector2(Mathf.InverseLerp(0.0f, terrain.terrainData.size.x, terrainLocalPos.x),
            Mathf.InverseLerp(0.0f, terrain.terrainData.size.z, terrainLocalPos.z));

        var terrainNormal = terrain.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);
        var terrainHeight = terrain.SampleHeight(worldPosition);


        return terrainHeight + terrain.transform.position.y;
    }

    public static Vector3 GetGroundNormal(this Terrain terrain, Vector3 worldPosition) //Returns the world y coordinate of the ground at the given worldPosition
    {
        var terrainLocalPos = worldPosition - terrain.transform.position;
        var normalizedPos = new Vector2(Mathf.InverseLerp(0.0f, terrain.terrainData.size.x, terrainLocalPos.x),
            Mathf.InverseLerp(0.0f, terrain.terrainData.size.z, terrainLocalPos.z));

        var terrainNormal = terrain.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);
        var terrainHeight = terrain.SampleHeight(worldPosition);

        return terrainNormal;
        //return terrainHeight + terrain.transform.position.y;
    }

    public static T GetComponentInAncestor<T>(this Transform transform) where T : Component
    {
        var component = transform.GetComponentInParent<T>();
        if (component == null && transform.parent != null)
            return transform.parent.GetComponentInAncestor<T>();
        else
            return component;
    }

    public static void DestroyChildren(this Transform transform)
    {
        while (transform.childCount > 0) GameObject.Destroy(transform.GetChild(0));
    }

    public static IEnumerable<GameObject> Children(this GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            yield return parent.transform.GetChild(i).gameObject;
        }
    }

    public static IEnumerable<Transform> Children(this Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            yield return parent.GetChild(i);
        }
    }

    public static IEnumerable<GameObject> Descendants(this GameObject parent)
    {
        foreach(var child in parent.Children())
        {
            yield return child;

            foreach (var grandchild in child.Descendants()) yield return grandchild;
        }
    }

    public static T GetInterface<T>(this GameObject item)
    {
        return (T)(object)item.GetComponent(typeof(T));
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2, bool clamp = false)
    {
        var v = (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        if (clamp)
        {
            if(from2 > to2)
                v = Mathf.Clamp(v, to2, from2);
            else
                v = Mathf.Clamp(v, from2, to2);
        }
        return v;
    }


    public static float MapToBuckets(this float value, float from1, float to1, float from2, float to2, int numberOfBuckets)
    {
        value = (int)value.Remap(from1, to1, 0, numberOfBuckets);
        value = value.Remap(0, numberOfBuckets, from2, to2);
        return value;
    }

    public static Vector3 Scaled(this Vector3 v, Vector3 scale)
    {
        v.Scale(scale);
        return v;
    }
}
