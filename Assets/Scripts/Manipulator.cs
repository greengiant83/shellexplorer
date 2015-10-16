using UnityEngine;
using System.Collections;

public class Manipulator : MonoBehaviour 
{
    public Transform ArchiveContainer;
    
    Hand hand;
    
	void Start () 
    {
        hand = GetComponent<Hand>();
	}
	
	void Update () 
    {
        if (hand.Controller == null) return;

        if (hand.Controller.GetButtonDown(SixenseButtons.BUMPER))
        {
            while (transform.childCount > 1)
            {
                transform.GetChild(1).SetParent(ArchiveContainer, true);
            }
        }

        ArchiveContainer.transform.Rotate(0, hand.Controller.JoystickX, 0);

        if (hand.Controller.GetButtonDown(SixenseButtons.TWO))
        {
            //Delete all strokes
            var strokes = GameObject.FindGameObjectsWithTag("Brush Stroke");
            foreach (var stroke in strokes)
            {
                Destroy(stroke);
            }
        }
	}
}
