using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MyButton1 : MonoBehaviour, IClickable
{
    


	void Start () {
	
	}
	
	void Update () 
    {
	
	}

    public void OnClick(HandInput Source)
    {
        var selectedObjects = UIManager.Instance.GetSelectedObjects();
        foreach (var item in selectedObjects)
        {
            var rigidbody = item.GetComponent<Rigidbody>();
            if (rigidbody != null)
                rigidbody.AddForce(new Vector3(-100f, 0, 0));
            else
                item.transform.Translate(-0.1f, 0, 0);
        }
    }
}


