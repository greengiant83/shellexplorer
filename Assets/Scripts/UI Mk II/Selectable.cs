using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ISelectable
{
    void AddHighlightColor(Color color);
    void RemoveHighlightColor(Color color);
    
    GameObject gameObject { get; }
}

public class Selectable : MonoBehaviour, ISelectable
{
    Renderer renderer;
    Material originalMaterial;
    List<Color> highlightColors = new List<Color>();


	void Start () 
    {
        renderer = GetComponent<Renderer>();
        originalMaterial = renderer.sharedMaterial;
	}
	
	void Update () 
    {	
	}

    void OnDestroy()
    {
        UIManager.Instance.NotifyObjectDestruction(this.gameObject);
    }

    void updateMaterialHighlight()
    {
        if(highlightColors.Count == 0)
        {
            renderer.material = originalMaterial;
        }
        else
        {
            var newColor = originalMaterial.color;
            foreach(var color in highlightColors)
            {
                newColor = Color.Lerp(newColor, color, 0.5f);
            }
            renderer.material.color = newColor;
        }
    }

    public void AddHighlightColor(Color color)
    {
        if(!highlightColors.Contains(color))
        {
            highlightColors.Add(color);
            updateMaterialHighlight();
        }
    }

    public void RemoveHighlightColor(Color color)
    {
        if (highlightColors.Contains(color))
        {
            highlightColors.Remove(color);
            updateMaterialHighlight();
        }
    }
}
