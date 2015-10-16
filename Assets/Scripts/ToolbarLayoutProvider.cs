using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ToolbarLayoutProvider : MonoBehaviour 
{
	void Start () 
    {
	
	}
	
	void Update () 
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.transform.localPosition = Vector3.right * i * -0.25f;
        }	
	}
}
