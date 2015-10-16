using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NotifyCollectionChangedEventArgs
{
    public IList NewItems;
    public IList OldItems;
}

public delegate void NotifyCollectionChangedEventHandler(System.Object sender, NotifyCollectionChangedEventArgs e);

public class ObservableCollection<T> : IEnumerable<T>
{
    public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

    private List<T> list = new List<T>();

    public int Count
    {
        get { return list.Count; }
    }

    public void Add(T item)
    {
        list.Add(item);

        if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs()
        {
            NewItems = new List<T>() { item }
        });
    }


    public void AddRange(IEnumerable<T> collection)
    {
        list.AddRange(collection);

        if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs()
        {
            NewItems = collection.ToList()
        });
    }

    public void Remove(T item)
    {
        list.Remove(item);

        if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs()
        {
            OldItems = new List<T>() { item }
        });
    }

    public void Clear()
    {
        var oldItems = list.Select(i => i).ToList();
        
        list.Clear();

        if (CollectionChanged != null) CollectionChanged(this, new NotifyCollectionChangedEventArgs()
        {
            OldItems = oldItems
        });
    }


    public IEnumerator<T> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return list.GetEnumerator();
    }
}
