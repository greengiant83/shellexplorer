using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Brush : MonoBehaviour 
{
    public Transform ActiveStrokeParent;
    public Transform InactiveStrokeParent;

    float brushScalar = 1;
    float triggerThreshold = 0.1f;
    float segmentThreshold = 0.01f;

    Vector3 corner1 = new Vector3(-0.5f, 0.5f, 0);
    Vector3 corner2 = new Vector3(0.5f, 0.5f, 0);
    Vector3 corner3 = new Vector3(0.5f, -0.5f, 0);
    Vector3 corner4 = new Vector3(-0.5f, -0.5f, 0);

    Hand hand;
    Renderer renderer;

    GameObject strokeObject;
    Mesh strokeMesh;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    bool isStroking;
    bool wasStroking;
    Vector3 currentLocalPosition;
    Vector3 currentWorldPosition;
    Vector3 lastLocalPosition;
    Vector3 lastWorldPosition;
    
	void Start () 
    {
        hand = transform.parent.GetComponent<Hand>();
        renderer = GetComponent<Renderer>();
	}
	
	void Update () 
    {
        if (hand.Controller == null) return;

        monitorStroke();

        if (hand.Controller.GetButtonDown(SixenseButtons.FOUR) && !isStroking) 
        {
            var strokes = GameObject.FindGameObjectsWithTag("Brush Stroke");
            if(strokes.Count() > 0) Destroy(strokes.Last());
        }


        
	}

    void monitorStroke()
    {
        if (hand.Controller.GetButtonDown(SixenseButtons.THREE))
        {
            brushScalar += 1f;
        }

        if (hand.Controller.GetButtonDown(SixenseButtons.ONE))
        {
            if(brushScalar > 0) brushScalar -= 1f;
        }
        if (brushScalar < 1) brushScalar = 1;

        

        float brushSize = hand.Controller.Trigger;
        isStroking = brushSize >= triggerThreshold;
        brushSize *= brushScalar;
        renderer.enabled = isStroking;

        if (isStroking)
        {
            transform.localScale = new Vector3(brushSize, brushSize, 0.1f);

            if (!wasStroking) beginStroke();

            currentWorldPosition = transform.position;
            currentLocalPosition = strokeObject.transform.InverseTransformPoint(currentWorldPosition);

            var strokeDirection = (currentWorldPosition - lastWorldPosition).normalized * 1;
            transform.rotation = Quaternion.LookRotation(strokeDirection);
            Debug.DrawRay(transform.position, strokeDirection, Color.red);

            var segmentDistance = (currentLocalPosition - lastLocalPosition).magnitude;
            if (segmentDistance >= segmentThreshold)
                addStrokeSegment();
        }

        if (wasStroking && !isStroking) endStroke();

        wasStroking = isStroking;
    }

    void beginStroke()
    {
        strokeMesh = new Mesh();
        strokeObject = new GameObject("Brush Stroke");
        strokeObject.tag = "Brush Stroke";
        strokeObject.transform.SetParent(ActiveStrokeParent);
        var meshFilter = strokeObject.AddComponent<MeshFilter>();
        var meshRenderer = strokeObject.AddComponent<MeshRenderer>();
        meshRenderer.enabled = true;
        meshFilter.mesh = strokeMesh;
        meshRenderer.material = renderer.material;
        strokeObject.transform.position = Vector3.zero; //TODO: this should probably be updated to something rational
        vertices.Clear();
        triangles.Clear();

        addCornerPoints();
        addQuadBetweenPoints(3, 2, 1, 0);
    }

    void addStrokeSegment()
    {
        var strokeDirection = currentLocalPosition - lastLocalPosition;
        var isFlipped = Vector3.Dot(strokeDirection, transform.forward) >= 0;

        addCornerPoints();

        int i = vertices.Count;
        addQuadBetweenPoints(
            i - 1, 
            i - 2, 
            i - 6, 
            i - 5,
            isFlipped);

        addQuadBetweenPoints(
            i - 2,
            i - 3,
            i - 7,
            i - 6,
            isFlipped);

        addQuadBetweenPoints(
            i - 3,
            i - 4,
            i - 8,
            i - 7,
            isFlipped);

        addQuadBetweenPoints(
            i - 4,
            i - 1,
            i - 5,
            i - 8,
            isFlipped);

        updateMesh();
    }

    void endStroke()
    {
        int i = vertices.Count;
        addQuadBetweenPoints(
            i - 4,
            i - 3,
            i - 2,
            i - 1);

        updateMesh();
    }

    void addCornerPoints()
    {
        vertices.Add(strokeObject.transform.InverseTransformPoint(transform.TransformPoint(corner1)));
        vertices.Add(strokeObject.transform.InverseTransformPoint(transform.TransformPoint(corner2)));
        vertices.Add(strokeObject.transform.InverseTransformPoint(transform.TransformPoint(corner3)));
        vertices.Add(strokeObject.transform.InverseTransformPoint(transform.TransformPoint(corner4)));
        lastWorldPosition = currentWorldPosition;
        lastLocalPosition = currentLocalPosition;
        //lastLocalPosition = strokeObject.transform.InverseTransformPoint(transform.position);
    }

    void addQuadBetweenPoints(int index1, int index2, int index3, int index4, bool flip = false)
    {
        if (flip)
        {
            triangles.AddRange(new int[]
            {
                index3, index2, index1, 
                index4, index3, index1
            });
        }
        else
        {
            triangles.AddRange(new int[]
            {
                index1, index2, index3, 
                index1, index3, index4
            });
        }
    }

    void updateMesh()
    {
        strokeMesh.vertices = vertices.ToArray();
        strokeMesh.triangles = triangles.ToArray();
        strokeMesh.uv = new Vector2[vertices.Count];
        strokeMesh.RecalculateNormals();
        strokeMesh.RecalculateBounds();
    }
}
