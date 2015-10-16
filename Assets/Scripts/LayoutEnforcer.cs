using UnityEngine;
using System.Collections;

public class LayoutEnforcer : MonoBehaviour 
{
    public Vector3 TargetPosition;

	void Start () 
    {
	
	}
	
	void Update () 
    {
        transform.position = Vector3.Lerp(transform.position, TargetPosition, 0.01f);
	}
}
