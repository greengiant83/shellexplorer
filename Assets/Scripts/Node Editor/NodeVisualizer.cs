using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class NodeVisualizer : MonoBehaviour 
{
    public Text DisplayNameText;
    public GameObject PortTemplate;
    public Transform Background;

    int portIndex;
    float portHeight = 0.08f;
    public void SetNodeDefinition(INodeDefinition NodeDefinition)
    {
        DisplayNameText.text = NodeDefinition.DisplayName;

        portIndex = 0;
        addPortVisualizers(NodeDefinition.InputPorts, true);
        addPortVisualizers(NodeDefinition.OutputPorts, false);

        float top = Background.localPosition.y - Background.localScale.y / 2;
        float height = (Background.localScale.y - portHeight) + portHeight * portIndex;
        Background.localScale = new Vector3(Background.localScale.x, height, Background.localScale.z);
        //Background.localPosition = new 
    }

    private void addPortVisualizers(IEnumerable<PortDefinition> ports, bool isInput)
    {
        foreach (var port in ports)
        {
            var newPort = Instantiate<GameObject>(PortTemplate);
            newPort.name = port.DisplayName;
            newPort.transform.SetParent(PortTemplate.transform.parent);
            newPort.transform.localPosition = PortTemplate.transform.localPosition + Vector3.down * portIndex * portHeight;
            newPort.GetComponent<PortVisualizer>().SetPortDefinition(port, isInput);
            newPort.SetActive(true);
            portIndex++;
        }
    }

	void Start () 
    {
        PortTemplate.SetActive(false);

        var nodes = NodeDefinitionProvider.Instance.GetDefinitions();
        SetNodeDefinition(nodes.First());
	}
	
	void Update () 
    {
	
	}
}
