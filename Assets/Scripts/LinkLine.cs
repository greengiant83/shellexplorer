using UnityEngine;
using System.Collections;

public class LinkLine : MonoBehaviour 
{
    public Transform LinkedObject;
    public Material Material;
    public float LineThickness = 0.01f;

    GameObject pivot;
    GameObject visual;

    Vector3 inverseScale
    {
        get
        {
            return new Vector3(
                1 / transform.lossyScale.x,
                1 / transform.lossyScale.y,
                1 / transform.lossyScale.z);
        }
    }
	
	void Start () 
    {
        pivot = new GameObject("Line Pivot");
        pivot.transform.SetParent(this.transform, false);

        visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.name = "Line";
        visual.GetComponent<Renderer>().material = Material;
        visual.transform.SetParent(pivot.transform, false);
        visual.transform.localScale = new Vector3(LineThickness, LineThickness, LineThickness).Scaled(inverseScale);
        visual.transform.localPosition = new Vector3(0, 0, 0);
        
        Destroy(visual.GetComponent<Collider>());
	}
	
	// Update is called once per frame
	void Update () 
    {
        var v = (LinkedObject.position - transform.position).Scaled(inverseScale);
        visual.transform.localScale = new Vector3(visual.transform.localScale.x, visual.transform.localScale.y, v.magnitude);
        visual.transform.localPosition = new Vector3(0, 0, v.magnitude / 2);
        pivot.transform.rotation = Quaternion.LookRotation(v);
	}
}
