using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PanelContainer : MonoBehaviour 
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
        //TODO: Need to figure out a way to decide when to update. Updating every frame even if nothing has changed is stupid
        UpdateLayout();
	}

    void UpdateLayout()
    {
        var backgroundTransform = transform.FindChild("Background Box");
        if (backgroundTransform != null) background = backgroundTransform.gameObject;

        var childObjects = gameObject.Descendants().Where(i => i != background);
        var childCorners = childObjects.SelectMany(i => i.GetRelativeCorners(this.transform));
        var bounds = HelperMethods.BoundsFromPoints(childCorners);

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
