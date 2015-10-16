using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class NoteSetter : MonoBehaviour 
{
    public MusicThing MusicThing;

    public int Note;
    
	void Start () 
    {
        GetComponent<Collider>().isTrigger = true;
	}
	
	void Update () 
    {
	
	}

    void OnTriggerEnter(Collider collider)
    {
        MusicThing.SetNote(Note);
    }
}
