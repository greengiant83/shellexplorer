using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public interface IProcessStep
{
    ProcessManager ProcessManager { get; set; }
    void Activate();
    void Revert();
    void Update();
}

public abstract class ProcessStep<ProcessManagerType> : IProcessStep where ProcessManagerType : ProcessManager
{
    public ProcessManager ProcessManager
    {
        get { return processManager; }
        set { processManager = (ProcessManagerType)value; }
    }

    public ProcessManagerType processManager { get; set; }

    public abstract void Activate();

    public abstract void Revert();

    public virtual void Update() { }
}

public abstract class ProcessManager
{
    public int CurrentStepIndex = -1;

    public bool IsComplete { get { return CurrentStepIndex < 0; } }

    protected IProcessStep[] Steps;

    public IProcessStep CurrentStep
    {
        get
        {
            return (CurrentStepIndex >= 0 && CurrentStepIndex < Steps.Length) ? Steps[CurrentStepIndex] : null;
        }
    }

    protected abstract IProcessStep[] GetSteps();

    public virtual void Begin()
    {
        initializeSteps();

        CurrentStepIndex = 0;
        Steps[CurrentStepIndex].Activate();
    }

    public void Update()
    {
        if (CurrentStep != null) CurrentStep.Update();
    }

    public void GoToNextStep()
    {
        if (CurrentStepIndex < Steps.Length - 1)
        {
            CurrentStepIndex++;
            Steps[CurrentStepIndex].Activate();
        }
        else
        {
            Debug.Log("Process complete");
            CurrentStepIndex = -1;
        }
    }

    public void GoToPrevStep()
    {
        throw new NotImplementedException();
    }

    void initializeSteps()
    {
        Steps = GetSteps();
        foreach (var step in Steps)
        {
            step.ProcessManager = this;
        }
    }
}