using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SelectionSet : MonoBehaviour 
{
    public Color ActiveColor;
    public Color HoverColor;

    public ObservableCollection<ISelectable> ActiveItems = new ObservableCollection<ISelectable>();
    public ISelectable HoverItem;

    public SelectionSet()
    {
        ActiveItems.CollectionChanged += ActiveItems_CollectionChanged;
    }

    void ActiveItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if(e.OldItems != null)
        {
            foreach(ISelectable item in e.OldItems)
            {
                item.RemoveHighlightColor(ActiveColor);
            }
        }

        if (e.NewItems != null)
        {
            foreach (ISelectable item in e.NewItems)
            {
                item.AddHighlightColor(ActiveColor);
            }
        }
    }
	
    public IEnumerable<GameObject> GetActive()
    {
        return ActiveItems.Select(i => i.gameObject);
    }

    public void AddActive(ISelectable item)
    {
        if (!ActiveItems.Contains(item)) ActiveItems.Add(item);
    }

    public void RemoveActive(ISelectable item)
    {
        if (ActiveItems.Contains(item)) ActiveItems.Remove(item);
    }

    public void ToggleSelection(ISelectable item)
    {
        if (ActiveItems.Contains(item))
            RemoveActive(item);
        else
            AddActive(item);
    }

    public void ActivateSingle(ISelectable item)
    {
        ClearActive();
        AddActive(item);
    }

    public void ClearActive()
    {

        ActiveItems.Clear();
    }

    public void SetHoverItem(ISelectable item)
    {
        var prevHoverItem = HoverItem;
        HoverItem = item;
        if (HoverItem != null) HoverItem.AddHighlightColor(HoverColor);
        if (prevHoverItem != null && item != prevHoverItem) prevHoverItem.RemoveHighlightColor(HoverColor);
    }
}
