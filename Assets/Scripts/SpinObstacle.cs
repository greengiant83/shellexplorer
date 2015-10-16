using UnityEngine;
using System.Collections;

public class SpinObstacle : MonoBehaviour
{
    public float angleOffset = 0;

    float rotateSpeed = 5f;
    float maxRadius = 0.5f;
    float minRadius = 0.25f;
    float oscillateSpeed = 0.1f;

    float radius = 0.5f;
    float angle;
    float t = 0;

	void FixedUpdate () 
    {
        t += oscillateSpeed;
        var v = Mathf.Sin(t); //gives you a value between -1 and 1
        v = (v + 1) / 2; //v is now between 0 and 1
        v = minRadius + v * (maxRadius - minRadius); //
        radius = v;
        print(v);
        

        angle += rotateSpeed;
        var rotation = Quaternion.AngleAxis(angle + angleOffset, Vector3.forward);
        transform.position = rotation * (Vector3.right * radius);
	}
}
