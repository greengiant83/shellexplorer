using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class StackPanel : MonoBehaviour 
{
	void Start () 
    {
        UpdateLayout();
	}

	void Update () 
    {
        //TODO: Need to figure out a way to decide when to update. Updating every frame even if nothing has changed is stupid
        UpdateLayout();
	}

    void UpdateLayout()
    {
        float y = 0;
        float spacing = 0;
        Vector3 position = Vector3.zero;

        for(int i=0;i<transform.childCount;i++)
        {
            var child = transform.GetChild(i).gameObject;
            var childBounds = child.GetRelativeBounds(this.transform, true);
            var meshToOriginOffset = childBounds.center - childBounds.extents;

            meshToOriginOffset.y *= -1;
            child.transform.localPosition = position; // -meshToOriginOffset;

            position -= Vector3.up * childBounds.size.y;
        }
    }
}
