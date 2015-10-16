using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LayoutEngine : MonoBehaviour 
{
    public Transform[] Items;

    ILayoutProvider layoutProvider = new GridLayoutProvider();
    ObservableCollection<Transform> items = new ObservableCollection<Transform>();
    bool layoutToggle = false;

	void Start () 
    {
        items.AddRange(Items);
        layoutProvider.Items = items;
	}

	void Update () 
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            var newItem = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newItem.transform.position = UnityEngine.Random.insideUnitSphere * 20;
            items.Add(newItem.transform);
        }

	    if(Input.GetKeyDown(KeyCode.Space))
        {
            layoutToggle = !layoutToggle;

            print("Toggling layout provider: " + layoutToggle);
            layoutProvider.Dispose();
            if(layoutToggle)
            {
                layoutProvider = new CircleLayoutProvider()
                {
                    Items = items
                };
            }
            else
            {
                layoutProvider = new GridLayoutProvider()
                {
                    Items = items
                };
            }
        }
	}
}

public interface ILayoutProvider : IDisposable
{
    ObservableCollection<Transform> Items { get; set; }
}

public abstract class LayoutProviderBase : ILayoutProvider
{
    protected ObservableCollection<Transform> items = new ObservableCollection<Transform>();

    public ObservableCollection<Transform> Items
    {
        get { return items; }
        set
        {
            items = value;
            monitorCollection();    
        }
    }

    public LayoutProviderBase()
    {
        monitorCollection();
    }

    void monitorCollection()
    {
        items.CollectionChanged += items_CollectionChanged;
        updateItems();
    }

    void items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        updateItems();
    }

    protected abstract void updateItems();

    public void Dispose()
    {
        items.CollectionChanged -= items_CollectionChanged;
    }
}


public class GridLayoutProvider : LayoutProviderBase
{
    protected override void updateItems()
    {
        Debug.Log("Grid updating layout");
        Vector3 position = Vector3.zero;

        foreach (var item in items)
        {
            LayoutEnforcer enforcer = item.GetComponent<LayoutEnforcer>();
            if (enforcer == null) enforcer = item.gameObject.AddComponent<LayoutEnforcer>();

            enforcer.TargetPosition = position;

            position += new Vector3(1.25f, 0, 0);
        }
    }
}

public class CircleLayoutProvider : LayoutProviderBase
{
    protected override void updateItems()
    {
        Debug.Log("Circle updating layout");
        Vector3 position = Vector3.zero;

        foreach (var item in items)
        {
            LayoutEnforcer enforcer = item.GetComponent<LayoutEnforcer>();
            if (enforcer == null) enforcer = item.gameObject.AddComponent<LayoutEnforcer>();

            enforcer.TargetPosition = position;

            position += new Vector3(0, 1.25f, 0);
        }
    }
}

