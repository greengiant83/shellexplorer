using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Duplicates the mesh data from DuplicationSourceMesh at each vertex from the input mesh data
/// </summary>
public class DuplivertsModifier : MeshModifier
{
    public MeshData DuplicationSourceMesh;

    public override MeshData Execute(MeshData input)
    {
        MeshData output = new MeshData();

        foreach (var inputVertex in input.Vertices)
        {
            int vertexOffset = output.Vertices.Count;

            //Vertices
            foreach (var sourceVertex in DuplicationSourceMesh.Vertices)
            {
                output.Vertices.Add(sourceVertex + inputVertex);
            }

            //Triangles
            foreach (var sourceTriangle in DuplicationSourceMesh.Triangles)
            {
                output.Triangles.Add(sourceTriangle + vertexOffset);
            }

            //Rotations
            if (DuplicationSourceMesh.Rotations != null)
            {
                if (output.Rotations == null) output.Rotations = new List<Vector3>();
                foreach (var sourceRotation in DuplicationSourceMesh.Rotations)
                {
                    output.Rotations.Add(sourceRotation);
                }
            }

            //Scales
            if (DuplicationSourceMesh.Scales != null)
            {
                if (output.Scales == null) output.Scales = new List<Vector3>();
                foreach (var sourceScale in DuplicationSourceMesh.Scales)
                {
                    output.Scales.Add(sourceScale);
                }
            }



            //Normals
            if (DuplicationSourceMesh.Normals != null)
            {
                if (output.Normals == null) output.Normals = new List<Vector3>();
                foreach (var sourceNormal in DuplicationSourceMesh.Normals)
                {
                    output.Normals.Add(sourceNormal);
                }
            }

            //UVs
            if (DuplicationSourceMesh.UVs != null)
            {
                if (output.UVs == null) output.UVs = new List<Vector2>();
                foreach (var sourceUv in DuplicationSourceMesh.UVs)
                {
                    output.UVs.Add(sourceUv);
                }
            }
        }

        return output;
    }
}