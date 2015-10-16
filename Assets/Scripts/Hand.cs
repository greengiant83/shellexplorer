using UnityEngine;
using System.Collections;

public class Hand : MonoBehaviour 
{
    public SixenseHands HandId;


    public SixenseInput.Controller Controller;
    Vector3 positionOffset = Vector3.zero;
    Quaternion rotOffset = Quaternion.identity;

    bool isInitialized = false;
    bool isPaused = false;
    Vector3 pauseStartPosition;
    Quaternion pauseStartRot;

	void Start () 
    {
        //rotOffset = transform.localRotation;
        //rotOffset = Quaternion.AngleAxis(45, Vector3.right);
	}

    bool ensureInitialization()
    {
        if (isInitialized) return true;
        Controller = SixenseInput.GetController(HandId);
        if (Controller != null)
        {
            isInitialized = true;
            //rotOffset = transform.rotation; // Quaternion.Inverse(Controller.Rotation);

        }
        
        return isInitialized;
    }
	
	void Update () 
    {
        if (!ensureInitialization()) return;

        if (Controller.GetButtonDown(SixenseButtons.START))
        {
            isPaused = true;
            pauseStartPosition = Controller.Position;
            pauseStartRot = Controller.Rotation;
        }
        else if(Controller.GetButtonUp(SixenseButtons.START))
        {
            isPaused = false;
            positionOffset += pauseStartPosition - Controller.Position;
            //rotOffset = rotOffset * (pauseStartRot * Quaternion.Inverse(Controller.Rotation));
        }

        if (!isPaused)
        {
            transform.localPosition = Controller.Position + positionOffset;
            transform.localRotation = rotOffset * Controller.Rotation;
        }
	}
}
