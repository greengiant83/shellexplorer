using UnityEngine;
using System.Collections;

public enum SelectionStateValue
{
    None,
    Hover,
    Active
}
public class BlockSelection
{
    public Transform OriginalParent;
    public Material OriginalMaterial;
    public Material HoverMaterial;
    public Material ActiveMaterial;
    public SpringJoint Spring;
    public Vector3 PrevPosition1;
    public Vector3 PrevPosition2;

    private Renderer renderer;

    private SelectionStateValue _selectionState;
    public SelectionStateValue SelectionState
    {
        get { return _selectionState; }
        set
        {
            _selectionState = value;
            if (Subject == null) return;
            switch (value) 
            { 
                case SelectionStateValue.None:
                    renderer.material = OriginalMaterial;
                    Subject = null;
                    break;
                case SelectionStateValue.Hover:
                    renderer.material = HoverMaterial;
                    break;
                case SelectionStateValue.Active:
                    renderer.material = ActiveMaterial;
                    break;
            }
        }
    }

    private GameObject _subject;
    public GameObject Subject
    {
        get { return _subject; }
        set
        {
            if(_subject != null && _subject != value)
            {
                renderer.material = OriginalMaterial;
            }

            _subject = value;
            if (value != null)
            {
                renderer = Subject.GetComponent<Renderer>();
                OriginalMaterial = renderer.material;
            }
            else
            {
                renderer = null;
                OriginalMaterial = null;
            }
        }
    }
}

[RequireComponent(typeof(Laser))]
public class LaserManipulator : MonoBehaviour 
{
    public Transform AnchorObject;
    public Material HoverMaterial;
    public Material SelectedMaterial;

    Hand hand;
    BlockSelection selection = new BlockSelection();
    Vector3 anchorRestLocalPosition;
    Laser laser;
    
	void Start () 
    {
        hand = GetComponent<Hand>();
        laser = transform.GetComponentInChildren<Laser>();
        selection.HoverMaterial = HoverMaterial;
        selection.ActiveMaterial = SelectedMaterial;
        anchorRestLocalPosition = AnchorObject.localPosition;
	}

    void FixedUpdate()
    {
        if (selection.SelectionState == SelectionStateValue.Active && hand.Controller.GetButton(SixenseButtons.TRIGGER))
        {
            selection.PrevPosition2 = selection.PrevPosition1;
            selection.PrevPosition1 = selection.Subject.transform.position;
        }
    }

	void Update () 
    {
        if(hand.Controller == null) return;

        if(selection.SelectionState == SelectionStateValue.Active)
        {
            //We currently have an active selection
            AnchorObject.localPosition += new Vector3(0, 0, hand.Controller.JoystickY * 0.01f);
            selection.Subject.transform.position = AnchorObject.position;

            if (hand.Controller.GetButtonUp(SixenseButtons.TRIGGER))
            {
                //We just released the trigger
                onBlockRelease();
                selection.SelectionState = SelectionStateValue.None;
            }
            else
            {
                selection.PrevPosition1 = selection.Subject.transform.position;
            }
        }

        if (selection.SelectionState != SelectionStateValue.Active)
        {
            //We do not have an active selection (may be Hover or None)
            var ray = new Ray(AnchorObject.TransformPoint(0, 0, .6f), AnchorObject.forward);
            
            if (laser.IsHit && laser.Hit.collider.gameObject.tag == "Block")
            {
                //We are currently pointing directly at a block
                if(laser.Hit.collider.gameObject != selection.Subject)
                {
                    //We just started pointing at a new block
                    selection.Subject = laser.Hit.collider.gameObject;
                    selection.SelectionState = SelectionStateValue.Hover;
                }

                if (selection.SelectionState == SelectionStateValue.Hover && hand.Controller.GetButtonDown(SixenseButtons.TRIGGER))
                {
                    //We just pulled the trigger to make the hover object active
                    selection.SelectionState = SelectionStateValue.Active;
                    onBlockActivation(laser.Hit);
                }
            }
            else
            {
                //We are not pointing at a block
                if(selection.SelectionState == SelectionStateValue.Hover)
                {
                    //We just stopped pointing at a block
                    selection.SelectionState = SelectionStateValue.None;
                }
            }
        }
	}

    void onBlockActivation(RaycastHit hitInfo)
    {
        AnchorObject.position = hitInfo.point;
        AnchorObject.position = selection.Subject.transform.position;
    }

    void onBlockRelease()
    {
        var delta = (selection.PrevPosition1 - selection.PrevPosition2);
        print(delta.magnitude);

        if(delta.magnitude < 0.01f) delta = Vector3.zero;

        selection.Subject.GetComponent<Rigidbody>().velocity = delta * 50f;

        AnchorObject.localPosition = anchorRestLocalPosition;

        
    }
}
