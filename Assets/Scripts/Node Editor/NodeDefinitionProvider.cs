using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeDefinitionProvider
{
    #region -- Singleton --
    public static NodeDefinitionProvider Instance;

    static NodeDefinitionProvider()
    {
        Instance = new NodeDefinitionProvider();
    }    
    #endregion

    private INodeDefinition[] definitions;

    private NodeDefinitionProvider()
    {
        definitions = new[]
        {
            new AddNodeDefinition()
        };
    }

    public IEnumerable<INodeDefinition> GetDefinitions()
    {
        foreach (var item in definitions) yield return item;
    }
}
