using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class Wave
{
    public float t;
    public float speed;
    public float amplitude;
    public float value;

    public void Update()
    {
        t += speed;
        value = Mathf.Sin(t) * amplitude;
    }
}

public class MovementAlgorithm : MonoBehaviour 
{
    public Transform Slide;
    public float RiseSpeed = 0.01f;
    public float RotateSpeed = 1f;

    float slideRadius;
    float t = 0;
    List<Wave> waves;
    
	void Start () 
    {
        slideRadius = Slide.transform.localPosition.magnitude;
        waves = new List<Wave>();
        
        waves.Add(new Wave()
        {
            amplitude = slideRadius * .75f,
            speed = 0.03f
        });

        waves.Add(new Wave()
        {
            amplitude = slideRadius * .2f,
            speed = 0.1f
        });

        waves.Add(new Wave()
        {
            amplitude = slideRadius * .05f,
            speed = 0.17f
        });
        
	}
	
	void FixedUpdate () 
    {
        transform.Rotate(Vector3.up, RotateSpeed);
        transform.Translate(Vector3.up * RiseSpeed);

        float v = 0;
        foreach(var wave in waves) 
        {
            wave.Update();
            v += wave.value + 0.01f;
        }
        Slide.transform.localPosition = new Vector3(v, 0, 0.05f);
	}
}
