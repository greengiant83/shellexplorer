using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MeshData 
{
    public List<Vector3> Vertices;
    public List<Vector3> Rotations;
    public List<Vector3> Scales;

    public List<int> Triangles;
    public List<Vector3> Normals;
    public List<Vector2> UVs;

    public MeshData()
    {
        Vertices = new List<Vector3>();
        Triangles = new List<int>();
    }

    public static explicit operator MeshData(Mesh mesh)
    {
        var meshData = new MeshData();
        meshData.Vertices = mesh.vertices.ToList();
        meshData.Triangles = mesh.triangles.ToList();
        meshData.UVs = mesh.uv.ToList();
        meshData.Normals = mesh.normals.ToList();
        return meshData;
    }

    public static explicit operator Mesh(MeshData meshData)
    {
        var mesh = new Mesh();
        meshData.UpdateMesh(mesh);
        return mesh;
    }

    public void UpdateMesh(Mesh mesh)
    {
        if (Vertices != null) mesh.vertices = Vertices.ToArray();

        if (Triangles != null) mesh.triangles = Triangles.ToArray();

        if (UVs != null) mesh.uv = UVs.ToArray();

        if (Normals != null)
            mesh.normals = Normals.ToArray();
        else
            mesh.RecalculateNormals();

        mesh.RecalculateBounds();
    }
}