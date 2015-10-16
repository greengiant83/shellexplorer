using UnityEngine;
using System.Collections;

public class FretBoard : MonoBehaviour 
{
    public int[] Notes;
    public MusicThing MusicThing;

	void Start () 
    {
        GetComponent<MeshRenderer>().enabled = false;
        createFretBoard();
	}

    void createFretBoard()
    {
        transform.DestroyChildren();

        float spacing = 1f / (float)Notes.Length;
        float height = spacing * 0.9f;
        Vector3 offset = new Vector3(0, -0.5f + spacing / 2, 0);

        for (int i = 0; i < Notes.Length; i++)
        {
            var noteCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            noteCube.name = "Note " + i;
            noteCube.transform.SetParent(this.transform);
            noteCube.transform.localScale = new Vector3(1.1f, height, 1.1f);
            noteCube.transform.localPosition = Vector3.up * spacing * i + offset;
            noteCube.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
            noteCube.layer = this.gameObject.layer;

            var noteSetter = noteCube.AddComponent<NoteSetter>();
            noteSetter.MusicThing = MusicThing;
            noteSetter.Note = Notes[i];
        }
    }

    void Update() 
    {
	
	}
}
