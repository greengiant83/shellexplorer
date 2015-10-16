using UnityEngine;
using System.Collections;
using System.Linq;

public class PipeTest : MonoBehaviour 
{
    public Transform[] Path;

    DynamicMesh mesh;
    MeshData source;

	void Start () 
    {
        mesh = GetComponent<DynamicMesh>();
        var profile = new MeshData();
        profile.Vertices.AddRange(new Vector3[]
        {
            new Vector3(0.1f, 0.1f, 0),
            new Vector3(-0.1f, 0.1f, 0),
            new Vector3(-0.1f, -0.1f, 0),
            new Vector3(0.1f, -0.1f, 0)
        });
        source = new MeshData();
        source.Vertices.AddRange(Path.Select(i => i.position));

        mesh.SourceMesh = source;
        mesh.Modifiers.Add(new ExtrudeAlongPathModifier() { Profile = profile });

        

        mesh.UpdateMesh();
	}
	
	void Update () 
    {
	    for(int i=0;i<source.Vertices.Count;i++)
        {
            source.Vertices[i] = Path[i].position;
        }
        mesh.UpdateMesh();
	}
}
