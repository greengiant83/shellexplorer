using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AddCubeProcess : ProcessManager
{
    public GameObject NewCube;
    public ThreeDCursor Cursor;
    public Vector3? Point1;
    public Vector3? Point2;
    public Vector3? Point3;

    protected override IProcessStep[] GetSteps()
    {
        return new IProcessStep[] 
        {
            new AcquireFirstPoint(),
            new AcquireSecondPoint(),
            new AcquireThirdPoint(),
            new Finalize()
        };
    }

    public AddCubeProcess()
    {
        this.Cursor = GameObject.Find("Right Hand").transform.Find("Tool Attachment").GetComponent<ThreeDCursor>();
    }

    #region -- Process Steps --
    class AcquireFirstPoint : ProcessStep<AddCubeProcess>
    {
        string instructions { get { return "Select first corner of footprint"; } }

        public override void Activate()
        {
            StatusBar.SetStatus(instructions);
            processManager.Cursor.IsSamplingFromLaser = true;
        }

        public override void Update()
        {
            processManager.Point1 = processManager.Cursor.Crosshairs.transform.position;
            if (SixenseInput.GetController(SixenseHands.RIGHT).GetButtonDown(SixenseButtons.BUMPER))
            {                
                processManager.GoToNextStep();
            }
        }

        public override void Revert()
        {
            processManager.Point1 = null;
        }
    }

    class AcquireSecondPoint : ProcessStep<AddCubeProcess>
    {
        string instructions { get { return "Select second corner of footprint"; } }

        public override void Activate()
        {
            StatusBar.SetStatus(instructions);

            processManager.NewCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            processManager.NewCube.GetComponent<Collider>().enabled = false;
        }

        public override void Update()
        {
            processManager.Point2 = processManager.Cursor.Crosshairs.transform.position;

            var height = 0.01f;
            var p12delta = processManager.Point2.Value - processManager.Point1.Value;
            processManager.NewCube.transform.position = ((processManager.Point1.Value + processManager.Point2.Value) / 2) + (Vector3.up * (height / 2));
            processManager.NewCube.transform.localScale = new Vector3(p12delta.x, height, p12delta.z);

            if (SixenseInput.GetController(SixenseHands.RIGHT).GetButtonDown(SixenseButtons.BUMPER))
            {
                ProcessManager.GoToNextStep();
            }
        }

        public override void Revert()
        {
            GameObject.Destroy(processManager.NewCube);
            processManager.NewCube = null;
            processManager.Point2 = null;
        }
    }

    class AcquireThirdPoint : ProcessStep<AddCubeProcess>
    {
        string instructions { get { return "Set height"; } }

        public override void Activate()
        {
            StatusBar.SetStatus(instructions);
            processManager.Cursor.IsSamplingFromLaser = false;
        }

        public override void Update()
        {
            processManager.Point3 = processManager.Cursor.Crosshairs.transform.position;

            var height = processManager.Point3.Value.y - processManager.Point2.Value.y;
            var p12delta = processManager.Point2.Value - processManager.Point1.Value;
            processManager.NewCube.transform.position = ((processManager.Point1.Value + processManager.Point2.Value) / 2) + (Vector3.up * (height/2));
            processManager.NewCube.transform.localScale = new Vector3(p12delta.x, height, p12delta.z);

            if (SixenseInput.GetController(SixenseHands.RIGHT).GetButtonDown(SixenseButtons.BUMPER))
            {
                ProcessManager.GoToNextStep();
            }
        }

        public override void Revert()
        {
        }
    }

    class Finalize : ProcessStep<AddCubeProcess>
    {
        public override void Activate()
        {
            StatusBar.SetStatus("");
            processManager.NewCube.GetComponent<Collider>().enabled = true;
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

public class AddCubeCommand : ToolbarCommand
{
    ProcessManager processManager;

    void Start () 
    {
	}

	void Update () 
    {
        if (processManager != null)
        {
            processManager.Update();
            if (processManager.IsComplete) processManager = null;
        }
	}

    public override void Activate(object Sender)
    {
        processManager = new AddCubeProcess();
        processManager.Begin();
    }
}
