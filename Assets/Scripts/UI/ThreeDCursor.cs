using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ThreeDCursor : MonoBehaviour 
{
    Laser laser;
    Hand hand;
    public GameObject Crosshairs;

    private bool isSamplingFromLaser = true;
    public bool IsSamplingFromLaser
    {
        get { return isSamplingFromLaser; }
        set
        {
            isSamplingFromLaser = value;
            laser.ActiveSampling = value;
        }
    }

    void Awake() 
    {
        Crosshairs = Instantiate(Resources.Load("Crosshair") as GameObject);
        Crosshairs.transform.SetParent(this.transform);
        Crosshairs.SetActive(this.enabled);

        laser = this.gameObject.AddComponent<Laser>();
        laser.enabled = this.enabled;
        hand = this.transform.parent.gameObject.GetComponent<Hand>();



        //TODO: I think it would be a good idea to be able to toggle between laser based 3d positioning and hand with thumb stick based positioning
	}

    void OnEnable()
    {
        laser.enabled = true;
    }

    void OnDisable()
    {
        laser.enabled = false;
    }

    public void SetCrosshairDistance(float distance)
    {
        if (isSamplingFromLaser) return;

        Crosshairs.transform.localPosition = Vector3.forward * distance;
    }

    public float GetCrosshairDistance()
    {
        return Crosshairs.transform.localPosition.magnitude;
    }
	
	void Update () 
    {
        if (hand.Controller == null) return;

        if (IsSamplingFromLaser)
        {
            if (laser.IsHit)
            {
                Crosshairs.transform.position = laser.Hit.point;
                Crosshairs.transform.rotation = Quaternion.FromToRotation(Vector3.forward, laser.Hit.normal);
                Crosshairs.SetActive(true);
            }
            else
            {
                Crosshairs.SetActive(false);
            }

            pumpMessages();
        }
        else
        {
            Crosshairs.transform.localRotation = Quaternion.identity;
            Crosshairs.transform.localPosition += Vector3.forward * hand.Controller.JoystickY * 0.05f;
            laser.SetLaserLength(Crosshairs.transform.localPosition.z);
        }
	}

    void pumpMessages()
    {
        if (hand.Controller.GetButtonDown(SixenseButtons.TRIGGER))
        {
            pumpSelectables();
            pumpGUIClicks();           
        }
    }

    void pumpSelectables()
    {
        if (laser.IsHit)
        {
            var selectable = laser.Hit.collider.gameObject.GetComponent<ISelectable>();
            if (selectable != null)
            {
                //SelectionManager.Instance.ToggleSelection(selectable);
            }
        }
        else
        {
            //SelectionManager.Instance.ClearSelection();
        }
    }

    void pumpGUIClicks()
    {
        if (!laser.IsHit) return;

        var inputField = laser.Hit.collider.gameObject.GetComponent<InputField>();
        if (inputField != null)
        {
            //inputField.ActivateInputField();
            inputField.Select();
        }

        var button = laser.Hit.collider.gameObject.GetComponent<Button>();
        if (button != null)
        {
            button.Select();
            ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
    }
}
