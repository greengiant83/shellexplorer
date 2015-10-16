using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public static class MeshHelper
{

    #region -- Wedge --
    public static Mesh CreateWedgeMesh(Vector3 axis, float thickness, float radius, float startAngle, float endAngle, int segments)
    {
        return CreateMeshWrapper(meshData => AddWedge(axis, thickness, radius, startAngle, endAngle, segments, meshData));
    }

    public static void AddWedge(Vector3 axis, float thickness, float radius, float startAngle, float endAngle, int segments, MeshData meshData)
    {
        var vertices = meshData.Vertices;
        var triangles = meshData.Triangles;
        var sweepVector = new Vector3(0, radius, 0);
        var centerPoint = Vector3.zero;
        var thicknessOffset = axis * (thickness / 2.0f);
        float angleSpread = endAngle - startAngle;
        float rotationStep = angleSpread / (segments - 1);

        var points = new List<Vector3>();
        for (int i = 0; i < segments; i++)
        {
            var point = Quaternion.Euler((startAngle + i * rotationStep) * axis) * sweepVector;
            points.Add(point);
        }

        int baseIndex = vertices.Count;

        //Add front face
        int frontCenter = vertices.Count;
        var frontTopPoints = new List<Vector3>();
        vertices.Add(centerPoint + thicknessOffset);
        vertices.Add(points[0] + thicknessOffset);
        frontTopPoints.Add(points[0] + thicknessOffset);
        for (int i = 1; i < points.Count; i++)
        {
            var point = points[i] + thicknessOffset;
            frontTopPoints.Add(point);
            vertices.Add(point);
            triangles.AddRange(new int[] { frontCenter + i, frontCenter + i + 1, frontCenter });
        }

        //Add back face
        int backCenter = vertices.Count;
        var backTopPoints = new List<Vector3>();
        vertices.Add(centerPoint - thicknessOffset);
        vertices.Add(points[0] - thicknessOffset);
        backTopPoints.Add(points[0] - thicknessOffset);
        for (int i = 1; i < points.Count; i++)
        {
            var point = points[i] - thicknessOffset;
            backTopPoints.Add(point);
            vertices.Add(point);
            triangles.AddRange(new int[] { backCenter, backCenter + i + 1, backCenter + i });
        }

        //Add side faces
        AddQuad(vertices[frontCenter], vertices[frontCenter + 1], vertices[backCenter + 1], vertices[backCenter], meshData);
        AddQuad(vertices[frontCenter], vertices[frontCenter + segments], vertices[backCenter + segments], vertices[backCenter], meshData, true);

        //Add round face across top
        AddBridgeBetweenPointSets(frontTopPoints.ToArray(), backTopPoints.ToArray(), meshData);

        //addCubeAtPoints(points, new Vector3(0.05f, 0.05f, 0.05f), vertices, triangles);
        //addCubeAtPoint(points[0], new Vector3(0.1f, 0.1f, 0.1f), vertices, triangles);
    }

    public static void PrintPoints(IEnumerable<Vector3> points)
    {
        int i = 0;
        foreach (var point in points)
        {
            Debug.Log("Point " + i + ": " + point);
            i++;
        }
    }
    #endregion

    #region -- Plane --
    public static Mesh CreatePlane(Vector2 size, Vector2 segments)
    {
        return CreateMeshWrapper(meshData => AddPlane(size, segments, meshData));
    }

    public static void AddPlane(Vector2 size, Vector2 segments, MeshData meshData)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region -- Polygon --
    public static Mesh CreatePolygon(Vector3[] points, bool doubleSided)
    {
        return CreateMeshWrapper(meshData => AddPolygon(points, doubleSided, meshData));
    }

    //Expects points to arranged as a triangle strip
    public static void AddPolygon(Vector3[] points, bool doubleSided, MeshData meshData)
    {
        var vertices = meshData.Vertices;
        var triangles = meshData.Triangles;

        //Front face
        int rootIndex = vertices.Count;
        foreach (var point in points) vertices.Add(point);

        for (int i = 0; i < points.Length - 2; i++)
        {
            if(i % 2 == 0)
                triangles.AddRange(new int[] { rootIndex+i, rootIndex+i+1, rootIndex+i+2 });
            else
                triangles.AddRange(new int[] { rootIndex + i, rootIndex + i + 2, rootIndex + i + 1 });
        }

        //Back face
        if (doubleSided)
        {
            rootIndex = vertices.Count;
            foreach (var point in points) vertices.Add(point);

            for (int i = 0; i < points.Length - 2; i++)
            {
                if (i % 2 == 1)
                    triangles.AddRange(new int[] { rootIndex + i, rootIndex + i + 1, rootIndex + i + 2 });
                else
                    triangles.AddRange(new int[] { rootIndex + i, rootIndex + i + 2, rootIndex + i + 1 });
            }
        }
    }

    public static void UpdatePolygon(Mesh mesh, Vector3[] points, bool doubleSided)
    {
        var vertices = mesh.vertices;
        for (int i = 0; i < points.Length; i++)
        {
            vertices[i] = points[i];
            if (doubleSided) vertices[i + points.Length] = points[i];
        }
        mesh.vertices = vertices;
    }
    #endregion

    #region -- Torus --
    public static Mesh CreateTorusMesh(float majorRadius, float minorRadius, int majorSegments, int minorSegments)
    {
        return CreateMeshWrapper(meshData => AddTorus(majorRadius, minorRadius, majorSegments, minorSegments, meshData));
    }

    public static void AddTorus(float majorRadius, float minorRadius, int majorSegments, int minorSegments, MeshData meshData)
    {
        var vertices = meshData.Vertices;
        var triangles = meshData.Triangles;
        var majorAxis = new Vector3(0, 0, 1);
        var minorAxis = new Vector3(1, 0, 0);
        float majorAngleStep = 360.0f / majorSegments;

        List<Vector3[]> minorCircles = new List<Vector3[]>();
        int startingIndex = vertices.Count;
        for (int i = 0; i < majorSegments; i++)
        {
            var minorPoints = GetCircleOfPoints(minorRadius, minorSegments, minorAxis);
            minorPoints.Translate(new Vector3(0, majorRadius, 0));
            //translatePoints(minorPoints, new Vector3(0, majorRadius, 0));
            minorPoints.Rotate(Quaternion.Euler(majorAxis * (majorAngleStep * i)));
            minorCircles.Add(minorPoints.ToArray());
            vertices.AddRange(minorPoints);

            ////Draw cubes at points for debugging
            //addCubeAtPoints(minorPoints, new Vector3(0.25f, 0.25f, 0.25f), vertices, triangles);
        }

        for (int i = 1; i < minorCircles.Count; i++)
        {

            //addBridgeBetweenPointSets (minorCircles [i - 1], minorCircles [i], vertices, triangles);
            AddTriangleBridgeBetweenIndices(range(startingIndex + (i - 1) * minorSegments, minorSegments).ToArray(),
                                            range(startingIndex + i * minorSegments, minorSegments).ToArray(),
                                            triangles);
        }
        //addBridgeBetweenPointSets(minorCircles[minorCircles.Count - 1], minorCircles[0], vertices, triangles);
        AddTriangleBridgeBetweenIndices(range(startingIndex + (minorCircles.Count - 1) * minorSegments, minorSegments).ToArray(),
                                        range(startingIndex, minorSegments).ToArray(),
                                        triangles);
    }

    public static void AddTriangleBridgeBetweenIndices(int[] pointsA, int[] pointsB, List<int> triangles)
    {
        int len = pointsA.Length;
        for (int i = 1; i < len; i++)
        {
            triangles.AddRange(new int[] { pointsA[i - 1], pointsB[i - 1], pointsB[i] });
            triangles.AddRange(new int[] { pointsA[i - 1], pointsB[i], pointsA[i] });
        }

        triangles.AddRange(new int[] { pointsA[len - 1], pointsB[len - 1], pointsB[0] });
        triangles.AddRange(new int[] { pointsA[len - 1], pointsB[0], pointsA[0] });
    }

    public static IEnumerable<int> range(int startValue, int count)
    {
        return Enumerable.Range(startValue, count);
    }
    #endregion

    #region -- Fibonacci Spiral Plane --
    /// <summary>
    /// TODO: Incomplete
    /// </summary>
    public static Mesh CreateFibonacciSpiralPlane(float Radius, Vector3 axis, Vector3 axisOrtho)
    {
        return CreateMeshWrapper(meshData => AddFibonacciSpiralPlane(Radius, axis, axisOrtho, meshData));
    }

    public static void AddFibonacciSpiralPlane(float Radius, Vector3 axis, Vector3 axisOrtho, MeshData meshData)
    {
        var vertices = meshData.Vertices;
        var triangles = meshData.Triangles;
        float scale = 0.0005f;
        float phi = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;
        float b = Mathf.Log(phi) / (Mathf.PI / 2.0f);
        float e = (float)Math.E;
        float t = 0;

        for (float c = 0; c <= 360; c += 360 / 5 )
        {
            t = 0;
            for (int i = 0; i < 200; i++)
            {
                float r = Mathf.Pow(e, b * t) * scale;

                var point = Quaternion.AngleAxis(t * Mathf.Rad2Deg + c, axis) * (axisOrtho * r);
                addCubeAtPoint(point, Vector3.one * 0.01f, meshData);

                t += Mathf.Deg2Rad * 10;
            }
        }
    }
    #endregion

    #region -- Concentric Circular Plane --
    public static Mesh CreateCircularPlane(float radius, int rings, int segments, Vector3 axis)
    {
        return CreateMeshWrapper(meshData => AddCircularPlane(radius, rings, segments, axis, meshData));
    }

    public static void AddCircularPlane(float radius, int rings, int segments, Vector3 axis, MeshData meshData)
    {
        var vertices = meshData.Vertices;
        var triangles = meshData.Triangles;
        float radiusScalar = radius / rings;
        var ringPoints = Enumerable.Range(1, rings).Select(i => GetCircleOfPoints(i * radiusScalar, segments, axis)).ToArray();

        int startingIndex = vertices.Count;
        vertices.Add(Vector3.zero);

        //add points to vertex list
        foreach (var circle in ringPoints) foreach (var point in circle) vertices.Add(point);
        

        //Add triangles from center point to the first ring
        for (int i = 2; i <= segments; i++)
        {
            triangles.AddRange(new int[] { startingIndex, startingIndex + i - 1, startingIndex + i });
        }
        //Add the last triangle
        triangles.AddRange(new int[] { startingIndex, startingIndex + segments, startingIndex + 1 });

        //Now add the rest of the rings
        for(int r=1;r<rings;r++)
        {
            int ringIndex = r * segments + 1 + startingIndex;
            int prevRingIndex = (r - 1) * segments + 1 + startingIndex;
            for(int s=1;s<segments;s++)
            {
                triangles.AddRange(new int[] 
                { 
                    ringIndex  + s - 1,
                    ringIndex + s,
                    prevRingIndex + s - 1
                });

                triangles.AddRange(new int[] 
                { 
                    prevRingIndex + s - 1,
                    ringIndex + s,
                    prevRingIndex + s
                });
            }

            //Add final quad
            triangles.AddRange(new int[] 
            { 
                ringIndex + segments-1,
                ringIndex + 0,
                prevRingIndex + segments-1
            });

            triangles.AddRange(new int[] 
            { 
                prevRingIndex + segments-1,
                ringIndex + 0,
                prevRingIndex + 0
            });
        }
    }
    #endregion

    #region -- Cube --
    public static Mesh createCubeMesh(Vector3 size)
    {
        return CreateMeshWrapper(meshData => addCube(size, meshData));
    }

    public static void addCube(Vector3 size, MeshData meshData)
    {
        var vertices = meshData.Vertices;
        var triangles = meshData.Triangles;
        var halfSize = size / 2;
        var x = halfSize.x;
        var y = halfSize.y;
        var z = halfSize.z;


        var frontTopLeft = new Vector3(-x, y, z);
        var frontTopRight = new Vector3(x, y, z);
        var frontBottomRight = new Vector3(x, -y, z);
        var frontBottomLeft = new Vector3(-x, -y, z);

        var backTopLeft = new Vector3(-x, y, -z);
        var backTopRight = new Vector3(x, y, -z);
        var backBottomRight = new Vector3(x, -y, -z);
        var backBottomLeft = new Vector3(-x, -y, -z);



        AddQuad(frontTopLeft, frontTopRight, frontBottomRight, frontBottomLeft, meshData);
        AddQuad(backTopLeft, backTopRight, backBottomRight, backBottomLeft, meshData, true);

        AddQuad(backTopLeft, frontTopLeft, frontBottomLeft, backBottomLeft, meshData);
        AddQuad(backTopRight, frontTopRight, frontBottomRight, backBottomRight, meshData, true);

        AddQuad(backTopLeft, backTopRight, frontTopRight, frontTopLeft, meshData);
        AddQuad(backBottomLeft, backBottomRight, frontBottomRight, frontBottomLeft, meshData, true);
    }

    public static void addCubeAtPoints(List<Vector3> points, Vector3 size, MeshData meshData)
    {
        var vertices = meshData.Vertices;
        var triangles = meshData.Triangles;
        foreach (var point in points)
        {
            addCubeAtPoint(point, size, meshData);
        }
    }

    public static void addCubeAtPoint(Vector3 point, Vector3 size, MeshData meshData)
    {
        var vertices = meshData.Vertices;
        var triangles = meshData.Triangles;
        int boxVerticesIndexStart = vertices.Count;
        addCube(size, meshData);
        Translate(vertices, point, boxVerticesIndexStart, vertices.Count - 1);
    }
    #endregion

    #region -- Utility --
    public static void AddQuad(int a, int b, int c, int d, MeshData meshData, bool flip = false)
    {
        if (flip)
            meshData.Triangles.AddRange(new int[] { a, b, c, a, c, d });
        else
            meshData.Triangles.AddRange(new int[] { c, b, a, d, c, a });
    }

    public static void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, MeshData meshData, bool flip = false)
    {
        int i = meshData.Vertices.Count;
        meshData.Vertices.AddRange(new Vector3[] { a, b, c, d });

        AddQuad(i + 0, i + 1, i + 2, i + 3, meshData, flip);
    }

    public static void AddTriangle(Vector3 a, Vector3 b, Vector3 c, List<Vector3> vertices, List<int> triangles, bool flip = false)
    {
        int i = vertices.Count;
        vertices.AddRange(new Vector3[] { a, b, c });

        if (flip)
            triangles.AddRange(new int[] { i + 0, i + 1, i + 2 });
        else
            triangles.AddRange(new int[] { i + 2, i + 1, i + 0 });
    }


    public static void AddTriangleFan(Vector3 centerPoint, Vector3[] points, List<Vector3> vertices, List<int> triangles, bool flip)
    {
        for (int i = 1; i < points.Length; i++)
        {
            AddTriangle(centerPoint, points[i], points[i - 1], vertices, triangles, flip);
        }
        AddTriangle(centerPoint, points[0], points[points.Length-1], vertices, triangles, flip);
    }

    public static void AddSmoothBridgeBetweenPointIndices(int[] pointIndicesA, int[] pointIndicesB, MeshData meshData)
    {

    }

    public static void AddBridgeBetweenPointSets(Vector3[] pointsA, Vector3[] pointsB, MeshData meshData, bool flip = false)
    {
        for (int i = 1; i < pointsA.Length; i++)
        {
            AddQuad(pointsA[i - 1],
                    pointsB[i - 1],
                    pointsB[i],
                    pointsA[i],
                    meshData, flip);
        }

        AddQuad(pointsA[pointsA.Length - 1],
                pointsB[pointsA.Length - 1],
                pointsB[0],
                pointsA[0],
                meshData, flip);
    }

    /// <summary>
    /// Returns a series of points that describe a circle located on the plane perpendicular to the specified axis. Axis parameter is assumed to be a unit euler angle
    /// </summary>
    public static List<Vector3> GetCircleOfPoints(float radius, int pointCount, Vector3 axis)
    {
        var points = new List<Vector3>();
        var v = new Vector3(axis.y, axis.z, axis.x) * radius; //Note, the mismatched axis components ensure (I think....) that the the vector being rotated and the axis of rotation are perpindecular
        var rotationStep = 360.0f / (float)pointCount;
        for (int i = 0; i < pointCount; i++)
        {
            var point = Quaternion.Euler(axis * (rotationStep * i)) * v;
            points.Add(point);
        }
        return points;
    }

    public static void Translate(this List<Vector3> points, Vector3 translation, int startIndex = -1, int endIndex = -1)
    {
        if (startIndex < 0) startIndex = 0;
        if (endIndex < 0 || endIndex >= points.Count) endIndex = points.Count - 1;
        for (int i = startIndex; i <= endIndex; i++)
        {
            points[i] = points[i] + translation;
        }
    }
    
    public static void Rotate(this List<Vector3> points, Quaternion rotation, int startIndex = -1, int endIndex = -1)
    {
        if (startIndex < 0) startIndex = 0;
        if (endIndex < 0 || endIndex >= points.Count) endIndex = points.Count - 1;
        for (int i = startIndex; i <= endIndex; i++)
        {
            points[i] = rotation * points[i];
        }
    }

    public static GameObject ToGameObject(this Mesh mesh, string name, Material material, Vector3 position, bool isVisible = true)
    {
        return gameObjectFromMesh(name, mesh, material, position, isVisible);
    }

    public static GameObject gameObjectFromMesh(string name, Mesh mesh, Material material, Vector3 position, bool isVisible = true)
    {
        var gameObject = new GameObject(name);
        var meshFilter = gameObject.AddComponent<MeshFilter>();
        var meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.enabled = isVisible;
        meshFilter.mesh = mesh;
        meshRenderer.material = material;
        gameObject.transform.position = position;
        return gameObject;
    }

    public static Mesh CreateMeshWrapper(Action<MeshData> createGeometryDelegate)
    {
        var meshData = new MeshData()
        {
            Vertices = new List<Vector3>(),
            Triangles = new List<int>()
        };
        
        createGeometryDelegate(meshData);

        var mesh = (Mesh)meshData;
        return mesh;
    }
    #endregion
}
