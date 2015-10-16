using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PortVisualizer : MonoBehaviour 
{
    public Text DisplayNameText;
    public Transform Connector;

    public void SetPortDefinition(PortDefinition PortDefinition, bool IsInput)
    {
        DisplayNameText.text = PortDefinition.DisplayName;
        DisplayNameText.alignment = IsInput ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
        Connector.localPosition = IsInput ? new Vector3(0.5f, 0, 0) : new Vector3(-0.5f, 0, 0);
    }

	void Start () 
    {
	
	}
	
	void Update () 
    {
	
	}
}
