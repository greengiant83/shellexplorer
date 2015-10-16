using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Grower : MonoBehaviour 
{
    public DynamicMesh GrowthObject;

    Hand hand;
    Collider collider;
    bool isActive;
    float seedSize = 0.01f;
    
	void Start () 
    {
        MeshData seedMeshData = new MeshData();
        MeshHelper.addCube(Vector3.one * seedSize, seedMeshData);

        GrowthObject.Modifiers.Add(new DuplivertsModifier()
        {
            DuplicationSourceMesh = seedMeshData
        });
        GrowthObject.UpdateMesh();

        hand = GetComponentInParent<Hand>();
        collider = GetComponent<Collider>();
	}
	
	void Update () 
    {
        if (hand.Controller == null) return;

        if(hand.Controller.GetButtonDown(SixenseButtons.TRIGGER))
        {
            CancelInvoke("iterate");
            InvokeRepeating("iterate", 0.0001f, 0.1f);
            isActive = true;
        }
        if(hand.Controller.GetButtonUp(SixenseButtons.TRIGGER))
        {
            CancelInvoke("iterate");
            isActive = false;
        }
	}

    void iterate()
    {
        var attractors = GameObject.FindGameObjectsWithTag("Attractor");
        var seeds = GrowthObject.SourceMesh.Vertices.Where(seed => collider.bounds.Contains(GrowthObject.transform.TransformPoint(seed))).ToArray();

        if (seeds.Count() <= 0 || attractors.Count() <= 0) return;

        var activeSeeds = new Dictionary<Vector3, List<GameObject>>();

        //Find seeds that are close to attractors
        foreach(var attractor in attractors)
        {
            var closetSeed = seeds.OrderBy(seed => (seed - GrowthObject.transform.InverseTransformPoint(attractor.transform.position)).magnitude).First();
            if(!activeSeeds.ContainsKey(closetSeed))
            {
                activeSeeds.Add(closetSeed, new List<GameObject>(new GameObject[] { attractor }));
            }
            else
            {
                activeSeeds[closetSeed].Add(attractor);
            }
        }

        //Loop over seeds and spawn new seed 
        foreach(var item in activeSeeds)
        {
            var seed = item.Key;
            var netForce = Vector3.zero;
            foreach (var attractor in item.Value)
                netForce += GrowthObject.transform.InverseTransformPoint(attractor.transform.position) - seed;
            netForce.Normalize();

            GrowthObject.SourceMesh.Vertices.Add(seed + netForce * seedSize);
        }

        //Kill any attractors with seeds to close
        foreach (var attractor in attractors)
        {
            var killRadius = attractor.transform.localScale.x;
            if (seeds.Any(seed => (seed - GrowthObject.transform.InverseTransformPoint(attractor.transform.position)).magnitude < killRadius))
                Destroy(attractor);
        }

        GrowthObject.UpdateMesh();
    }
}
