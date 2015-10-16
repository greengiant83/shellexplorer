using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeDefinition
{
    public string DisplayName;
    public List<PortDefinition> InputPorts;
    public List<PortDefinition> OutputPorts;
}

public class PortDefinition
{
    public string Key;
    public string DisplayName;
    public string DataType;
}

public interface INodeDefinition
{
    string DisplayName { get; }
    IEnumerable<PortDefinition> InputPorts { get; }
    IEnumerable<PortDefinition> OutputPorts { get; }
    INodeInstance GetInstance();
}

public interface INodeInstance
{
    Dictionary<string, object> Outputs { get; }
    void UpdateOutputs(Dictionary<string, object> Inputs);
}

public class NodeLink
{
    public INodeInstance SourceNode;
    public INodeInstance DestinationNode;

    public string SourcePortKey;
    public string DestinationPortKey;
}


// ------------------------------------------------------

public abstract class NodeDefinition<T> : INodeDefinition where T: INodeInstance, new()
{
    public abstract string DisplayName { get; }
    
    public abstract IEnumerable<PortDefinition> InputPorts { get; }
    
    public abstract IEnumerable<PortDefinition> OutputPorts { get; }
    
    public INodeInstance GetInstance()
    {
        return new T();
    }
}


public class AddNodeDefinition : NodeDefinition<AddNodeInstance>
{
    public override string DisplayName { get { return "Add"; } }

    private PortDefinition[] inputs = new PortDefinition[]
    {
        new PortDefinition() { Key = "X", DisplayName = "X", DataType = "Number" },
        new PortDefinition() { Key = "Y", DisplayName = "Y", DataType = "Number" },
    };
    public override IEnumerable<PortDefinition> InputPorts { get { return inputs; } }

    private PortDefinition[] outputs = new PortDefinition[]
    {
        new PortDefinition() { Key = "Sum", DisplayName = "Sum", DataType = "Number" },
    };
    public override IEnumerable<PortDefinition> OutputPorts { get { return outputs; } }
}

public class AddNodeInstance : INodeInstance
{
    private Dictionary<string, object> outputs = new Dictionary<string, object>();
    public Dictionary<string, object> Outputs { get { return outputs; } }

    public void UpdateOutputs(Dictionary<string, object> Inputs)
    {
        int x = (int)Inputs["X"];
        int y = (int)Inputs["Y"];
        int value = x + y;

        if (Outputs.ContainsKey("Sum"))
            Outputs["Sum"] = value;
        else
            Outputs.Add("Sum", value);
    }
}