using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public abstract class MeshModifier
{
    public abstract MeshData Execute(MeshData input);
}
