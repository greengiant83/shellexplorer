using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DynamicMesh : MonoBehaviour 
{
    public MeshData SourceMesh = new MeshData();
    public List<MeshModifier> Modifiers = new List<MeshModifier>();

    MeshFilter meshFilter;

	void Start () 
    {
	}

	void Update () 
    {
	
	}

    public void UpdateMesh()
    {
        if (meshFilter == null) meshFilter = gameObject.GetComponent<MeshFilter>();

        var resultMesh = SourceMesh;
        foreach(var modifier in Modifiers)
        {
            resultMesh = modifier.Execute(resultMesh);
        }
        resultMesh.UpdateMesh(meshFilter.mesh);
    }
}
