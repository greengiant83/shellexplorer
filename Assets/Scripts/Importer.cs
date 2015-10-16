using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Security.Policy;

public class Importer : MonoBehaviour 
{
    public GameObject Cube1;
    public GameObject Cube2;

    List<Component> addedComponents = new List<Component>();

	void Start () 
    {        
	}

    void destroyAddedComponents()
    {
        foreach (var component in addedComponents)
        {
            Destroy(component);
        }
        addedComponents.Clear();
    }

    void makeItBounce(GameObject cube, string path)
    {
        AppDomain childDomain = BuildChildDomain(AppDomain.CurrentDomain);

        
        var ass = Assembly.LoadFile(path);
        var bounceType = ass.GetType("Bounce");
        var component = cube.AddComponent(bounceType);
        addedComponents.Add(component);

    }

    private static AppDomain BuildChildDomain(AppDomain parentDomain)
    {
        var evidence = new Evidence(parentDomain.Evidence);
        AppDomainSetup setup = parentDomain.SetupInformation;
        return AppDomain.CreateDomain("DiscoveryRegion",
            evidence, setup);
    }
	
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            destroyAddedComponents();
            makeItBounce(Cube1, @"D:\Drive\Unity\PID\Export\bin\Debug\PID-Export-CSharp.dll");
            makeItBounce(Cube2, @"D:\Drive\Unity\GrappleCar\Export\bin\Debug\GrappleCar-Export-CSharp.dll");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            destroyAddedComponents();
        }
	}
}
