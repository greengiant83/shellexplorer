using UnityEngine;
using System.Collections;

public class HandInput : MonoBehaviour 
{
    public SixenseHands HandId;
    public SixenseInput.Controller Controller;
    public SelectionSet SelectionSet;

    public Laser Laser;
    public Transform Crosshairs;

    bool isInitialized = false;
    
    void Start()
    {
        SelectionSet = GetComponent<SelectionSet>();
    }

    bool ensureInitialization()
    {
        if (isInitialized) return true;

        Controller = SixenseInput.GetController(HandId);
        if (Controller != null)
        {
            isInitialized = true;
        }

        return isInitialized;
    }

    void Update()
    {
        if (!ensureInitialization()) return;

        transform.localPosition = Controller.Position;
        transform.localRotation = Controller.Rotation;

        if (Laser.IsHit)
        {
            Crosshairs.position = Laser.Hit.point;
            Crosshairs.rotation = Quaternion.FromToRotation(Vector3.forward, Laser.Hit.normal);
            Crosshairs.gameObject.SetActive(true);
        }
        else
        {
            Crosshairs.gameObject.SetActive(false);
        }
    }

    public bool GetButton(SixenseButtons button)
    {
        if (Controller != null)
            return Controller.GetButton(button);
        else
            return false;
    }

    public bool GetButtonDown(SixenseButtons button)
    {
        if (Controller != null)
            return Controller.GetButtonDown(button);
        else
            return false;
    }

    public bool GetButtonUp(SixenseButtons button)
    {
        if (Controller != null)
            return Controller.GetButtonUp(button);
        else
            return false;
    }

    public float JoystickX
    {
        get 
        {
            return Controller != null ? Controller.JoystickX : 0;
        }
    }

    public float JoystickY
    {
        get
        {
            return Controller != null ? Controller.JoystickY : 0;
        }
    }

    public float Trigger
    {
        get
        {
            return Controller != null ? Controller.Trigger : 0;
        }
    }
}
