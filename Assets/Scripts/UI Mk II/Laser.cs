using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour 
{
    public Material Material;
    public RaycastHit Hit;
    public bool IsHit;
    public bool ActiveSampling = true;

    private bool isInitialized = false;

    GameObject visual;

	void Start () 
    {
        if (!isInitialized && enabled) initialize();
	}

    void initialize()
    {
        if (Material == null) Material = Globals.Instance.LaserMaterial;
        visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.GetComponent<Renderer>().material = Material;
        visual.transform.SetParent(this.transform, false);
        visual.transform.localScale = new Vector3(0.001f, 0.001f, 5);
        visual.transform.localPosition = new Vector3(0, 0, 2.5f);
        Destroy(visual.GetComponent<Collider>());
        isInitialized = true;
    }


    void OnEnable()
    {
        if (!isInitialized) initialize();
        visual.SetActive(true);
    }

    void OnDisable()
    {
        if(visual != null) visual.SetActive(false);
    }
	
	void Update () 
    {
        if (ActiveSampling)
        {
            var ray = new Ray(transform.position, transform.forward);
            IsHit = Physics.Raycast(ray, out Hit);

            if (IsHit)
                SetLaserLength((Hit.point - transform.position).magnitude);
            else
                SetLaserLength(100);
        }
	}

    public void SetLaserLength(float length)
    {
        visual.transform.localScale = new Vector3(visual.transform.localScale.x, visual.transform.localScale.y, length);
        visual.transform.localPosition = new Vector3(0, 0, length / 2);
    }
}
