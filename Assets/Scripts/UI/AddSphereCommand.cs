using UnityEngine;
using System.Collections;

public class AddSphereProcess : ProcessManager
{
    public GameObject NewSphere;
    public ThreeDCursor rightCursor;
    public ThreeDCursor leftCursor;

    protected override IProcessStep[] GetSteps()
    {
        return new IProcessStep[] 
        {
            new DefinePositionAndRadiusStep(),
            new Finalize()
        };
    }

    public AddSphereProcess()
    {
        this.rightCursor = GameObject.Find("Right Hand").transform.Find("Tool Attachment").GetComponent<ThreeDCursor>();
        this.leftCursor = GameObject.Find("Left Hand").transform.Find("Tool Attachment").GetComponent<ThreeDCursor>();
    }
    
    #region -- Process Steps --
    class DefinePositionAndRadiusStep : ProcessStep<AddSphereProcess>
    {
        string instructions { get { return "Press bumper button when complete"; } }

        public override void Activate()
        {
            StatusBar.SetStatus(instructions);
            processManager.rightCursor.IsSamplingFromLaser = false;
            processManager.leftCursor.IsSamplingFromLaser = false;

            processManager.rightCursor.SetCrosshairDistance(0.25f);
            processManager.leftCursor.SetCrosshairDistance(0.25f);

            processManager.NewSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }

        public override void Update()
        {
            processManager.leftCursor.SetCrosshairDistance(processManager.rightCursor.GetCrosshairDistance()); //Keep the distances aligned to prevent things getting too crazy

            var leftPoint = processManager.leftCursor.Crosshairs.transform.position;
            var rightPoint = processManager.rightCursor.Crosshairs.transform.position;

            processManager.NewSphere.transform.position = (leftPoint + rightPoint) / 2;
            processManager.NewSphere.transform.localScale = Vector3.one * (rightPoint - leftPoint).magnitude;

            if (SixenseInput.GetController(SixenseHands.RIGHT).GetButtonDown(SixenseButtons.BUMPER))
            {
                processManager.GoToNextStep();
            }
        }

        public override void Revert()
        {
            
        }
    }

    class Finalize : ProcessStep<AddSphereProcess>
    {
        public override void Activate()
        {
            StatusBar.SetStatus("");
            processManager.GoToNextStep();
        }

        public override void Update()
        {
        }

        public override void Revert()
        {
        }
    }
    #endregion
}

public class AddSphereCommand : ToolbarCommand
{
    ProcessManager processManager;

    void Start()
    {
    }

    void Update()
    {
        if (processManager != null)
        {
            processManager.Update();
            if (processManager.IsComplete) processManager = null;
        }
    }

    public override void Activate(object Sender)
    {
        processManager = new AddSphereProcess();
        processManager.Begin();
    }
}