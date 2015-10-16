using UnityEngine;
using System.Collections;

public class ExtrudeAlongPathModifier : MeshModifier
{
    public MeshData Profile;

    public override MeshData Execute(MeshData input)
    {
        MeshData output = new MeshData();

        if (input.Vertices.Count < 2) return output;

        //Calculate rotations
        Quaternion[] rots = new Quaternion[input.Vertices.Count];
        for (int i = 0; i < rots.Length-1;i++)
        {
            rots[i] = Quaternion.LookRotation(input.Vertices[i + 1] - input.Vertices[i]);
        }

        rots[rots.Length - 1] = rots[rots.Length - 2];

        for (int i = 1; i < rots.Length-1;i++)
        {
            rots[i] = Quaternion.Slerp(rots[i], rots[i - 1], 0.5f);
        }

        //Add points
        for (int v = 0; v < rots.Length;v++)
        {
            for(int p=0;p<Profile.Vertices.Count;p++)
            {
                var point = rots[v] * Profile.Vertices[p] + input.Vertices[v];
                output.Vertices.Add(point);
                //MeshHelper.addCubeAtPoint(point, Vector3.one * 0.1f, output);
            }
        }

        //Add triangles
        for (int s = 0; s < rots.Length-1;s++)
        {
            for(int p=0;p<Profile.Vertices.Count-1;p++)
            {
                MeshHelper.AddQuad(
                    s * Profile.Vertices.Count + p, 
                    (s+1) * Profile.Vertices.Count + p,
                    (s+1) * Profile.Vertices.Count + (p + 1),
                    s * Profile.Vertices.Count + p + 1,
                    output);
            }
            
            MeshHelper.AddQuad(
                s * Profile.Vertices.Count + Profile.Vertices.Count - 1,
                (s + 1) * Profile.Vertices.Count + Profile.Vertices.Count - 1,
                (s + 1) * Profile.Vertices.Count,
                s * Profile.Vertices.Count,
                output);
        }
        return output;
    }
}
