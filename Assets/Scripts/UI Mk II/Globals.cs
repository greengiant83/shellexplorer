using UnityEngine;
using System.Collections;

public class Globals : MonoBehaviour 
{
    public static Globals Instance;

    public Material LaserMaterial;

	void Awake () 
    {
        Globals.Instance = this;
	}
}
