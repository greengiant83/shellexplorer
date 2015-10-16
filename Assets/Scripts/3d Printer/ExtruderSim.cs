using UnityEngine;
using System.Collections;
using System.Linq;

public class ExtruderSim : MonoBehaviour 
{
    public DynamicMesh ExtrusionResult;
    public float StepSize = 0.1f;
    public float NozzleSize = 0.4f / 1000;

    MeshData pathSource;
    Vector3 lastPosition;

	void Start () 
    {
        var profile = new MeshData();
        profile.Vertices.AddRange(new Vector3[]
        {
            new Vector3(0.5f, 0.5f, 0) * NozzleSize,
            new Vector3(-0.5f, 0.5f, 0) * NozzleSize,
            new Vector3(-0.5f, -0.5f, 0) * NozzleSize,
            new Vector3(0.5f, -0.5f, 0) * NozzleSize
        });
        pathSource = new MeshData();
        pathSource.Vertices.Add(transform.position);

        ExtrusionResult.SourceMesh = pathSource;
        ExtrusionResult.Modifiers.Add(new ExtrudeAlongPathModifier() { Profile = profile });
        ExtrusionResult.UpdateMesh();
	}
	
	void Update () 
    {
        var lastPosition = pathSource.Vertices.Last();
        var distance = (transform.position - lastPosition).magnitude;
        if (distance >= StepSize)
        {
            pathSource.Vertices.Add(transform.position);
            ExtrusionResult.UpdateMesh();
        }
	}
}
