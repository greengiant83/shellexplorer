using UnityEngine;
using System.Collections;

public abstract class ToolbarCommand : MonoBehaviour
{
    public abstract void Activate(object Sender);
}