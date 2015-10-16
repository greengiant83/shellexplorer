using UnityEngine;
using System.Collections;
using System.Linq;

public class ToolbarManager : MonoBehaviour 
{
    public Material HoverMaterial;
    public Material InactiveMaterial;

    GameObject itemCollection;
    GameObject hoverItem;

    Hand leftHand;
    Hand rightHand;
    Hand[] hands;

	void Start () 
    {
        itemCollection = transform.FindChild("Items").gameObject;
        leftHand = GameObject.Find("Left Hand").GetComponent<Hand>();
        rightHand = GameObject.Find("Right Hand").GetComponent<Hand>();
        hands = new Hand[] { leftHand, rightHand };
	}
	
	void Update () 
    {
        updateGazeObject();

        if (hoverItem != null)
        {
            var leftController = SixenseInput.GetController(SixenseHands.LEFT);
            var rightController = SixenseInput.GetController(SixenseHands.RIGHT);

            //Check Right Hand button
            if (rightController != null && rightController.GetButtonDown(SixenseButtons.BUMPER))
            {
                var command = hoverItem.GetComponent<ToolbarCommand>();
                if (command != null)
                {
                    command.Activate(SixenseInput.GetController(SixenseHands.RIGHT));
                }
            }

            //Check left hand button
            if (leftController != null && leftController.GetButtonDown(SixenseButtons.BUMPER))
            {
                var command = hoverItem.GetComponent<ToolbarCommand>();
                if (command != null)
                {
                    command.Activate(SixenseInput.GetController(SixenseHands.LEFT));
                }
            }
        }
	}

    void updateGazeObject()
    {
        RaycastHit hitInfo;
        Transform camera = Camera.main.transform;

        GameObject newHoverItem = null;
        if (Physics.Raycast(new Ray(camera.position, camera.forward), out hitInfo))
        {
            newHoverItem = itemCollection.Children().FirstOrDefault(i => i == hitInfo.collider.gameObject);
        }

        //Activate new gaze object
        if (newHoverItem != null && newHoverItem != hoverItem)
        {
            //activate newhoveritem
            newHoverItem.GetComponent<Renderer>().material = HoverMaterial;
        }

        if (hoverItem != null && newHoverItem != hoverItem)
        {
            //deactive old hover item
            hoverItem.GetComponent<Renderer>().material = InactiveMaterial;
        }

        hoverItem = newHoverItem;
    }
}
