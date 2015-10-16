using UnityEngine;
using System.Collections;

public class SpringCluster : MonoBehaviour 
{
    public Material[] LineMaterials;
     
	void Start () 
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var childA = transform.GetChild(i);

            var lineMaterial = LineMaterials[Random.Range(0, LineMaterials.Length)];

            childA.GetComponent<Rigidbody>().mass = childA.transform.localScale.x * childA.transform.localScale.y * childA.transform.localScale.z;

            for (int j = i+1; j < transform.childCount; j++)
            {
                var childB = transform.GetChild(j);
                if (childA != childB)
                {
                    if (Random.Range(0.0f, 1.0f) > 0.15f) continue;

                    var spring = childA.gameObject.AddComponent<SpringJoint>();
                    spring.autoConfigureConnectedAnchor = false;
                    spring.anchor = Vector3.zero;
                    spring.connectedAnchor = Vector3.zero;
                    spring.connectedBody = childB.gameObject.GetComponent<Rigidbody>();
                    spring.enableCollision = true;
                    spring.minDistance = spring.maxDistance = 15;
                    spring.spring = Random.Range(2f, 10f);

                    var line = childA.gameObject.AddComponent<LinkLine>();
                    line.LinkedObject = childB;
                    line.Material = lineMaterial;
                    line.transform.SetParent(this.transform);
                }                
            }
        }
	}

    void Update() 
    {
        return;
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            var springs = child.GetComponents<SpringJoint>();
            foreach (var spring in springs)
            {
                Debug.DrawLine(
                    child.transform.TransformPoint(spring.anchor),
                    spring.connectedBody.transform.TransformPoint(spring.connectedAnchor),
                    Color.red);
            }
        }
	}
}
