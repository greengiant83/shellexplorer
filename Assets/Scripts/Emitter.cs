using UnityEngine;
using System.Collections;

public class Emitter : MonoBehaviour 
{
    public GameObject Item;
    public float IntervalInSeconds = 1f;

	void Start () 
    {
        InvokeRepeating("Tick", 0.0001f, IntervalInSeconds);
	}

    void Tick()
    {
        var clone = Instantiate<GameObject>(Item);
        clone.transform.position = transform.TransformPoint(Random.insideUnitSphere * 0.5f);
        clone.transform.localScale = Item.transform.localScale * Random.Range(0.5f, 2f);
        clone.transform.rotation = Random.rotation;
    }
}
