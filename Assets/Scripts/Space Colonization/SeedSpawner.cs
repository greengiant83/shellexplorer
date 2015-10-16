using UnityEngine;
using System.Collections;

public class SeedSpawner : MonoBehaviour 
{
    public DynamicMesh GrowthObject;

    Hand hand;

	void Start () 
    {
        hand = transform.parent.gameObject.GetComponent<Hand>();
	}
	
	void Update () 
    {
        if (hand.Controller == null) return;

        if(hand.Controller.GetButtonDown(SixenseButtons.TRIGGER))
        {
            GrowthObject.SourceMesh.Vertices.Add(GrowthObject.transform.InverseTransformPoint(this.transform.position));
            GrowthObject.UpdateMesh();
            //var seed = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //seed.tag = "Seed";
            //seed.name = "Seed";
            //seed.transform.position = this.transform.position;
            //seed.transform.rotation = this.transform.rotation;
            //seed.transform.localScale = Vector3.one * 0.01f;
        }	

        if(hand.Controller.GetButtonDown(SixenseButtons.FOUR))
        {
            //var seeds = GameObject.FindGameObjectsWithTag("Seed");
            //foreach (var seed in seeds) Destroy(seed);
            GrowthObject.SourceMesh.Vertices.Clear();
            GrowthObject.UpdateMesh();
        }
	}
}
