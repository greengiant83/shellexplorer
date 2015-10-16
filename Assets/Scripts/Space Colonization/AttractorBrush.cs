using UnityEngine;
using System.Collections;

public class AttractorBrush : MonoBehaviour 
{
    public int PointCount = 10;
    Hand hand;

	void Start () 
    {
        hand = transform.parent.parent.GetComponent<Hand>();
	}
	
	void Update () 
    {
        if(hand.Controller != null)
        {
            var brushScale = transform.localScale.x;
            brushScale *= hand.Controller.JoystickX * 0.01f + 1;

            transform.localScale = brushScale * Vector3.one;
            transform.localPosition += Vector3.forward * hand.Controller.JoystickY * 0.01f;
        }

	    if(this.transform.childCount < PointCount)
        {
            var newChildCount = PointCount - transform.childCount;
            for(int i=0;i<newChildCount;i++)
            {
                var attractor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                attractor.transform.SetParent(this.transform);
                attractor.transform.localPosition = Random.insideUnitSphere * 0.5f;
                attractor.transform.localScale = Vector3.one * 0.05f;
                attractor.tag = "Attractor";
            }
        }
	}
}
