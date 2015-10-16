using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class UIManager : MonoBehaviour 
{
    public static UIManager Instance;

    public HandInput RightHand;
    public HandInput LeftHand;

    HandInput[] hands;
    SixenseButtons addToSelectionModifier = SixenseButtons.THREE;

	void Start () 
    {
        Instance = this;
        hands = new HandInput[] { RightHand, LeftHand };
	}
	
	void Update () 
    {
	    foreach(var hand in hands)
        {
            bool triggerHandled = false;

            //Check for clickables
            if (hand.Laser.IsHit)
            {
                var clickable = hand.Laser.Hit.collider.gameObject.GetComponent<IClickable>();
                if (clickable != null && hand.GetButtonDown(SixenseButtons.TRIGGER))
                {
                    clickable.OnClick(hand);
                    triggerHandled = true;
                }
            }

            //Update selection set
            ISelectable hoverItem = null;
            if(hand.Laser.IsHit)
            {
                var hitGameObject = hand.Laser.Hit.collider.gameObject;
                hoverItem = hitGameObject.GetComponent<ISelectable>();
            }
            hand.SelectionSet.SetHoverItem(hoverItem);
            
            
            if(hand.GetButtonDown(SixenseButtons.TRIGGER))
            {
                if (hand.GetButton(addToSelectionModifier))
                {
                    if (hoverItem != null) hand.SelectionSet.ToggleSelection(hoverItem);
                }
                else if (hoverItem != null)
                {
                    hand.SelectionSet.ActivateSingle(hoverItem);
                }
                else if(!triggerHandled)
                {
                    hand.SelectionSet.ClearActive();
                }

                
            }

            
        }
	}

    public IEnumerable<SelectionSet> GetSelectionSets()
    {
        return hands.Select(i => i.SelectionSet);
    }

    public IEnumerable<GameObject> GetSelectedObjects()
    {
        return GetSelectionSets().SelectMany(i => i.GetActive());
    }

    public void NotifyObjectDestruction(GameObject item)
    {
        var selectable = item.GetComponent<ISelectable>();
        if(selectable != null)
        {
            foreach(var hand in hands)
            {
                hand.SelectionSet.RemoveActive(selectable);
            }
        }
    }
}
