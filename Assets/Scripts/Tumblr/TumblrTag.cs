using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TumblrTag : MonoBehaviour 
{
    public Text NameLabel;

    public string TagName
    {
        get { return NameLabel.text; }
        set { NameLabel.text = value; }
    }

	void Start () 
    {
	
	}
	
	void Update () 
    {
	
	}
}
