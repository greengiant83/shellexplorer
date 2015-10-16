using UnityEngine;
using System.Collections;

public class DotExplorer : MonoBehaviour 
{
    public Transform TravelObject;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        var strokeDirection = TravelObject.position - transform.position;
        var dot = Vector3.Dot(strokeDirection, transform.forward);
        var isFlipped = dot < 0;

        Debug.DrawRay(transform.position, transform.forward, Color.blue);
        Debug.DrawRay(transform.position, strokeDirection, isFlipped ? Color.red : Color.green);
        Debug.Log(dot);
	}
}
