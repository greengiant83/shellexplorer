using UnityEngine;
using System.Collections;

public class MidasTouch : MonoBehaviour 
{
	void Start () 
    {
	}
	
	void Update () 
    {
	
	}

    void OnTriggerEnter(Collider collider)
    {
        collider.gameObject.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
    }
}
