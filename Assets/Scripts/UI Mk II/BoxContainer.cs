using UnityEngine;
using System.Collections;
using System.Linq;

[ExecuteInEditMode]
public class BoxContainer : MonoBehaviour 
{
    public float Thickness = 0.01f;
    public float StandOff = 0.01f;
    public float Padding = 0.04f;

    GameObject background;

	void Start () 
    {
        UpdateLayout();
	}
	
	void Update () 
    {
        UpdateLayout();
	}

    void UpdateLayout()
    {
        var backgroundTransform = transform.FindChild("Background Box");
        if (backgroundTransform != null) background = backgroundTransform.gameObject;

        Bounds bounds = new Bounds();
        bool isFirst = true;
        foreach(var item in gameObject.Descendants())
        {
            if (item == background) continue;

            var meshFilter = item.GetComponent<MeshFilter>();
            if(meshFilter != null)
            {
                var itemBounds = meshFilter.sharedMesh.bounds;
                var corners = itemBounds.Corners().Select(i => transform.InverseTransformPoint(item.transform.TransformPoint(i)));

                foreach(var corner in corners)
                {
                    if(isFirst)
                    {
                        bounds.center = corner;
                        bounds.size = Vector3.zero;
                        isFirst = false;
                    }
                    else
                    {
                        bounds.Encapsulate(corner);
                    }
                }
            }
        }

        //bounds coordinates are relative to this.transform

        if (background == null)
        {
            background = GameObject.CreatePrimitive(PrimitiveType.Cube);
            background.name = "Background Box";
            background.transform.SetParent(this.transform, false);
        }

        
        background.transform.localPosition = bounds.center + new Vector3(0, 0, bounds.extents.z + Thickness/2 + StandOff);
        background.transform.localScale = new Vector3(bounds.size.x + Padding * 2, bounds.size.y + Padding * 2, Thickness);
        
    }
}
