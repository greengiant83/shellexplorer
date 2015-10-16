using UnityEngine;
using System.Collections;

public class StatusBar : MonoBehaviour 
{
    private static TextMesh label;

    public static void SetStatus(string Text)
    {
        Debug.Log("Status update: " + Text);

        if (label == null) return;
        label.text = Text;
    }

	void Start () 
    {
        label = GetComponent<TextMesh>();
	}
	
	void Update () 
    {
	
	}
}
